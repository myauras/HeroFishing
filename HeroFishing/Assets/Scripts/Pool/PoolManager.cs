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

    // Addressable Assets�Ϊ������
    public void Pop(string key, Vector3 position = default, Quaternion rotaiton = default, Transform parent = null, Action<GameObject> popCallback = null) {
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

    // �l�u�Ϊ������
    public GameObject PopBullet() {
        // �p�G�����l�u���A�إߡC
        if (!_pools.TryGetValue(POOL_BULLET, out var objList)) {
            objList = new List<GameObject>();
            _pools[POOL_BULLET] = objList;
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
        obj.SetActive(false);
        obj.transform.SetParent(transform);
    }
}
