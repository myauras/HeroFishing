namespace HeroFishing.Socket.Matchgame {
    public class SETHERO : SocketContent {
        //class名稱就是封包的CMD名稱
        public int HeroID { get; private set; }
        public string HeroSkinID { get; private set; }

        public SETHERO(int _heroID, string _heroSkinID) {
            HeroID = _heroID;
            HeroSkinID = _heroSkinID;
        }
    }
}