namespace HeroFishing.Socket.Matchgame {
    public class AUTO : SocketContent {
        /// <summary>
        /// ¬O§_¶}±Òauto¼Ò¦¡
        /// </summary>
        public bool IsAuto { get; private set; }
        public AUTO(bool isAuto) {
            IsAuto = isAuto;
        }
    }
}
