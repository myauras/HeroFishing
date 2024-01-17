using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket.Matchgame {
    public class UPDATEPLAYER_TOCLIENT : SocketContent {
        //class名稱就是封包的CMD名稱

        public Player[] Players;

        public UPDATEPLAYER_TOCLIENT() {
        }
    }

    public class Player {
        public string ID { get; private set; }
        public int Idx { get; private set; }
        public long GainPoints { get; private set; }
        public PlayerBuff[] PlayerBuffs { get; private set; }
    }

    public class PlayerBuff {
        public string Name { get; private set; }
        public double Value { get; private set; }
        public double AtTime { get; private set; }
        public double Duration { get; private set; }
    }
    public class PlayerStatus {

    }
}