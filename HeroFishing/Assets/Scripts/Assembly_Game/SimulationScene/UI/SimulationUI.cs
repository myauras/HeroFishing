using HeroFishing.Battle;
using HeroFishing.Main;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
namespace HeroFishing.Battle {
    public class SimulationUI : MonoBehaviour {


        public void OnAttackBtnClick() {
            SimulationSceneManager.Instance.MyHero.PlaySpell(SpellName.attack);
            SetAttack();
        }
        void SetAttack() {


        }
        public void OnSpellClick(string _name) {
            SimulationSceneManager.Instance.MyHero.SetAniTrigger(_name);
        }
    }

}