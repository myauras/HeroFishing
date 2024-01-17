using UnityEngine;

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
        public int MonsterIdx { get; private set; }
        /// <summary>
        /// 是否為鎖定攻擊
        /// </summary>
        public bool AttackLock { get; private set; }
        /// <summary>
        /// 攻擊施放位置
        /// </summary>
        public double[] AttackPos { get; private set; }
        /// <summary>
        /// 攻擊施放方向
        /// </summary>
        public double[] AttackDir { get; private set; }
        public ATTACK_TOCLIENT() {
        }
    }
}