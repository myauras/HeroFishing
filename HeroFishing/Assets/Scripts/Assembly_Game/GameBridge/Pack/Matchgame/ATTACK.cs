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
        public long MonsterIdx { get; private set; }

        public ATTACK(int _attackID, string _spellJsonID, long _monsterIdx) {
            AttackID = _attackID;
            SpellJsonID = _spellJsonID;
            MonsterIdx = _monsterIdx;
        }
    }
}