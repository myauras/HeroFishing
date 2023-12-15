namespace HeroFishing.Socket.Matchgame {
    public class HIT : SocketContent {
        //class名稱就是封包的CMD名稱

        /// <summary>
        /// 攻擊ID為攻擊流水號, 同個攻擊但是不同波次要送同一個AttackID, 假設好運姊的彈幕有三波擊中, 這三波送的AttackID需要一樣
        /// </summary>
        public int AttackID { get; private set; }
        /// <summary>
        /// 此次命中怪物索引清單
        /// </summary>
        public int[] MonsterIdxs { get; private set; }
        /// <summary>
        /// 技能表ID
        /// </summary>
        public string SpellJsonID { get; private set; }

        public HIT(int _attackID, int[] _monsterIdxs, string _spellJsonID) {
            AttackID = _attackID;
            MonsterIdxs = _monsterIdxs;
            SpellJsonID = _spellJsonID;
        }
    }
}