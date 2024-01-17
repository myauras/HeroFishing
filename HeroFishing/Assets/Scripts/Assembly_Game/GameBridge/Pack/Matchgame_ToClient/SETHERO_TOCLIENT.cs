using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket.Matchgame {
    public class SETHERO_TOCLIENT : SocketContent {
        //class名稱就是封包的CMD名稱

        public int[] HeroIDs { get; private set; }
        public string[] HeroSkinIDs { get; private set; }
        public SETHERO_TOCLIENT() {
        }
    }
}