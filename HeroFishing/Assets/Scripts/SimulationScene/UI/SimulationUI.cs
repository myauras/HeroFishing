using HeroFishing.Battlefield;
using HeroFishing.Main;
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
            var attackData = new AttackData {
                AttackerPos = GameObject.Find("Role").transform.position,
                TargetPos = GameObject.Find("Target").transform.position,
                Direction = (GameObject.Find("Target").transform.position - GameObject.Find("Role").transform.position).normalized,
                BulletSpeed = 20f,
            };

            EntityManager _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery query = _entityManager.CreateEntityQuery(typeof(BattlefieldSettingSingleton));
            var battlefieldSetting = query.GetSingleton<BattlefieldSettingSingleton>();
            battlefieldSetting.MyAttackState = BattlefieldSettingSingleton.AttackState.Attacking;
            battlefieldSetting.MyAttackData = attackData;
            _entityManager.SetComponentData(query.GetSingletonEntity(), battlefieldSetting);
        }
        public void OnSpellClick(string _name) {
            SimulationSceneManager.Instance.MyHero.SetAniTrigger(_name);
        }
    }

}