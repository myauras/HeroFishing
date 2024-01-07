using System;
using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket.Matchgame {
    public class HIT_TOCLIENT : SocketContent {
        //class名稱就是封包的CMD名稱

        /// <summary>
        /// 擊殺怪物索引清單, [1,1,3]就是依次擊殺索引為1,1與3的怪物
        /// </summary>
        public int[] KillMonsterIdxs { get; private set; }

        /// <summary>
        /// 獲得點數清單, [1,1,3]就是依次獲得點數1,1與3
        /// </summary>
        public long[] GainPoints { get; private set; }

        /// <summary>
        /// 獲得英雄經驗清單, [1,1,3]就是依次獲得英雄經驗1,1與3
        /// </summary>
        public int[] GainHeroExps { get; private set; }

        /// <summary>
        /// 獲得技能充能清單, [1,1,3]就是依次獲得技能1,技能1,技能3的充能
        /// </summary>
        public int[] GainSpellCharges { get; private set; }

        /// <summary>
        /// 獲得掉落清單, [1,1,3]就是依次獲得DropJson中ID為1,1與3的掉落
        /// </summary>
        public int[] GainDrops { get; private set; }

        public HIT_TOCLIENT() {
        }
    }
}