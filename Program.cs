using System.Text;
using Ecotrack_Api.Application.Auth;
using Ecotrack_Api.Config;
using Ecotrack_Api.Infrastructure;
using Ecotrack_Api.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ===== Options (appsettings.json) =====
builder.Services.Configure<MongoOptions>(builder.Configuration.GetSection("Mongo"));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

// ===== Mongo =====
builder.Services.AddSingleton<IMongoContext, MongoContext>();

// ===== Repositorios =====
builder.Services.AddScoped<IUserRepository, UserRepository>();

// ===== Servicios de aplicación =====
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ===== Controllers =====
builder.Services.AddControllers();

// ===== Swagger + JWT (botón Authorize) =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ecotrack API", Version = "v1" });

    // Seguridad: Bearer JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introduce: Bearer {tu_token_jwt}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });

    // (Opcional) Incluir comentarios XML si habilitas <GenerateDocumentationFile>true</GenerateDocumentationFile> en el .csproj
    // var xml = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // var xmlPath = Path.Combine(AppContext.BaseDirectory, xml);
    // c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

// ===== CORS (ajusta el origin de tu frontend si aplica) =====
const string CorsPolicy = "CorsPolicy";
builder.Services.AddCors(o => o.AddPolicy(CorsPolicy, p =>
{
    p.WithOrigins("http://localhost:5173")
     .AllowAnyHeader()
     .AllowAnyMethod();
}));

// ===== Auth JWT =====
var jwtSection = builder.Configuration.GetSection("Jwt");
var keyBytes = Encoding.UTF8.GetBytes(jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key no configurado"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// ===== Pipeline =====
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ecotrack API v1");
    // c.RoutePrefix = string.Empty; // <- opcional: sirve Swagger en "/"
});

app.UseHttpsRedirection();
app.UseCors(CorsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ===== Tareas al iniciar (índices + ping opcional) =====
using (var scope = app.Services.CreateScope())
{
    try
    {
        var usersRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        await usersRepo.EnsureIndexesAsync();

        var ctx = scope.ServiceProvider.GetRequiredService<IMongoContext>();
        await ctx.Database.RunCommandAsync<MongoDB.Bson.BsonDocument>("{ ping: 1 }");
        Console.WriteLine("✅ MongoDB conectado correctamente.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️  Startup sin Mongo OK, pero la conexión falló: {ex.Message}");
        // No relances la excepción: así Swagger carga y puedes depurar
        // throw;  <-- NO
    }
}

app.Run();

