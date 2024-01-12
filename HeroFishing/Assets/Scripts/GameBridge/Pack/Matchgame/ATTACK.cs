using UnityEngine;
using Unity.Mathematics;

namespace HeroFishing.Socket.Matchgame {
    public class ATTACK : SocketContent {
        //class名稱就是封包的CMD名稱

        /// <summary>
        /// 攻擊ID為攻擊流水號, 同個攻擊但是不同波次要送同一個AttackID, 假設好運姊的彈幕有三波擊中, 這三波送的AttackID需要一樣
        /// </summary>
        public int AttackID { get; private set; }
        /// <summary>
        /// 技能表ID
        /// </summary>
        public string SpellJsonID { get; private set; }
        /// <summary>
        /// 目標怪物索引, 沒有目標傳-1就可以
        /// </summary>
        public int MonsterIdx { get; private set; }
        ///// <summary>
        ///// 是否為鎖定攻擊
        ///// </summary>
        //public bool AttackLock { get; private set; }
        ///// <summary>
        ///// 攻擊施放位置
        ///// </summary>
        //public float[] AttackPos { get; private set; }
        ///// <summary>
        ///// 攻擊施放方向
        ///// </summary>
        //public float[] AttackDir { get; private set; }

        public ATTACK(int _attackID, string _spellJsonID, int _monsterIdx, bool _attackLock, Vector3 _attackPos, Vector3 _attackDir) {
            AttackID = _attackID;
            SpellJsonID = _spellJsonID;
            MonsterIdx = _monsterIdx;
            //AttackLock = _attackLock;
            //AttackPos = new float[3];
            //AttackDir = new float[3];
            //for (int i = 0; i < 3; i++) {
            //    AttackPos[i] = _attackPos[i];
            //    AttackDir[i] = _attackDir[i];
            //}
        }
    }
}