using HeroFishing.Main;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HeroFishing.Battle {
    public class Monster : Role {
        public MonsterData MyData { get; private set; }

        public void SetData(MonsterData data) {
            MyData = data;
        }

    }

}