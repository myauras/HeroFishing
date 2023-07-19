using HeroFishing.Battlefield;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
namespace HeroFishing.Battlefield {
    public class SimulationUI : MonoBehaviour {
        public void OnAttackBtnClick() {
            SimulationSceneManager.Instance.MyHero.PlayAttackMotion();
            SetAttack();
        }
        void SetAttack() {
            int bulletLayerValue = LayerMask.NameToLayer("Bullet");
            uint bulletLayer = 1u << bulletLayerValue;
            int targetLayerValue = LayerMask.NameToLayer("Target");
            uint targetLayer = 1u << targetLayerValue;
            var attackData = new AttackData {
                AttackerPos = GameObject.Find("Role").transform.position,
                TargetPos = GameObject.Find("Target").transform.position,
                Direction = (GameObject.Find("Target").transform.position - GameObject.Find("Role").transform.position).normalized,
                BulletSpeed = 20f,
                BulletLayer = bulletLayer,
                TargetLayer = targetLayer,
            };
            AttackTrigger.Attack(attackData);
        }
        public void OnSpellClick(string _name) {
            SimulationSceneManager.Instance.MyHero.SetAniTrigger(_name);
        }
    }

}