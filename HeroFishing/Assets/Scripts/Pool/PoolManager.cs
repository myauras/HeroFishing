using Cysharp.Threading.Tasks;
using HeroFishing.Battle;
using Scoz.Func;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PoolManager : MonoBehaviour {
    private static PoolManager _instance;
    public static PoolManager Instance => _instance;

    [SerializeField] private GameObject _bullet;
    [SerializeField] private GameObject _monster;

    private Dictionary<string, List<GameObject>> _pools;

    private const string POOL_BULLET = "bullet";

    public static PoolManager CreateNewInstance() {
        if (Instance != null) {
            WriteLog.Log("PoolManager已經建立");
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

    // Addressable Assets用的物件池
    public void Pop(string key, Vector3 position = default, Quaternion rotaiton = default, Transform parent = null, Action<GameObject> popCallback = null) {
        // 如果還無法識別該物件，重新建立一個List
        if (!_pools.TryGetValue(key, out var objList)) {
            objList = new List<GameObject>();
            _pools[key] = objList;
        }

        // 查看是否有待命中的物件，有則返回
        for (int i = 0; i < objList.Count; i++) {
            if (!objList[i].activeSelf) {
                objList[i].SetActive(true);
                objList[i].transform.SetParent(parent);
                objList[i].transform.SetLocalPositionAndRotation(position, rotaiton);
                popCallback?.Invoke(objList[i]);
                return;
            }
        }

        // 沒有待命中的物件，建立新物件
        Addressables.LoadAssetAsync<GameObject>(key).Completed += handle => {
            switch (handle.Status) {
                case AsyncOperationStatus.Succeeded:
                    AddressableManage.SetToChangeSceneRelease(handle);
                    GameObject go = Instantiate(handle.Result, transform);
                    go.transform.SetParent(parent);
                    go.transform.SetLocalPositionAndRotation(position, rotaiton);
                    objList.Add(go);
                    popCallback?.Invoke(go);
                    break;
            }
        };
    }

    // 子彈用的物件池
    public GameObject PopBullet() {
        // 如果未有子彈池，建立。
        if (!_pools.TryGetValue(POOL_BULLET, out var objList)) {
            objList = new List<GameObject>();
            _pools[POOL_BULLET] = objList;
        }

        // 確認是否有待命中的子彈，有就返回
        for (int i = 0; i < objList.Count; i++) {
            if (!objList[i].activeSelf) {
                objList[i].SetActive(true);
                return objList[i];
            }
        }

        // 沒有的話就建立一個子彈物件
        GameObject bulletGO = Instantiate(_bullet, transform);
        bulletGO.name = _bullet.name;
        objList.Add(bulletGO);
        return bulletGO;
    }

    // 回收物件
    public void Push(GameObject obj) {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
    }
}
