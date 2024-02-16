namespace HeroFishing.Socket.Matchgame {
    public class DROPSPELL : SocketContent {
        //class名稱就是封包的CMD名稱
        public int AttackID { get; private set; }
        /// <summary>
        /// DropSpell表ID
        /// </summary>
        public int DropSpellJsonID { get; private set; }

        public DROPSPELL(int _dropSpellJsonID) {
            AttackID = -1;
            DropSpellJsonID = _dropSpellJsonID;
        }

        public DROPSPELL(int _attackID, int _dropSpellJsonID) {
            AttackID = _attackID;
            DropSpellJsonID = _dropSpellJsonID;
        }
    }
}