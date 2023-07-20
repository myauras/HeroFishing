using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

namespace HeroFishing.Battlefield {
    public partial struct AttackData {
        public float3 AttackerPos;
        public float3 TargetPos;
        public float3 Direction;
        public float BulletSpeed;


    }
}
