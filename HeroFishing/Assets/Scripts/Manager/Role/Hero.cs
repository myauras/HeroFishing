using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HeroFishing.Battle {
    public class Hero : Role {
        string[] IdleMotionStrs = new string[] { "idle1", "idle2", "idle3" };
        string[] AttackMotionStrs = new string[] { "attack1", "attack2" };
        public void PlayIdleMotion() {
            string rndMotion = Prob.GetRandomTFromTArray(IdleMotionStrs);
            SetAniTrigger(rndMotion);
        }
        public void PlayAttackMotion() {
            string rndMotion = Prob.GetRandomTFromTArray(AttackMotionStrs);
            SetAniTrigger(rndMotion);
        }
    }
}