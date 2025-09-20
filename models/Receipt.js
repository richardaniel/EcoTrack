// models/Receipt.js
const { Schema, model, Types } = require('mongoose');

const receiptSchema = new Schema({
    organizationId: { type: Types.ObjectId, ref: 'Organization', required: true },
    provider: { type: String, trim: true },
    period: { year: { type: Number, required: true }, month: { type: Number, required: true, min: 1, max: 12 } },
    total: { type: Number, min: 0 },
    currency: { type: String, default: 'HNL' },
    fileUrl: { type: String, trim: true },
    status: { type: String, enum: ['uploaded', 'parsed', 'linked'], default: 'uploaded' },
    classifiedAs: { type: String, enum: ['electricity', 'diesel', 'gasolina', 'gas_natural', 'vapor', 'otros'], default: 'otros' },
    ocr: {
        success: { type: Boolean, default: false },
        fields: { type: Schema.Types.Mixed } // JSON con campos extraídos
    },
    lineItems: [{
        description: String,
        quantity: Number,
        unitPrice: Number,
        total: Number
    }],
    linkedConsumptionId: { type: Types.ObjectId } // Puede apuntar a electricity/fuel/energy según el caso
}, { timestamps: true });

receiptSchema.index({ organizationId: 1, 'period.year': 1, 'period.month': 1 });
receiptSchema.index({ provider: 1 });

module.exports = model('Receipt', receiptSchema);
