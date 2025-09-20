// models/ElectricityConsumption.js
const { Schema, model, Types } = require('mongoose');

const electricityConsumptionSchema = new Schema({
    organizationId: { type: Types.ObjectId, ref: 'Organization', required: true },
    period: { year: { type: Number, required: true }, month: { type: Number, required: true, min: 1, max: 12 } },
    meterId: { type: String, trim: true }, // si tienes múltiples medidores
    kWh: { type: Number, required: true, min: 0 },
    gridRegion: { type: String, trim: true }, // ej. 'HN'
    emissionFactor: {
        value: { type: Number, required: true, min: 0 }, // kgCO2e/kWh
        unit: { type: String, default: 'kgCO2e/kWh' },
        source: { type: String, default: 'Oficial - operador local/IEA/IPCC' },
        year: { type: Number }
    },
    co2e: { type: Number, required: true, min: 0 }, // kWh * factor
    scope: { type: String, default: 'scope2' },
    source: { type: String, enum: ['manual', 'receipt', 'iot'], default: 'manual' },
    receiptId: { type: Types.ObjectId, ref: 'Receipt' }
}, { timestamps: true });

electricityConsumptionSchema.index({ organizationId: 1, 'period.year': 1, 'period.month': 1 });
electricityConsumptionSchema.index({ receiptId: 1 });

module.exports = model('ElectricityConsumption', electricityConsumptionSchema);
