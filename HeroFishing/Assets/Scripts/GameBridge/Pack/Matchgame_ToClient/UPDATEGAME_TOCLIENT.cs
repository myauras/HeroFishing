using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket.Matchgame {
    public class UPDATEGAME_TOCLIENT : SocketContent {
        public double GameTime { get; private set; }

        public UPDATEGAME_TOCLIENT() {
        }
    }
}