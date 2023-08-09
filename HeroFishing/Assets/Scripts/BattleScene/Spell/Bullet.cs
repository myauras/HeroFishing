using DG.Tweening;
using HeroFishing.Main;
using Scoz.Func;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HeroFishing.Battle {
    public class Bullet : MonoBehaviour {

        public void SetData(int _spellPrefabID, Action _ac) {
            LoadModel(_spellPrefabID, _ac);
        }
        void LoadModel(int _spellPrefabID, Action _ac) {
            string path = string.Format("Bullet/BulletProjectile{0}.prefab", _spellPrefabID);
            AddressablesLoader.GetPrefabResourceByPath<GameObject>(path, (prefab, handle) => {
                var go = Instantiate(prefab, transform);
                go.transform.localPosition = prefab.transform.localPosition;
                go.transform.localRotation = prefab.transform.localRotation;
                go.transform.localScale = prefab.transform.localScale;
                Addressables.Release(handle);
                LoadDone();
                _ac?.Invoke();
            });
        }


        public virtual void Start() {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 模型都載入完才呼叫並顯示物件
        /// </summary>
        protected virtual void LoadDone() {
            gameObject.SetActive(true);
        }

    }

}