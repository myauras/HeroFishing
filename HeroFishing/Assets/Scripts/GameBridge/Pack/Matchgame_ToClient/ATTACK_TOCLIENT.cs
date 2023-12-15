namespace HeroFishing.Socket.Matchgame {
    public class ATTACK_TOCLIENT : SocketContent {
        //class名稱就是封包的CMD名稱

        /// <summary>
        /// 玩家索引
        /// </summary>
        public int PlayerIdx { get; private set; }
        /// <summary>
        /// 技能表ID
        /// </summary>
        public string SpellJsonID { get; private set; }
        /// <summary>
        /// 目標怪物索引, 沒有目標傳-1就可以
        /// </summary>
        public long MonsterIdx { get; private set; }
        public ATTACK_TOCLIENT() {
        }
    }
}