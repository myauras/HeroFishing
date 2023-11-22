using Cysharp.Threading.Tasks;
using HeroFishing.Battle;
using HeroFishing.Main;
using Scoz.Func;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEngine.Application;
using UnityEngine.UIElements;

public class PoolManager : MonoBehaviour {
    private static PoolManager _instance;
    public static PoolManager Instance => _instance;

    [SerializeField] private GameObject _bullet;
    [SerializeField] private GameObject _monster;

    private Dictionary<string, List<GameObject>> _pools;

    private const string POOL_BULLET = "bullet";

    public static PoolManager CreateNewInstance() {
        if (Instance != null) {
            WriteLog.Log("PoolManager�w�g�إ�");
        }
        else {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Common/PoolManager");
            GameObject go = Instantiate(prefab);
            _instance = go.GetComponent<PoolManager>();
            _instance.Init();
        }
        return Instance;
    }

    public void Init() {
        _instance = this;
        DontDestroyOnLoad(gameObject);
        _pools = new Dictionary<string, List<GameObject>>();
    }

    public void InitHeroSpell(HeroSpellJsonData data) {
        if (data.SubPrefabID != 0) {
            var path = string.Format("Bullet/BulletProjectile{0}_{1}", data.PrefabID, data.SubPrefabID);
            CreateParticleInstance(path);
        }

        var projectilePath = string.Format("Bullet/BulletProjectile{0}", data.PrefabID);
        CreateParticleInstance(projectilePath);

        var firePath = string.Format("Bullet/BulletFire{0}", data.PrefabID);
        CreateParticleInstance(firePath);

        var hitPath = string.Format("Bullet/BulletHit{0}", data.PrefabID);
        CreateParticleInstance(hitPath);
    }

    // Addressable Assets�Ϊ������
    public void Pop(string key, Vector3 position = default, Quaternion rotaiton = default, Transform parent = null, Action<GameObject> popCallback = null) {
        WriteLog.Log("pop " + key);
        // �p�G�ٵL�k�ѧO�Ӫ���A���s�إߤ@��List
        if (!_pools.TryGetValue(key, out var objList)) {
            objList = new List<GameObject>();
            _pools[key] = objList;
        }

        // �d�ݬO�_���ݩR��������A���h��^
        for (int i = 0; i < objList.Count; i++) {
            if (!objList[i].activeSelf) {
                objList[i].SetActive(true);
                objList[i].transform.SetParent(parent);
                objList[i].transform.SetLocalPositionAndRotation(position, rotaiton);
                popCallback?.Invoke(objList[i]);
                return;
            }
        }

        // �S���ݩR��������A�إ߷s����
        CreateParticleInstance(key, parent, go => {
            go.transform.SetLocalPositionAndRotation(position, rotaiton);
            go.SetActive(true);
            popCallback?.Invoke(go);
        });
    }

    // �l�u�Ϊ������
    public GameObject PopBullet(int prefabID, int subPrefabID) {
        WriteLog.Log("pop bullet");
        string bulletID;
        if (subPrefabID == 0) {
            bulletID = POOL_BULLET + prefabID.ToString();
        }
        else {
            bulletID = POOL_BULLET + prefabID + "_" + subPrefabID;
        }

        // �p�G�����l�u���A�إߡC
        if (!_pools.TryGetValue(bulletID, out var objList)) {
            objList = new List<GameObject>();
            _pools[bulletID] = objList;
        }

        // �T�{�O�_���ݩR�����l�u�A���N��^
        for (int i = 0; i < objList.Count; i++) {
            if (!objList[i].activeSelf) {
                objList[i].SetActive(true);
                return objList[i];
            }
        }

        // �S�����ܴN�إߤ@�Ӥl�u����
        GameObject bulletGO = Instantiate(_bullet, transform);
        bulletGO.name = _bullet.name;
        objList.Add(bulletGO);
        return bulletGO;
    }

    // �^������
    public void Push(GameObject obj) {
        WriteLog.Log("push " + obj.name);
        obj.SetActive(false);
        obj.transform.SetParent(transform);
    }

    private void CreateParticleInstance(string key, Transform parent = null, Action<GameObject> callback = null) {
        if (parent == null) {
            parent = transform;
        }

        if (!_pools.TryGetValue(key, out var list)) {
            list = new List<GameObject>();
            _pools[key] = list;
        }

        GameObjSpawner.SpawnParticleObjByPath(key, parent, (go, handle) => {
            AddressableManage.SetToChangeSceneRelease(handle);
            list.Add(go);
            go.SetActive(false);
            callback?.Invoke(go);
            WriteLog.Log("create new object " + go.name);
        });
    }
}
