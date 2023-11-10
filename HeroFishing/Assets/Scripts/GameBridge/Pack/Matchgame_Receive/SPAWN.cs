using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket.Matchgame {
    public class SPAWN : SocketContent {
        /// <summary>
        /// 怪物JsonID
        /// </summary>
        public int MonsterID { get; private set; }
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