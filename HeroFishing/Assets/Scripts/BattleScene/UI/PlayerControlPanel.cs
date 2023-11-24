using UnityEngine;
using Unity.Entities;
using Scoz.Func;
using UnityEngine.EventSystems;
using HeroFishing.Main;
using Unity.Mathematics;
using HeroFishing.Socket;

namespace HeroFishing.Battle {
    public class PlayerControlPanel : MonoBehaviour {
        [SerializeField]
        bool LockAttack = false;

        bool IsSkillMode = false;
        //Vector3 OriginPos;
        HeroSpellJsonData TmpSpellData;
        Hero TmpHero;
        Vector3 TmpSpellDir;
        Vector3 TmpSpellPos;
        Vector3 OriginPos;

        private const float MOVE_SCALE_FACTOR = 2;

        private void Update() {
            AttackDetect();
        }
        //普通攻擊
        void AttackDetect() {
            if (!Input.GetMouseButtonDown(0)) return;
            if (IsSkillMode) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (BattleManager.Instance == null) return;
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
            OriginPos = UIPosition.GetMouseWorldPointOnYZero(0);//設定初始按下位置
            //    TmpHero.transform.position;
            SpellIndicator.Instance.ShowIndicator(TmpSpellData);
            IsSkillMode = true;
        }

        //施放技能-拖曳
        public void OnDrag() {
            if (!IsSkillMode) return;
            var mousePos = UIPosition.GetMouseWorldPointOnYZero(0);
            TmpSpellPos = (mousePos - OriginPos) * MOVE_SCALE_FACTOR + TmpHero.transform.position;
            TmpSpellDir = (mousePos - OriginPos).normalized;
            TmpHero.FaceDir(Quaternion.LookRotation(TmpSpellDir));

            if (TmpSpellData.MyDragType == HeroSpellJsonData.DragType.Rot) {
                float angle = Mathf.Atan2(TmpSpellDir.x, TmpSpellDir.z) * Mathf.Rad2Deg;
                SpellIndicator.Instance.RotateIndicator(Quaternion.Euler(0, angle, 0));
            } else {
                SpellIndicator.Instance.MoveIndicator(TmpSpellPos);
            }
        }
        //施放技能-放開
        public void OnPointerUp() {
            if (!IsSkillMode) return;
            IsSkillMode = false;
            SpellIndicator.Instance.Hide();
            // 回到原位，否則旋轉的Indicator會有錯誤
            SpellIndicator.Instance.MoveIndicator(TmpHero.transform.position);

            var position = TmpSpellData.MyDragType == HeroSpellJsonData.DragType.Rot ? TmpHero.transform.position : TmpSpellPos;

            //設定技能
            OnSetSpell(position, TmpSpellDir);
        }
        public void OnSetSpell(Vector3 _attackerPos, Vector3 _attackDir) {
            //播放腳色動作(targetPos - TmpHero.transform.position).normalized
            TmpHero.PlaySpellMotion(TmpSpellData.SpellName);
            TmpHero.FaceDir(Quaternion.LookRotation(_attackDir));
            //設定ECS施法資料
            SetECSSpellData(_attackerPos, _attackDir);

            if (TmpSpellData.SpellName == SpellName.spell3) {
                CamManager.ShakeCam(CamManager.CamNames.Battle,
                    GameSettingJsonData.GetFloat(GameSetting.CamShake_Spell3_AmplitudeGain),
                    GameSettingJsonData.GetFloat(GameSetting.CamShake_Spell3_FrequencyGain),
                    GameSettingJsonData.GetFloat(GameSetting.CamShake_Sepll3_Duration));
            }
        }

        void SetECSSpellData(Vector3 _attackPos, Vector3 _attackDir) {
            if (TmpSpellData.Spell == null) return;
            var spell = TmpSpellData.Spell;
            spell.Play(_attackPos, TmpHero.transform.position, _attackDir);
            if(spell.Move != null) {
                var moveBehaviour = TmpHero.GetComponent<HeroMoveBehaviour>();
                if (moveBehaviour == null)
                    moveBehaviour = TmpHero.gameObject.AddComponent<HeroMoveBehaviour>();
                spell.Move.Play(_attackPos, TmpHero.transform.position, _attackDir, moveBehaviour);
            }
            //switch (TmpSpellData.MySpellType) {
            //    case HeroSpellJsonData.SpellType.SpreadLineShot:
            //        radius = float.Parse(TmpSpellData.SpellTypeValues[1]);
            //        speed = float.Parse(TmpSpellData.SpellTypeValues[2]);
            //        lifeTime = float.Parse(TmpSpellData.SpellTypeValues[3]);
            //        float intervalAngle = float.Parse(TmpSpellData.SpellTypeValues[3]);//射散間隔角度
            //        int spreadLineCount = int.Parse(TmpSpellData.SpellTypeValues[4]);//射散數量
            //        float startAngle = -intervalAngle * (spreadLineCount - 1) / 2.0f;//設定第一個指標的角度
            //        for (int i = 0; i < spreadLineCount; i++) {
            //            float curAngle = startAngle + intervalAngle * i;
            //            Quaternion rotateQ = Quaternion.Euler(new Vector3(0, curAngle, 0));//旋轉X度的四元數
            //            entity = _entityManager.CreateEntity();
            //            _entityManager.AddComponentData(entity, new SpellData() {
            //                PlayerID = 1,
            //                StrIndex_SpellID = strIndex_SpellID,
            //                SpellPrefabID = TmpSpellData.PrefabID,
            //                InitPosition = _attackPos,
            //                InitRotation = quaternion.LookRotationSafe(rotateQ * _attackDir, math.up()),//使用四元數來旋轉本來的攻擊向量
            //                Speed = speed,
            //                Radius = radius,
            //                LifeTime = lifeTime,
            //                DestoryOnCollision = TmpSpellData.DestroyOnCollision,
            //                Waves = TmpSpellData.Waves
            //            });
            //        }

            //        break;

            //}

        }

        public void OnLeaveBtnClick() {
            GameConnector.Instance.LeaveRoom();
            PopupUI.CallSceneTransition(MyScene.LobbyScene);
        }

    }
}