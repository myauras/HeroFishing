const express = require('express');
const mongoose = require('mongoose');

// 連線到DB
mongoose.connect('mongodb+srv://scozirge2:<password>@cluster-herofishing.1jp5bq2.mongodb.net/?retryWrites=true&w=majority', {
  useNewUrlParser: true,
  useUnifiedTopology: true
});
const db = mongoose.connection;
db.on('error', console.error.bind(console, 'MongoDB 連線錯誤：'));
db.once('open', function() {
  console.log('已連接到DB');
});


// 建立Express app
const app = express();

// 定義路由
app.get('/api/test', async (req, res) => {
    console.log("test")
});

// Server Port偵聽
const port = 3000;
app.listen(port, () => {
  console.log(`Server 已偵聽Port: ${port}`);
});