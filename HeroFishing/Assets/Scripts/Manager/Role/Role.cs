using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HeroFishing.Battle {
    public abstract class Role : MonoBehaviour {
        [SerializeField]
        Animator MyAni;

        public virtual void Start() {
        }

        public void SetAniTrigger(string name) {
            MyAni.SetTrigger(name);
        }
        public void FaceDir(Quaternion _dir) {
            transform.rotation = _dir;
        }

    }
}