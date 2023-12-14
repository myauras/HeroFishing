using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket.Matchgame {
    public class UPDATESCENE_TOCLIENT : SocketContent {
        //class名稱就是封包的CMD名稱

        public Spawn[] Spawns { get; private set; }

        public UPDATESCENE_TOCLIENT() {
        }
    }
    public class Spawn {
        public int RouteJsonID { get; private set; }
        public float SpanwTime { get; private set; }
        public bool IsBoss { get; private set; }

        public Monster[] Monsters { get; private set; }

    }
    public class Monster {
        public int JsonID { get; private set; }
        public int Idx { get; private set; }
        public bool Death { get; private set; }

    }
}