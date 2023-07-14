using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;

namespace ECSTest {
    class SpawnerAuthoring : MonoBehaviour {
        [SerializeField] public GameObject Prefab;
        [SerializeField] public Vector3 RolePos;
        [SerializeField] public Vector3 RoleRot;
        [SerializeField] public Animator Animator;

        public void Create() {
        }
    }
    class SpawnerBaker : Baker<SpawnerAuthoring> {
        public override void Bake(SpawnerAuthoring authoring) {
            Debug.Log("Bake");
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new RoleSpawner {
                // By default, each authoring GameObject turns into an Entity.
                // Given a GameObject (or authoring component), GetEntity looks up the resulting Entity.
                Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
                Pos = authoring.RolePos,
                Rot = authoring.RoleRot,
                Animator = GetEntity(authoring.Animator, TransformUsageFlags.Dynamic),
            });
        }
    }


}