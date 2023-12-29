namespace HeroFishing.Socket.Matchgame {
    public class DROPSPELL : SocketContent {
        //class名稱就是封包的CMD名稱

        /// <summary>
        /// DropSpell表ID
        /// </summary>
        public int DropSpellJsonID { get; private set; }

        public DROPSPELL(int _dropSpellJsonID) {
            DropSpellJsonID = _dropSpellJsonID;
        }
    }
}