const axios = require('axios');
const cheerio = require('cheerio');
const fs = require('fs');
const path = require('path');

const url = 'https://dota2.fandom.com/wiki/Ancient_Apparition/Responses';

axios(url).then(response => {
    const html = response.data;
    const $ = cheerio.load(html);
    const audioElements = $('li span audio');

    audioElements.each((i, elem) => {
        const audioSrc = $(elem).find('source').attr('src');
        let title = $(elem).parent().parent().contents().last().text().trim();
        if(title){
            // 移除 Windows 文件名中不允許的字符
            title = title.replace(/[\/\\:*\?"<>|]/g, '');

            const fileDirectory = path.join(__dirname, '..', 'Audios/DOTA2/Ancient_Apparition');
    
            // 檢查目錄是否存在，若不存在則創建
            if (!fs.existsSync(fileDirectory)){
                fs.mkdirSync(fileDirectory, { recursive: true });
            }
    
            const filePath = path.join(fileDirectory, title + '.mp3');
    
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