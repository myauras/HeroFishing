using System;
using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket.Matchgame {
    public class UPDATESCENE_TOCLIENT : SocketContent {
        //class名稱就是封包的CMD名稱

        public Spawn[] Spawns { get; private set; }// 生怪清單(仍有效的生怪事件才傳, 如果該事件的怪物全數死亡就不用傳)
        public SceneEffect[] SceneEffects { get; private set; }

        public UPDATESCENE_TOCLIENT() {
        }
    }
    public class Spawn {
        public int RID { get; private set; }// 路徑JsonID, RouteJsonID
        public double STime { get; private set; }// 在遊戲時間第X秒時被產生的, SpawnTime
        public bool IsB { get; private set; }// 是否為Boss生怪, IsBoss

        public Monster[] Ms { get; private set; }// 怪物清單, Monsters
    }

    public class Monster {
        public int ID { get; private set; } // 怪物JsonID, JsonID
        public int Idx { get; private set; }// 怪物索引
        public bool Death { get; private set; }// 是否已死亡
        public double LTime { get; private set; }// 離開時間, LTime
        public MonsterEffect[] Effects { get; private set; }
    }

    public class MonsterEffect {
        public string Name { get; private set; }
        public float AtTime { get; private set; }
        public float Duration { get; private set; }
    }

    public class SceneEffect {
        public string Name { get; private set; }
        public double Value { get; private set; }
        public double AtTime { get; private set; }
        public double Duration { get; private set; }
    }
}