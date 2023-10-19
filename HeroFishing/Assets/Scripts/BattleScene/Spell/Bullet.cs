using DG.Tweening;
using HeroFishing.Main;
using HeroFishing.Socket;
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
        }
        public void HitParticleEffect() {
            //載入Hit模型
            var bulletPos = transform.position - new Vector3(0, GameSettingJsonData.GetFloat(GameSetting.Bullet_PositionY), 0);
            var bulletRot = transform.rotation;
            string hitPath = string.Format("Bullet/BulletHit{0}", SpellPrefabID);
            GameObjSpawner.SpawnParticleObjByPath(hitPath, bulletPos, bulletRot, null, (go, handle) => {
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
            string firePath = string.Format("Bullet/BulletFire{0}", SpellPrefabID);
            GameObjSpawner.SpawnParticleObjByPath(firePath, bulletPos, bulletRot, null, (go, handle) => {
                AddressableManage.SetToChangeSceneRelease(handle);//切場景再釋放資源
                go.SetActive(false);
                DOVirtual.DelayedCall(0.05f, () => {
                    go.SetActive(true);
                });
            });
            ////載入Projectile模型
            string projectilePath = string.Format("Bullet/BulletProjectile{0}", SpellPrefabID);
            GameObjSpawner.SpawnParticleObjByPath(projectilePath, Vector3.zero, Quaternion.identity, transform, (go, handle) => {
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