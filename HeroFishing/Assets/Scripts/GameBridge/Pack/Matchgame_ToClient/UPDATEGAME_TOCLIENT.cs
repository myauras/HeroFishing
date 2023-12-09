using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket.Matchgame {
    public class UPDATEGAME_TOCLIENT : SocketContent {
        /// <summary>
        /// 遊戲開始X秒
        /// </summary>
        public double GameTime { get; private set; }
        /// <summary>
        /// 所有玩家的總獲得點數
        /// </summary>
        public long[] PlayerGainPoints { get; private set; } = new long[4];

        public UPDATEGAME_TOCLIENT() {
        }
    }
}