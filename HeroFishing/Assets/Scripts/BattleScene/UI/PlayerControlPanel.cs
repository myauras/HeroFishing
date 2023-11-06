using UnityEngine;
using Unity.Entities;
using Scoz.Func;
using UnityEngine.EventSystems;
using HeroFishing.Main;

namespace HeroFishing.Battle {
    public class PlayerControlPanel : MonoBehaviour {


        bool IsSkillMode = false;
        Vector3 OriginPos;
        HeroSpellJsonData TmpSpellData;
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
            var spellData = HeroSpellJsonData.GetSpell(TmpHero.MyData.ID, SpellName.attack);
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
            var spellData = HeroSpellJsonData.GetSpell(TmpHero.MyData.ID, spellName);
            if (spellData == null) { WriteLog.LogErrorFormat("玩家英雄的 {0} 不存在", spellName); return; }
            TmpSpellData = spellData;
            OriginPos = TmpHero.transform.position;
            if (TmpSpellData.MyUseType == HeroSpellJsonData.UseType.Mov)
                SpellIndicator.Instance.MoveIndicator(UIPosition.GetMouseWorldPointOnYZero(0));
            //OriginPos = UIPosition.GetMouseWorldPointOnYZero(0);//設定初始按下位置
            SpellIndicator.Instance.ShowIndicator(TmpSpellData);
            IsSkillMode = true;
        }

        //施放技能-拖曳
        public void OnDrag() {
            if (!IsSkillMode) return;
            if (TmpSpellData.MyUseType == HeroSpellJsonData.UseType.None) return;
            if (TmpSpellData.MyUseType == HeroSpellJsonData.UseType.Rot) {
                TmpSpellDir = (UIPosition.GetMouseWorldPointOnYZero(0) - OriginPos).normalized;
                float angle = Mathf.Atan2(TmpSpellDir.x, TmpSpellDir.z) * Mathf.Rad2Deg;
                SpellIndicator.Instance.RotateLineIndicator(Quaternion.Euler(0, angle, 0));
            }
            else {
                SpellIndicator.Instance.MoveIndicator(UIPosition.GetMouseWorldPointOnYZero(0));
            }
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
            _attackerPos += new Vector3(0, GameSettingJsonData.GetFloat(GameSetting.Bullet_PositionY), 0);//子彈高度固定調整
            SetECSSpellData(_attackerPos, _attackDir);
        }

        void SetECSSpellData(Vector3 _attackPos, Vector3 _attackDir) {
            //在ECS世界中建立一個施法
            switch (TmpSpellData.MySpellType) {
                case HeroSpellJsonData.SpellType.Area:
                    CreateAreaEntity(TmpSpellData, _attackPos, UIPosition.GetMouseWorldPointOnYZero(0));
                    break;
                //case HeroSpellJsonData.SpellType.LineShot:

                //    radius = float.Parse(TmpSpellData.SpellTypeValues[1]);
                //    speed = float.Parse(TmpSpellData.SpellTypeValues[2]);
                //    lifeTime = float.Parse(TmpSpellData.SpellTypeValues[3]);
                //    entity = _entityManager.CreateEntity();
                //    _entityManager.AddComponentData(entity, new SpellCom() {
                //        PlayerID = 1,
                //        StrIndex_SpellID = strIndex_SpellID,
                //        SpellPrefabID = TmpSpellData.PrefabID,
                //        AttackerPos = _attackPos,
                //        Direction = _attackDir,
                //        Speed = speed,
                //        Radius = radius,
                //        LifeTime = lifeTime,
                //    });
                //    break;
                case HeroSpellJsonData.SpellType.Bullet:

                    float intervalAngle = float.Parse(TmpSpellData.SpellTypeValues[6]);//射散間隔角度
                    int spreadLineCount = int.Parse(TmpSpellData.SpellTypeValues[7]);//射散數量

                    if(spreadLineCount < 2)
                    {
                        CreateBulletEntity(TmpSpellData, _attackPos, _attackDir);
                        return;
                    }

                    float startAngle = -intervalAngle * (spreadLineCount - 1) / 2.0f;//設定第一個指標的角度
                    for (int i = 0; i < spreadLineCount; i++) {
                        float curAngle = startAngle + intervalAngle * i;
                        Quaternion rotateQ = Quaternion.Euler(new Vector3(0, curAngle, 0));//旋轉X度的四元數
                        CreateBulletEntity(TmpSpellData, _attackPos, rotateQ * _attackDir);
                    }

                    break;

            }

        }

        void CreateBulletEntity(HeroSpellJsonData data, Vector3 attackerPos, Vector3 direction)
        {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            Entity entity = entityManager.CreateEntity();
            var strIndex_SpellID = ECSStrManager.AddStr(TmpSpellData.ID);

            bool isPiercing = bool.Parse(TmpSpellData.SpellTypeValues[0]);
            int maxPiercingCount = int.Parse(TmpSpellData.SpellTypeValues[1]);
            float radius = float.Parse(TmpSpellData.SpellTypeValues[3]);
            float speed = float.Parse(TmpSpellData.SpellTypeValues[4]);
            float lifeTime = float.Parse(TmpSpellData.SpellTypeValues[5]);

            entityManager.AddComponentData(entity, new BulletCom()
            {
                PlayerID = 1,
                StrIndex_SpellID = strIndex_SpellID,
                SpellPrefabID = data.PrefabID,
                AttackerPos = attackerPos,
                Direction = direction,
                Speed = speed,
                Radius = radius,
                LifeTime = lifeTime,
                Piercing = isPiercing,
                MaxPiercingCount = maxPiercingCount
            });
        }

        void CreateAreaEntity(HeroSpellJsonData data, Vector3 attackerPos, Vector3 position) {
            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            Entity entity = entityManager.CreateEntity();
            var strIndex_SpellID = ECSStrManager.AddStr(TmpSpellData.ID);

            var areaValues = new Vector2(float.Parse(data.SpellTypeValues[1]), float.Parse(data.SpellTypeValues[2]));
            var shapeType = MyEnum.ParseEnum<AreaValue.ShapeType>(data.SpellTypeValues[0]);
            var lifeTime = float.Parse(data.SpellTypeValues[4]);
            var waveCount = int.Parse(data.SpellTypeValues[5]);

            entityManager.AddComponentData(entity, new AreaCom() {
                PlayerID = 1,
                StrIndex_SpellID = strIndex_SpellID,
                SpellPrefabID = data.PrefabID,
                AttackerPos = attackerPos,
                AreaPos = position,
                AreaValues = areaValues,
                ShapeType = shapeType,
                LifeTime = lifeTime,
                WaveCount = waveCount
            });
            Debug.Log("create");
        }
    }
}