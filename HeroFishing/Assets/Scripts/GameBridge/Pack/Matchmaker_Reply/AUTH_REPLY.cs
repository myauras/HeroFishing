using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket.Matchmaker {
    public class AUTH_REPLY : SocketContent {
        public bool IsAuth { get; private set; }

        public AUTH_REPLY() {
        }
    }
}