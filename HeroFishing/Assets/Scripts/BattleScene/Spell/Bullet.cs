using DG.Tweening;
using HeroFishing.Main;
using HeroFishing.Socket;
using Scoz.Func;
using System;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

namespace HeroFishing.Battle {
    public struct BulletInit {
        public Vector3 FirePosition;
        public int PrefabID;
        public int SubPrefabID;
        public bool IgnoreFireModel;
        public float Delay;
    }
    public class Bullet : MonoBehaviour {

        int SpellPrefabID;
        int SubSpellPrefabID;
        Vector3 FirePosition;
        float Delay;

        public bool IsLoaded { get; private set; }

        private void OnDestroy() {
        }

        //public void HitParticleEffect() {
        //    //載入Hit模型
        //    var bulletPos = transform.position - new Vector3(0, GameSettingJsonData.GetFloat(GameSetting.Bullet_PositionY), 0);
        //    var bulletRot = transform.rotation;
        //    string hitPath = string.Format("Bullet/BulletHit{0}", SpellPrefabID);
        //    GameObjSpawner.SpawnParticleObjByPath(hitPath, bulletPos, bulletRot, null, (go, handle) => {
        //        AddressableManage.SetToChangeSceneRelease(handle);//切場景再釋放資源
        //    });
        //}
        public void Create(BulletInit bulletInit) {
            SpellPrefabID = bulletInit.PrefabID;
            SubSpellPrefabID = bulletInit.SubPrefabID;
            FirePosition = bulletInit.FirePosition;
            Delay = bulletInit.Delay;
            gameObject.SetActive(false);
            if (!bulletInit.IgnoreFireModel)
                LoadFireModel();
            if (Delay > 0) {
                DOVirtual.DelayedCall(Delay, LoadProjetileModel);
            }
            else
                LoadProjetileModel();
        }

        void LoadFireModel() {
            //載入Fire模型
            var bulletPos = FirePosition;
            var bulletRot = transform.rotation;
            string firePath = string.Format("Bullet/BulletFire{0}", SpellPrefabID);
            GameObjSpawner.SpawnParticleObjByPath(firePath, bulletPos, bulletRot, null, (go, handle) => {
                AddressableManage.SetToChangeSceneRelease(handle);//切場景再釋放資源
                go.SetActive(false);
                DOVirtual.DelayedCall(0.05f, () => {
                    go.SetActive(true);
                });
                DOVirtual.DelayedCall(3f, () => {
                    if (go != null)
                        Destroy(go);
                });
            });
        }

        void LoadProjetileModel() {
            string projectilePath;
            ////載入Projectile模型
            if (SubSpellPrefabID == 0)
                projectilePath = string.Format("Bullet/BulletProjectile{0}", SpellPrefabID);
            else
                projectilePath = string.Format("Bullet/BulletProjectile{0}_{1}", SpellPrefabID, SubSpellPrefabID);
            GameObjSpawner.SpawnParticleObjByPath(projectilePath, Vector3.zero, Quaternion.identity, transform, (go, handle) => {
                AddressableManage.SetToChangeSceneRelease(handle);//切場景再釋放資源
                LoadDone();
            });
            
        }

        /// <summary>
        /// 模型都載入完才呼叫並顯示物件
        /// </summary>
        protected virtual void LoadDone() {
            IsLoaded = true;
            gameObject.SetActive(true);
        }

    }

}