using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket.Matchgame {
    public class ADDBOT_TOCLIENT : SocketContent {
        //class名稱就是封包的CMD名稱

        public bool Success { get; private set; }
        public int Index { get; private set; } // 玩家在房間的索引, 也就是座位
        public ADDBOT_TOCLIENT() {
        }
    }
}