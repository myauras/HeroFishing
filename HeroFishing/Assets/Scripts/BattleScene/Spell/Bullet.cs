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

#if UNITY_EDITOR
    public struct BulletGizmoData {
        public Vector3 Position;
        public Vector3 Direction;
        public float Radius;
        public float Angle;
    }
#endif
    public class Bullet : MonoBehaviour {

        int SpellPrefabID;
        int SubSpellPrefabID;
        Vector3 FirePosition;
        float Delay;

#if UNITY_EDITOR
        BulletGizmoData gizmoData;
#endif

        public bool IsLoaded { get; private set; }
        public GameObject Projectile { get; private set; }

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
            var pool = PoolManager.Instance;
            var path = string.Format("Assets/AddressableAssets/Particles/{0}.prefab", firePath);
            pool.Pop(path, bulletPos, bulletRot, null, go => {
                // 讓AutoBackPool.cs自己控制返回物件池的時間
            });
        }

        void LoadProjetileModel() {
            string projectilePath;
            ////載入Projectile模型
            if (SubSpellPrefabID == 0)
                projectilePath = string.Format("Bullet/BulletProjectile{0}", SpellPrefabID);
            else
                projectilePath = string.Format("Bullet/BulletProjectile{0}_{1}", SpellPrefabID, SubSpellPrefabID);

            var pool = PoolManager.Instance;
            var path = string.Format("Assets/AddressableAssets/Particles/{0}.prefab", projectilePath);
            pool.Pop(path, Vector3.zero, Quaternion.identity, transform, go => {
                Projectile = go;
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

#if UNITY_EDITOR
        public void SetGizmoData(BulletGizmoData gizmoData) {
            this.gizmoData = gizmoData;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            UnityEditor.Handles.color = Color.yellow;
            if (gizmoData.Angle > 0 && gizmoData.Direction != Vector3.zero) {
                Vector3 fromDir = Quaternion.AngleAxis(-gizmoData.Angle / 2, Vector3.up) * gizmoData.Direction;
                gizmoData.Position.y += 0.1f;                
                UnityEditor.Handles.DrawWireArc(gizmoData.Position, Vector3.up, fromDir, gizmoData.Angle, gizmoData.Radius);
                Gizmos.DrawRay(gizmoData.Position, fromDir * gizmoData.Radius);
                Gizmos.DrawRay(gizmoData.Position, Quaternion.AngleAxis(gizmoData.Angle, Vector3.up) * fromDir * gizmoData.Radius);
            }
            else {
                Gizmos.DrawWireSphere(gizmoData.Position, gizmoData.Radius);
            }
        }
#endif
    }

}