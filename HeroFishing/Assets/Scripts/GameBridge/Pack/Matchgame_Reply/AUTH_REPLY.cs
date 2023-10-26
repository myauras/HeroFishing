using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket.Matchgame {
    public class AUTH_REPLY : SocketContent {
        public bool IsAuth { get; private set; }
        public string ConnToken { get; private set; }
        public AUTH_REPLY() {
        }
    }
}