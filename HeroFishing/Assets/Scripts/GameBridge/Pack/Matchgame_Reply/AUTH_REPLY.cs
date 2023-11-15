using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket.Matchgame {
    public class AUTH_REPLY : SocketContent {
        //class名稱就是封包的CMD名稱

        public bool IsAuth { get; private set; }
        public string ConnToken { get; private set; } // Auth驗證成功後之後要連Matchgame都是透過這個連線Token
        public int Index { get; private set; } // 玩家在房間的索引, 也就是座位
        public AUTH_REPLY() {
        }
    }
}