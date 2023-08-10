using DG.Tweening;
using HeroFishing.Main;
using Scoz.Func;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace HeroFishing.Battle {
    public class Bullet : MonoBehaviour {

        int SpellPrefabID;

        private void OnDestroy() {
            LoadHitModel();
        }
        void LoadHitModel() {
            //載入Hit模型
            var bulletPos = transform.position;
            var bulletRot = transform.rotation;
            string firePath = string.Format("Bullet/BulletHit{0}.prefab", SpellPrefabID);
            AddressablesLoader.GetPrefabResourceByPath<GameObject>(firePath, (prefab, handle) => {
                var go = Instantiate(prefab, null);
                go.transform.position = bulletPos;
                go.transform.rotation = bulletRot;
                go.transform.localScale = prefab.transform.localScale;
                AddressableManage.SetToChangeSceneRelease(handle);//切場景再釋放資源
            });
        }
        public void SetData(int _spellPrefabID) {
            SpellPrefabID = _spellPrefabID;
            LoadModel();
        }
        void LoadModel() {
            gameObject.SetActive(false);
            //載入Fire模型
            var bulletPos = transform.position;
            var bulletRot = transform.rotation;
            string firePath = string.Format("Bullet/BulletFire{0}.prefab", SpellPrefabID);
            AddressablesLoader.GetPrefabResourceByPath<GameObject>(firePath, (prefab, handle) => {
                var go = Instantiate(prefab, null);
                go.transform.position = bulletPos;
                go.transform.rotation = bulletRot;
                go.transform.localScale = prefab.transform.localScale;
                AddressableManage.SetToChangeSceneRelease(handle);//切場景再釋放資源
            });

            //載入Projectile模型
            string projectilePath = string.Format("Bullet/BulletProjectile{0}.prefab", SpellPrefabID);
            AddressablesLoader.GetPrefabResourceByPath<GameObject>(projectilePath, (prefab, handle) => {
                var go = Instantiate(prefab, transform);
                go.transform.localPosition = prefab.transform.localPosition;
                go.transform.localRotation = prefab.transform.localRotation;
                go.transform.localScale = prefab.transform.localScale;
                AddressableManage.SetToChangeSceneRelease(handle);//切場景再釋放資源
                LoadDone();
            });
        }

        /// <summary>
        /// 模型都載入完才呼叫並顯示物件
        /// </summary>
        protected virtual void LoadDone() {
            gameObject.SetActive(true);
        }

    }

}