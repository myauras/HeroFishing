using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket {
    public class UdpUpdatePacket : BaseContent {
        public double ServerTime;

        public UdpUpdatePacket() {
        }
    }
}