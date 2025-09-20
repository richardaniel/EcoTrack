// models/BadgeAward.js
const { Schema, model, Types } = require('mongoose');

const badgeAwardSchema = new Schema({
    organizationId: { type: Types.ObjectId, ref: 'Organization', required: true },
    badgeId: { type: Types.ObjectId, ref: 'Badge', required: true },
    period: { type: String, trim: true }, // ej. "2025-Q1" o "2025-09"
    awardedAt: { type: Date, default: Date.now },
    evidence: { type: Schema.Types.Mixed } // KPIs, links a reportes, etc.
}, { timestamps: true });

badgeAwardSchema.index({ organizationId: 1, badgeId: 1, awardedAt: -1 });

module.exports = model('BadgeAward', badgeAwardSchema);

