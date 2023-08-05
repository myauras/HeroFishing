using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Scoz.Func;
using UnityEngine.EventSystems;

namespace HeroFishing.Battle {
    public class ControlPanel : MonoBehaviour {
        bool IsSkillMode = false;
        public Transform Indicator;
        Vector3 origin;
        Camera SceneCam;

        private void Start() {
            SceneCam = GameObject.FindGameObjectWithTag("SceneCam").GetComponent<Camera>();
            Indicator.gameObject.SetActive(false);
        }

        public void OnPointerDown() {
            IsSkillMode = true;
            origin = GetTargetPos();
            Indicator.gameObject.SetActive(true);
        }

        public void OnDrag() {
            if (!IsSkillMode) return;
            Vector2 direction = GetTargetPos() - origin;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Indicator.rotation = Quaternion.Euler(0, 90 - angle, 0);
        }

        public void OnPointerUp() {
            if (!IsSkillMode) return;
            IsSkillMode = false;
            Vector2 direction = GetTargetPos() - origin;
            ShootSkill(direction.normalized);
            Indicator.gameObject.SetActive(false);
        }

        void ShootSkill(Vector2 direction) {
            // 技能射擊邏輯
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
            if (!Input.GetMouseButtonDown(0)) return;

            if (!IsSkillMode && !IsPointerOverUI()) {// 普通射擊邏輯
                var role = BattleManager.Instance.GetHero(0);
                if (role == null) WriteLog.LogError("腳色不存在");
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

        // 檢查當前的鼠標/觸碰是否在UI元素上
        bool IsPointerOverUI() {
            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}