using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using Unity.Transforms;
using UnityEngine;
namespace HeroFishing.Battle {
    public abstract class Role : MonoBehaviour {

        protected SkinnedMeshRenderer[] MySkinnedMeshRenderers;
        //protected Material MySkinnedMaterial;
        protected Animator MyAni;
        protected MaterialPropertyBlock PropertyBlock;


        public virtual void Start() {
            gameObject.SetActive(false);
            PropertyBlock = new MaterialPropertyBlock();
        }

        /// <summary>
        /// 模型都載入完才呼叫並顯示物件
        /// </summary>
        protected virtual void LoadDone() {
            gameObject.SetActive(true);
        }

        protected virtual void SetModel() {
            MySkinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            MySkinnedMeshRenderers[0].GetPropertyBlock(PropertyBlock);
            MyAni = MySkinnedMeshRenderers[0].transform.parent.GetComponent<Animator>();
        }

        public void SetAniTrigger(string _aniName) {
            if (MyAni == null) { WriteLog.LogError(name + "的MyAni為null"); return; }
            MyAni.SetTrigger(_aniName);
        }
        public void FaceDir(Quaternion _dir) {
            transform.rotation = _dir;
        }

        protected void SetPropertyBlock(MaterialPropertyBlock propertyBlock) {
            foreach (var renderer in MySkinnedMeshRenderers) {
                renderer.SetPropertyBlock(propertyBlock);
            }
        }
    }
}