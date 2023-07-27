using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Scoz.Func;

namespace HeroFishing.Battle {
    public class ControlPanel : MonoBehaviour {
        Camera Cam;

        private void Start() {
            Cam = Camera.main;
        }
        public void OnAttackBtnClick() {
            SimulationSceneManager.Instance.MyHero.PlayAttackMotion();
            SetAttack();
        }
        void SetAttack() {
            var hero = BattleManager.Instance.GetHero(0);
            var pos = GetTargetPos();

            var attackData = new AttackData {
                AttackerPos = hero.transform.position,
                TargetPos = pos,
                Direction = (pos - hero.transform.position).normalized,
            };

            EntityManager _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery query = _entityManager.CreateEntityQuery(typeof(BattlefieldSettingSingleton));
            var battlefieldSetting = query.GetSingleton<BattlefieldSettingSingleton>();
            battlefieldSetting.MyAttackState = BattlefieldSettingSingleton.AttackState.Attacking;
            battlefieldSetting.MyAttackData = attackData;
            _entityManager.SetComponentData(query.GetSingletonEntity(), battlefieldSetting);
        }
        public void OnSpellClick(string _name) {
            BattleManager.Instance.GetHero(0).SetAniTrigger(_name);
        }
        private void Update() {
            if (Input.GetMouseButtonDown(0)) // 當按下滑鼠左鍵
            {
                var role = BattleManager.Instance.GetHero(0);
                var pos = GetTargetPos();
                role.PlayAttackMotion();
                var dir = pos - role.transform.position;
                dir.y = 0;
                Quaternion qDir = Quaternion.LookRotation(dir);
                role.FaceDir(qDir);
                SetAttack();
            }
        }
        Vector3 GetTargetPos() {
            Vector3 worldPoint = UIPosition.GetMouseWorldPointOnYZero(-1);
            return worldPoint;
        }
    }
}