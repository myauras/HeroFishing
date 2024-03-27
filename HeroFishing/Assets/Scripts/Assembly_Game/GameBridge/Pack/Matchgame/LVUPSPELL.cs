namespace HeroFishing.Socket.Matchgame {
    public class LVUPSPELL : SocketContent {
        //class名稱就是封包的CMD名稱
        public int SpellIdx { get; private set; } //英雄技能等級, idx傳0~2(技能1~技能3)

        public LVUPSPELL(int _spellIdx) {
            SpellIdx = _spellIdx;
        }
    }
}