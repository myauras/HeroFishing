const axios = require('axios');
const cheerio = require('cheerio');
const fs = require('fs');
const path = require('path');

const url = 'https://leagueoflegends.fandom.com/wiki/Ahri/LoL/Audio';

axios(url).then(response => {
    const html = response.data;
    const $ = cheerio.load(html);
    const audioElements = $('li span audio');

    audioElements.each((i, elem) => {
        const audioSrc = $(elem).find('source').attr('src');
        let title = $(elem).parent().next().find('a').attr('title');
        if(title){
            // 移除 'File:' 字樣並將餘下的標題用作文件名
            title = title.replace('File:', '');

            const fileDirectory = path.join(__dirname, '..', 'Audios/LOL/Ahri');
    
            // 檢查目錄是否存在，若不存在則創建
            if (!fs.existsSync(fileDirectory)){
                fs.mkdirSync(fileDirectory, { recursive: true });
            }
    
            const filePath = path.join(fileDirectory, title);
    
            axios({
                method: 'get',
                url: audioSrc,
                responseType: 'stream',
            }).then(response => {
                response.data.pipe(fs.createWriteStream(filePath));
            });
        }
    });
});