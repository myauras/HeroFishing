using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket.Matchgame {
    public class MONSTERDIE_TOCLIENT : SocketContent {
        //class名稱就是封包的CMD名稱

        public DieMonster[] DieMonsters { get; private set; }// 死亡怪物清單

        public MONSTERDIE_TOCLIENT() {
        }
    }
    public class DieMonster {
        public int ID { get; private set; } // 怪物JsonID
        public int Idx { get; private set; }// 怪物索引

    }
}