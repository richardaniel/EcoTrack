// models/User.js
const { Schema, model, Types } = require('mongoose');

const userSchema = new Schema({
    organizationId: { type: Types.ObjectId, ref: 'Organization', required: true },
    name: { type: String, required: true, trim: true },
    email: { type: String, required: true, unique: true, lowercase: true, trim: true },
    passwordHash: { type: String, required: true },
    role: { type: String, enum: ['admin', 'analyst', 'viewer'], default: 'viewer' },
    lastLoginAt: { type: Date }
}, { timestamps: true });

userSchema.index({ email: 1 }, { unique: true });
userSchema.index({ organizationId: 1 });

module.exports = model('User', userSchema);
