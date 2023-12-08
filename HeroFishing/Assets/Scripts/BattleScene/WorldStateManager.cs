using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct FreezeTag : IComponentData { }

public class WorldStateManager : MonoBehaviour {
    private static WorldStateManager _instance;
    public static WorldStateManager Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<WorldStateManager>();
                if (_instance == null) {
                    _instance = new GameObject("World State Manager").AddComponent<WorldStateManager>();
                    DontDestroyOnLoad(_instance);
                    _instance.Init();
                }
            }
            return _instance;
        }
    }

    public bool IsFrozen { get; private set; }

    private Entity _worldStateEntity;
    private EntityManager _entityManager;

    private void Init() {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _worldStateEntity = _entityManager.CreateEntity();
    }

    public void Freeze(bool active) {
        if (active) {
            _entityManager.AddComponentData(_worldStateEntity, new FreezeTag());
        }
        else {
            _entityManager.RemoveComponent<FreezeTag>(_worldStateEntity);
        }
    }
}