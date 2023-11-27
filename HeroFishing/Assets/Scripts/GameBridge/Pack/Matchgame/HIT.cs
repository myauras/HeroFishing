namespace HeroFishing.Socket.Matchgame {
    public class HIT : SocketContent {
        //class名稱就是封包的CMD名稱

        /// <summary>
        /// 攻擊ID格式為 [玩家index]_[攻擊流水號] (攻擊流水號(AttackID)是client端送來的施放攻擊的累加流水號
	    /// EX. 2_3就代表房間座位2的玩家進行的第3次攻擊
        /// </summary>
        public string AttackID { get; private set; }
        /// <summary>
        /// 此次命中怪物索引清單
        /// </summary>
        public int[] MonsterIdxs { get; private set; }
        /// <summary>
        /// 技能表ID
        /// </summary>
        public string SpellJsonID { get; private set; }

        public HIT(string _attackID, int[] _monsterIdxs, string _spellJsonID) {
            AttackID = _attackID;
            MonsterIdxs = _monsterIdxs;
            SpellJsonID = _spellJsonID;
        }
    }
}