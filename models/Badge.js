// models/Badge.js
const { Schema, model, Types } = require('mongoose');

const badgeSchema = new Schema({
    key: { type: String, required: true, unique: true, trim: true }, // ej. "reduction_20"
    title: { type: String, required: true },
    description: { type: String },
    iconUrl: { type: String, trim: true },
    criteria: { type: Schema.Types.Mixed, required: true } // JSON con reglas (ej. % reducción vs baseline)
}, { timestamps: true });

badgeSchema.index({ key: 1 }, { unique: true });

module.exports = model('Badge', badgeSchema);
