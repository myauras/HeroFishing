using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket {
    public class Auth_Reply : SocketContent {
        public bool IsAuth { get; private set; }

        public Auth_Reply() {
        }
    }
}