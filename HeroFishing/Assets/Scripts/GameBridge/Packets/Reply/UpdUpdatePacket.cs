using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket {
    public class UdpUpdatePacket : SocketContent {
        public double ServerTime;

        public UdpUpdatePacket() {
        }
    }
}