const mongoose = require('mongoose');

// 定义模型
const playerSchema = new mongoose.Schema({
  name: {
    type: String,
    required: true
  }
}, { timestamps: true });

const Player = mongoose.model('Player', playerSchema);

module.exports = Player;