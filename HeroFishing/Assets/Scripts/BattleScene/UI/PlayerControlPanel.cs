using UnityEngine;
using Unity.Entities;
using Scoz.Func;
using UnityEngine.EventSystems;
using HeroFishing.Main;

namespace HeroFishing.Battle {
    public class PlayerControlPanel : MonoBehaviour {


        bool IsSkillMode = false;
        Vector3 OriginPos;
        HeroSpellData TmpSpellData;
        Hero TmpHero;
        Vector3 TmpSpellDir;

        private void Update() {
            AttackDetect();
        }
        //普通攻擊
        void AttackDetect() {
            if (!Input.GetMouseButtonDown(0)) return;
            if (IsSkillMode) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;
            var hero = BattleManager.Instance.GetHero(0);
            if (hero == null) { WriteLog.LogError("玩家英雄不存在"); return; }
            TmpHero = hero;
            var spellData = HeroSpellData.GetSpell(TmpHero.MyData.ID, SpellName.attack);
            if (spellData == null) { WriteLog.LogErrorFormat("玩家英雄的 {0} 不存在", SpellName.attack); return; }
            TmpSpellData = spellData;

            //攻擊方向
            var pos = UIPosition.GetMouseWorldPointOnYZero(0);
            var dir = (pos - hero.transform.position).normalized;
            //設定技能
            OnSetSpell(TmpHero.transform.position, dir);
        }


        //施放技能-按下
        public void OnPointerDown(string _spellNameStr) {
            SpellName spellName;
            if (!MyEnum.TryParseEnum(_spellNameStr, out spellName)) return;
            var hero = BattleManager.Instance.GetHero(0);
            if (hero == null) { WriteLog.LogError("玩家英雄不存在"); return; }
            TmpHero = hero;
            var spellData = HeroSpellData.GetSpell(TmpHero.MyData.ID, spellName);
            if (spellData == null) { WriteLog.LogErrorFormat("玩家英雄的 {0} 不存在", spellName); return; }
            TmpSpellData = spellData;
            OriginPos = UIPosition.GetMouseWorldPointOnYZero(0);//設定初始按下位置
            SpellIndicator.Instance.ShowIndicator(TmpSpellData);
            IsSkillMode = true;
        }

        //施放技能-拖曳
        public void OnDrag() {
            if (!IsSkillMode) return;
            TmpSpellDir = (UIPosition.GetMouseWorldPointOnYZero(0) - OriginPos).normalized;
            float angle = Mathf.Atan2(TmpSpellDir.x, TmpSpellDir.z) * Mathf.Rad2Deg;
            SpellIndicator.Instance.RotateLineIndicator(Quaternion.Euler(0, angle, 0));
        }
        //施放技能-放開
        public void OnPointerUp() {
            if (!IsSkillMode) return;
            IsSkillMode = false;
            SpellIndicator.Instance.Hide();
            //設定技能
            OnSetSpell(TmpHero.transform.position, TmpSpellDir);
        }
        public void OnSetSpell(Vector3 _attackerPos, Vector3 _attackDir) {
            //播放腳色動作(targetPos - TmpHero.transform.position).normalized
            TmpHero.PlaySpellMotion(TmpSpellData.SpellName);
            TmpHero.FaceDir(Quaternion.LookRotation(_attackDir));
            //設定ECS施法資料
            SetECSSpellData(_attackerPos, _attackDir);
        }

        void SetECSSpellData(Vector3 _attackPos, Vector3 _attackDir) {
            float radius;
            float speed;
            float lifeTime;
            //在ECS世界中建立一個施法
            EntityManager _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            switch (TmpSpellData.MySpellType) {
                case HeroSpellData.SpellType.LineShot:
                    var entity = _entityManager.CreateEntity();
                    radius = float.Parse(TmpSpellData.SpellTypeValues[1]);
                    speed = float.Parse(TmpSpellData.SpellTypeValues[2]);
                    lifeTime = float.Parse(TmpSpellData.SpellTypeValues[3]);
                    _entityManager.AddComponentData(entity, new SpellCom() {
                        PlayerID = 1,
                        BulletPrefabID = TmpSpellData.PrefabID,
                        AttackerPos = _attackPos,
                        Direction = _attackDir,
                        Speed = speed,
                        Radius = radius,
                        LifeTime = lifeTime,
                    });
                    break;

            }





        }



    }
}