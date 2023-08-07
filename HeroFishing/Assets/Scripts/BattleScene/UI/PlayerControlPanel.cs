using UnityEngine;
using Unity.Entities;
using Scoz.Func;
using UnityEngine.EventSystems;
using HeroFishing.Main;

namespace HeroFishing.Battle {
    public class PlayerControlPanel : MonoBehaviour {
        [SerializeField] Transform Indicator;

        bool IsSkillMode = false;
        HeroSpellData TmpSpellData;
        Vector3 OriginPos;


        private void Start() {
            Indicator.gameObject.SetActive(false);
        }
        private void Update() {
            AttackDetect();
        }
        void AttackDetect() {
            if (!Input.GetMouseButtonDown(0)) return;
            if (IsSkillMode) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;
            OnSetSpell(SpellName.attack);
        }
        public void OnSetSpell(SpellName _spellName) {
            var hero = BattleManager.Instance.GetHero(0);
            if (hero == null) {
                WriteLog.LogError("腳色不存在");
                return;
            }
            //播放腳色動作
            hero.PlaySpellMotion(_spellName);
            //腳色面向方向
            var pos = UIPosition.GetMouseWorldPointOnYZero(0);
            var dir = pos - hero.transform.position;
            Quaternion qDir = Quaternion.LookRotation(dir);
            hero.FaceDir(Quaternion.LookRotation(dir));
            //設定施法
            switch (_spellName) {
                case SpellName.attack:
                    SetECSAttackData(hero);
                    break;
                default:

                    SetECSSpellData(hero);
                    break;
            }
        }


        public void OnPointerDown(string _spell) {
            SpellName spellName;
            if (!MyEnum.TryParseEnum(_spell, out spellName)) return;
            var hero = BattleManager.Instance.GetHero(0);
            if (hero == null) { WriteLog.LogError("玩家英雄不存在"); return; }
            var spellData = HeroSpellData.GetSpell(hero.MyData.ID, spellName);
            if (spellData == null) { WriteLog.LogErrorFormat("玩家英雄的 {0} 不存在", spellName); return; }
            TmpSpellData = spellData;
            OriginPos = UIPosition.GetMouseWorldPointOnYZero(0);//設定初始按下位置
            Indicator.gameObject.SetActive(true);
            IsSkillMode = true;
        }

        public void OnDrag() {
            if (!IsSkillMode) return;
            Vector3 dir = UIPosition.GetMouseWorldPointOnYZero(0) - OriginPos;
            float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            Indicator.rotation = Quaternion.Euler(0, angle, 0);
        }

        public void OnPointerUp() {
            if (!IsSkillMode) return;
            IsSkillMode = false;
            OnSetSpell(TmpSpellData.SpellName);
            Indicator.gameObject.SetActive(false);
        }


        void SetECSAttackData(Hero _hero) {
            var pos = UIPosition.GetMouseWorldPointOnYZero(0);

            var attackData = new AttackData {
                AttackerPos = _hero.transform.position,
                TargetPos = pos,
                Direction = (pos - _hero.transform.position).normalized,
            };

            EntityManager _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            EntityQuery query = _entityManager.CreateEntityQuery(typeof(BattlefieldSettingSingleton));
            var battlefieldSetting = query.GetSingleton<BattlefieldSettingSingleton>();
            battlefieldSetting.MyAttackState = BattlefieldSettingSingleton.AttackState.Attacking;
            battlefieldSetting.MyAttackData = attackData;
            _entityManager.SetComponentData(query.GetSingletonEntity(), battlefieldSetting);
        }
        void SetECSSpellData(Hero _hero) {
            Vector2 dir = (UIPosition.GetMouseWorldPointOnYZero(0) - OriginPos).normalized;//取得方向向量
        }



    }
}