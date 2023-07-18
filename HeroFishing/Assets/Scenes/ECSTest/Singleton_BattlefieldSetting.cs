using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
namespace HeroFishing.Battlefield {
    public struct Singleton_BattlefieldSetting : IComponentData {
        public Unity.Mathematics.float3 HeroPos;
    }
}