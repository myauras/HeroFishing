using HeroFishing.Battle;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private GameObject _freezeEffect;

    public bool IsFrozen { get; private set; }

    //private Entity _worldStateEntity;
    //private EntityManager _entityManager;

    private void Init() {
        //_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        //_worldStateEntity = _entityManager.CreateEntity();
    }

    public void Freeze(bool active) {
        IsFrozen = active;
        if (_freezeEffect == null) {
            SetupFreezeModel(active);
        }
        else {
            _freezeEffect.SetActive(active);
            Monster.Freeze(active);
            if (active) {
                //_entityManager.AddComponentData(_worldStateEntity, new FreezeTag());
                //SceneMaterialConverter.Instance?.Freeze();
            }
            else {
                //_entityManager.RemoveComponent<FreezeTag>(_worldStateEntity);
                //SceneMaterialConverter.Instance?.Unfreeze();
            }
        }
    }

    private void SetupFreezeModel(bool active) {
        AddressablesLoader.GetParticle("Scene/Freeze/SnowStorm", (obj, handle) => {
            AddressableManage.SetToChangeSceneRelease(handle);
            _freezeEffect = Instantiate(obj);
            Freeze(active);
        });
    }
}