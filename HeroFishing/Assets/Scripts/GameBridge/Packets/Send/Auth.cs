using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket {
    public class Auth : SocketContent {
        public string Token { get; private set; }

        public Auth(string token) {
            Token = token;
        }
    }
}