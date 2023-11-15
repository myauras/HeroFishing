namespace HeroFishing.Socket.Matchgame {
    public class ACTION_SETHERO : SocketContent {
        //class名稱就是封包的CMD名稱
        public int Index { get; private set; } //玩家在房間中的索引, 也就是座位ID
        public int HeroID { get; private set; }
        public string HeroSkinID { get; private set; }

        public ACTION_SETHERO(int _index, int _heroID, string _heroSkinID) {
            Index = _index;
            HeroID = _heroID;
            HeroSkinID = _heroSkinID;
        }
    }
}