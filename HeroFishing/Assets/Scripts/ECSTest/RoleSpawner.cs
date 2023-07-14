using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECSTest {
    public struct RoleSpawner : IComponentData {
        public Entity Prefab;
        public Entity Animator;
        public float3 Pos;
        public float3 Rot;
    }
}
