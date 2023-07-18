using HeroFishing.Battlefield;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
namespace HeroFishing.Battlefield {
    public class SimulationUI : MonoBehaviour {
        public void OnAttackBtnClick() {
            SimulationSceneManager.Instance.MyHero.PlayAttackMotion();
            var attackData = new AttackData {
                AttackerPos = GameObject.Find("Role").transform.position,
                TargetPos = GameObject.Find("Target").transform.position
            };

        }
        public void OnSpellClick(string _name) {
            SimulationSceneManager.Instance.MyHero.SetAniTrigger(_name);
        }
    }

}