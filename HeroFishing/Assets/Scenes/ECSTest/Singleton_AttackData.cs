using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
namespace HeroFishing.Battlefield {
    public struct Singleton_AttackData : IComponentData {
        public float3 AttackerPos;
        public float3 TargetPos;
    }
}