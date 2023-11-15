using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket.Matchgame {
    public class ACTION_SETHERO_REPLY : SocketContent {
        //class名稱就是封包的CMD名稱

        public int[] HeroIDs { get; private set; }
        public ACTION_SETHERO_REPLY() {
        }
    }
}