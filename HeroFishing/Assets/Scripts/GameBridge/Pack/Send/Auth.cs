using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket {    
    public class AUTH : SocketContent {
        //class名稱就是封包的CMD名稱
        public string Token { get; private set; }

        public AUTH(string token) {
            Token = token;
        }
    }
}