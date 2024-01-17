namespace HeroFishing.Socket.Matchgame {
    public class LEAVE_TOCLIENT : SocketContent {
        public int PlayerIdx { get; private set; }
        public LEAVE_TOCLIENT() { }
    }
}