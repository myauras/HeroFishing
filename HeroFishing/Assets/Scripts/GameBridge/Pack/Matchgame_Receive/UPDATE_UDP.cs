using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket.Matchgame {
    public class UPDATE_UDP : SocketContent {
        public double ServerTime { get; private set; }

        public UPDATE_UDP() {
        }
    }
}