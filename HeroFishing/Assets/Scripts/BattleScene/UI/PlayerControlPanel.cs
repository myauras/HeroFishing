using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Scoz.Func;
using UnityEngine.EventSystems;
using System.Data;

public enum SpellName {
    attack,
    spell1,
    spell2,
    spell3,
}
namespace HeroFishing.Battle {
    public class PlayerControlPanel : MonoBehaviour {
        [SerializeField] Transform Indicator;

        bool IsSkillMode = false;
        SpellName CurSpellType;
        Vector3 origin;


        private void Start() {
            Indicator.gameObject.SetActive(false);
        }
        private void Update() {
            if (!Input.GetMouseButtonDown(0)) return;
            if (IsSkillMode) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;
            var role = BattleManager.Instance.GetHero(0);
            if (role == null) {
                WriteLog.LogError("腳色不存在");
                return;
            }
            var pos = UIPosition.GetMouseWorldPointOnYZero(0);
            role.PlaySpellMotion(SpellName.attack);
            var dir = pos - role.transform.position;
            dir.y = 0;
            Quaternion qDir = Quaternion.LookRotation(dir);
            role.FaceDir(qDir);
            SetAttack();

        }
        public void OnPointerDown(string _spell) {
            SpellName spellType;
            if (!MyEnum.TryParseEnum(_spell, out spellType)) return;
            CurSpellType = spellType;
            IsSkillMode = true;
            origin = UIPosition.GetMouseWorldPointOnYZero(0);
            Indicator.gameObject.SetActive(true);
        }

        public void OnDrag() {
            if (!IsSkillMode) return;
            Vector3 direction = UIPosition.GetMouseWorldPointOnYZero(0) - origin;
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            Indicator.rotation = Quaternion.Euler(0, angle, 0);
        }

        public void OnPointerUp() {
            if (!IsSkillMode) return;
            IsSkillMode = false;
            Vector2 direction = UIPosition.GetMouseWorldPointOnYZero(0) - origin;
            Spell(CurSpellType, direction.normalized);
            Indicator.gameObject.SetActive(false);
        }

        void Spell(SpellName _spell, Vector2 _dir) {
            var role = BattleManager.Instance.GetHero(0);
            if (role == null) {
                WriteLog.LogError("腳色不存在");
                return;
            }
            role.PlaySpellMotion(_spell);
        }

        void SetAttack() {
            var hero = BattleManager.Instance.GetHero(0);
            var pos = UIPosition.GetMouseWorldPointOnYZero(0);

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


    }
}