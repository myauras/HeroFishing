namespace HeroFishing.Socket.Matchgame {
    public class LVUPSPELL : SocketContent {
        //class名稱就是封包的CMD名稱
        public int SpellIdx { get; private set; }

        public LVUPSPELL(int _spellIdx) {
            SpellIdx = _spellIdx;
        }
    }
}