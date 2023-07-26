
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using System;

namespace HeroFishing.Battle {
    public class MonsterAuthoring : MonoBehaviour {

        [SerializeField] float Radius;
        [SerializeField] GameObject Prefab;

        class Baker : Baker<MonsterAuthoring> {
            public override void Bake(MonsterAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MonsterValue {
                    Radius = authoring.Radius,
                });
                AddComponentObject(entity, new MonsterGOPrefab() {
                    Prefab = authoring.Prefab,
                });
            }
        }
    }

    public struct MonsterValue : IComponentData {
        public float Radius;
    }
    public class MonsterGOPrefab : IComponentData {
        public GameObject Prefab;
    }
    public class MonsterGOInstance : IComponentData, IDisposable {
        public GameObject Instance;

        public void Dispose() {
            UnityEngine.Object.DestroyImmediate(Instance);
        }
    }
}

