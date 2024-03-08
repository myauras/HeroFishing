using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket.Matchgame {
    public class LVUPSPELL_TOCLIENT : SocketContent {
        //class名稱就是封包的CMD名稱

        public bool Success { get; private set; }

        public LVUPSPELL_TOCLIENT() {
        }
    }
}