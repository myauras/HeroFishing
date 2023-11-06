using DG.Tweening;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeroFishing.Battle {
    public class Area : MonoBehaviour {
        int SpellPrefabID;

        private void OnDestroy() {
        }
        public void HitParticleEffect() {
            //更Hit家
            var bulletPos = transform.position - new Vector3(0, GameSettingJsonData.GetFloat(GameSetting.Bullet_PositionY), 0);
            var bulletRot = transform.rotation;
            //string hitPath = string.Format("Bullet/AreaHit{0}", SpellPrefabID);
            //GameObjSpawner.SpawnParticleObjByPath(hitPath, bulletPos, bulletRot, null, (go, handle) => {
            //    AddressableManage.SetToChangeSceneRelease(handle);//ち初春睦戈方
            //});
        }
        public void SetData(int _spellPrefabID) {
            SpellPrefabID = _spellPrefabID;
            LoadModel();
        }
        void LoadModel() {
            gameObject.SetActive(false);

            //更Fire家
            var bulletPos = transform.position;
            var bulletRot = transform.rotation;
            string firePath = string.Format("Bullet/AreaFire{0}", SpellPrefabID);
            GameObjSpawner.SpawnParticleObjByPath(firePath, bulletPos, bulletRot, null, (go, handle) => {
                AddressableManage.SetToChangeSceneRelease(handle);//ち初春睦戈方
                go.SetActive(false);
                DOVirtual.DelayedCall(0.05f, () => {
                    go.SetActive(true);
                });
            });
            ////更Projectile家
            string projectilePath = string.Format("Bullet/Area{0}", SpellPrefabID);
            GameObjSpawner.SpawnParticleObjByPath(projectilePath, Vector3.zero, Quaternion.identity, transform, (go, handle) => {
                AddressableManage.SetToChangeSceneRelease(handle);//ち初春睦戈方
                LoadDone();
            });

        }

        /// <summary>
        /// 家常更Ч㊣陪ボン
        /// </summary>
        protected virtual void LoadDone() {
            gameObject.SetActive(true);
        }

    }
}
