// models/EnergyConsumption.js
const { Schema, model, Types } = require('mongoose');

const energyConsumptionSchema = new Schema({
    organizationId: { type: Types.ObjectId, ref: 'Organization', required: true },
    period: { year: { type: Number, required: true }, month: { type: Number, required: true, min: 1, max: 12 } },
    energyType: { type: String, enum: ['gas_natural', 'vapor', 'refrigerante', 'biomasa', 'otro'], required: true },
    quantity: { type: Number, required: true, min: 0 },
    unit: { type: String, enum: ['kg', 't', 'm3', 'kWh_thermal', 'BTU', 'otro'], required: true },
    emissionFactor: {
        value: { type: Number, required: true, min: 0 }, // kgCO2e por unidad
        unit: { type: String, default: 'kgCO2e/unit' },
        source: { type: String, default: 'Oficial/IEA/IPCC' },
        year: { type: Number }
    },
    co2e: { type: Number, required: true, min: 0 },
    scope: { type: String, enum: ['scope1', 'scope2', 'scope3'], default: 'scope1' },
    source: { type: String, enum: ['manual', 'receipt', 'iot'], default: 'manual' },
    receiptId: { type: Types.ObjectId, ref: 'Receipt' }
}, { timestamps: true });

energyConsumptionSchema.index({ organizationId: 1, 'period.year': 1, 'period.month': 1 });
energyConsumptionSchema.index({ energyType: 1 });

module.exports = model('EnergyConsumption', energyConsumptionSchema);
