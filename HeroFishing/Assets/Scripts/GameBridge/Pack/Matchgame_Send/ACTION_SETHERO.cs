namespace HeroFishing.Socket.Matchgame {
    public class ACTION_SETHERO : SocketContent {
        //class名稱就是封包的CMD名稱
        public int HeroID { get; private set; }
        public string HeroSkinID { get; private set; }

        public ACTION_SETHERO(int _heroID, string _heroSkinID) {
            HeroID = _heroID;
            HeroSkinID = _heroSkinID;
        }
    }
}