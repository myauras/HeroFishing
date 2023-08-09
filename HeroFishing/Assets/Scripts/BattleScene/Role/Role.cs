using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using Unity.Transforms;
using UnityEngine;
namespace HeroFishing.Battle {
    public abstract class Role : MonoBehaviour {

        protected SkinnedMeshRenderer MySkinnedMeshRenderer;
        protected Material MySkinnedMaterial;
        protected Animator MyAni;


        public virtual void Start() {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 模型都載入完才呼叫並顯示物件
        /// </summary>
        protected virtual void LoadDone() {
            gameObject.SetActive(true);
        }

        protected virtual void SetModel() {
            MySkinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            MySkinnedMaterial = MySkinnedMeshRenderer.material;
            MyAni = MySkinnedMeshRenderer.transform.parent.GetComponent<Animator>();
        }

        public void SetAniTrigger(string _aniName) {
            if (MyAni == null) { WriteLog.LogError(name + "的MyAni為null"); return; }
            MyAni.SetTrigger(_aniName);
        }
        public void FaceDir(Quaternion _dir) {
            transform.rotation = _dir;
        }

    }
}