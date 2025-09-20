// models/FuelConsumption.js
const { Schema, model, Types } = require('mongoose');

const fuelConsumptionSchema = new Schema({
    organizationId: { type: Types.ObjectId, ref: 'Organization', required: true },
    period: { year: { type: Number, required: true }, month: { type: Number, required: true, min: 1, max: 12 } },
    fuelType: { type: String, enum: ['diesel', 'gasolina', 'glp', 'gln', 'queroseno', 'otro'], required: true },
    quantity: { type: Number, required: true, min: 0 },
    unit: { type: String, enum: ['L', 'gal', 'kg', 'm3'], required: true },
    emissionFactor: {
        value: { type: Number, required: true, min: 0 }, // kgCO2e por unidad
        unit: { type: String, default: 'kgCO2e/unit' },
        source: { type: String, default: 'Oficial/IPCC' },
        year: { type: Number }
    },
    co2e: { type: Number, required: true, min: 0 }, // quantity * factor
    scope: { type: String, default: 'scope1' },
    source: { type: String, enum: ['manual', 'receipt', 'telematics'], default: 'manual' },
    receiptId: { type: Types.ObjectId, ref: 'Receipt' }
}, { timestamps: true });

fuelConsumptionSchema.index({ organizationId: 1, 'period.year': 1, 'period.month': 1 });
fuelConsumptionSchema.index({ fuelType: 1 });

module.exports = model('FuelConsumption', fuelConsumptionSchema);
