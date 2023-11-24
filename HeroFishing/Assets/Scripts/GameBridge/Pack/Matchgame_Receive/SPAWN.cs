using System;
using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket.Matchgame {
    public class SPAWN : SocketContent {
        //class名稱就是封包的CMD名稱

        /// <summary>
        /// 是否為Boss生怪
        /// </summary>
        public bool IsBoss { get; private set; }
        /// <summary>
        /// 怪物JsonIDs
        /// </summary>
        public int[] MonsterIDs { get; private set; }
        /// <summary>
        /// 怪物唯一索引清單
        /// </summary>
        public int[] MonsterIdxs { get; private set; }
        /// <summary>
        /// 路徑JsonID
        /// </summary>
        public int RouteID { get; private set; }
        /// <summary>
        /// 在遊戲時間第X秒時被產生的
        /// </summary>
        public double SpawnTime { get; private set; }

        public SPAWN() {
        }
    }
}