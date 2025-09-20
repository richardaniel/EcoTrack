// models/Organization.js
const { Schema, model, Types } = require('mongoose');

const organizationSchema = new Schema({
    name: { type: String, required: true, trim: true },
    country: { type: String, trim: true },
    sector: { type: String, trim: true },
    settings: {
        timezone: { type: String, default: 'America/Tegucigalpa' },
        currency: { type: String, default: 'HNL' }
    }
}, { timestamps: true });

organizationSchema.index({ name: 1 });

module.exports = model('Organization', organizationSchema);
