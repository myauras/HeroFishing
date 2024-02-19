using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket.Matchgame {
    public class DROPSPELL_TOCLIENT : SocketContent {
        //class名稱就是封包的CMD名稱

        public bool Success { get; private set; }
        public int PlayerIdx { get; private set; }
        public int DropSpellJsonID { get; private set; }

        public DROPSPELL_TOCLIENT() {
        }
    }
}