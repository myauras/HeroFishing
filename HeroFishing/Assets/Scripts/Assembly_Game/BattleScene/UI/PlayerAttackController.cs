using UnityEngine;
using Unity.Entities;
using Scoz.Func;
using UnityEngine.EventSystems;
using HeroFishing.Main;
using Unity.Mathematics;
using HeroFishing.Socket;
using HeroFishing.Socket.Matchgame;

namespace HeroFishing.Battle {
    public class PlayerAttackController : MonoBehaviour {
        [SerializeField]
        private bool _lockAttack = false;

        private bool _isSkillMode = false;
        //Vector3 OriginPos;
        private HeroSpellJsonData _spellData;
        private Hero _hero;
        private Vector3 _spellDir;
        private Vector3 _spellPos;
        private Vector3 _originPos;
        private HeroMoveBehaviour _currentMove;
        private int _attackID = 0;
        private float _scheduledNextAttackTime;
        private float _scheduledRecoverTime;
        private float _scheduledLockTime;
        private bool _isAttack;
        private Monster _targetMonster;
        public bool CanAttack {
            get {
                if (_lockAttack && _targetMonster != null)
                    return _targetMonster.IsAlive && _targetMonster.InField;
                return _isAttack;
            }
        }

        private const float MOVE_SCALE_FACTOR = 2;
        private const float ATTACK_BUFFER_TIME = 0.2f;
        private const float ATTACK_LOCK_TIME = 1.5f;
        public bool ControlLock {
            get {
                return _currentMove != null && _currentMove.IsMoving;
            }
        }

        public bool IsSpellTest => BattleManager.Instance.IsSpellTest;

        private void Update() {
            AttackInput();
            AttackLock();
            Attack();
            AttackRecover();
        }

        //普通攻擊
        //加入攻擊buffer，讓狂點的時候能維持最高的速度點擊
        //簡單說就是將攻擊指令存0.2秒，如果0.2秒內可以再次發射，會立刻發射。
        //比較不會導致狂點，但是射出時間有落差造成的卡頓感。
        private void AttackInput() {
            if (!Input.GetMouseButtonDown(0)) return;
            if (ControlLock) return;
            if (_isSkillMode) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (!CheckSpell(SpellName.attack)) return;
            if (!IsSpellTest && _hero.TotalPoints < Hero.Bet) {
                PopupUI.ShowClickCancel("你的金幣不足", null);
                return;
            }

            _isAttack = true;
            _scheduledRecoverTime = Time.time + ATTACK_BUFFER_TIME;

            // 如果在鎖定狀態，解除鎖定狀態
            if (_lockAttack) {
                _lockAttack = false;
                _targetMonster.Lock(false);
                _targetMonster = null;
            }
        }

        private void AttackLock() {
            if (ControlLock) return;
            if (_isSkillMode) return;
            if (EventSystem.current.IsPointerOverGameObject()) return;

            // 如果怪物死亡或不存在，解除鎖定狀態
            if (_targetMonster == null || !_targetMonster.IsAlive || !_targetMonster.InField) {
                _lockAttack = false;
                _targetMonster = null;
            }

            // 按下的時候檢查是否有抓到怪物，如果有的話就開始計時
            if (Input.GetMouseButtonDown(0)) {
                if (BattleManager.Instance == null) return;
                var ray = BattleManager.Instance.BattleCam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hitInfo, 100, LayerMask.GetMask("Monster"), QueryTriggerInteraction.Ignore)) {
                    var monster = hitInfo.collider.GetComponentInParent<Monster>();
                    if (monster.IsAlive) {
                        _targetMonster = monster;
                    }
                    _scheduledLockTime = Time.time + ATTACK_LOCK_TIME;
                }
            }

            // 確認有按到怪物，判斷是否真的進入鎖定狀態
            if (_targetMonster != null) {
                // 時間內放開，將怪物清空
                if (Input.GetMouseButtonUp(0) && Time.time <= _scheduledLockTime)
                    _targetMonster = null;
                // 超過指定時間，真的鎖定
                else if (!_lockAttack && Time.time > _scheduledLockTime) {
                    _lockAttack = true;
                    _targetMonster.Lock(true);
                }
            }
        }

        private void Attack() {
            if (!CanAttack) return;
            if (Time.time < _scheduledNextAttackTime) return;
            _hero.UsePoints();

            _isAttack = false;
            _scheduledNextAttackTime = Time.time + _spellData.CD;
            //攻擊方向
            var pos = _lockAttack && _targetMonster != null ? _targetMonster.transform.position : UIPosition.GetMouseWorldPointOnYZero(0);
            var dir = (pos - _hero.transform.position).normalized;
            //設定技能
            OnSetSpell(pos, dir);
        }

        private void AttackRecover() {
            if (!CanAttack) return;
            if (Time.time < _scheduledRecoverTime) return;
            _isAttack = false;
        }

        //施放技能-按下
        public void OnPointerDown(SpellName spellName) {
            if (ControlLock) return;
            if (!CheckSpell(spellName)) return;
            //if (!IsSpellTest && !BaseUI.GetInstance<SpellUI>().CanUse(spellName)) return;

            _originPos = UIPosition.GetMouseWorldPointOnYZero(0);//設定初始按下位置
            //    TmpHero.transform.position;
            SpellIndicator.Instance.ShowIndicator(_spellData);
            SpellIndicator.Instance.MoveIndicator(_hero.transform.position);
            _isSkillMode = true;
        }

        //施放技能-拖曳
        public void OnDrag() {
            if (ControlLock) return;
            if (!_isSkillMode) return;
            var mousePos = UIPosition.GetMouseWorldPointOnYZero(0);
            _spellPos = (mousePos - _originPos) * MOVE_SCALE_FACTOR + _hero.transform.position;
            _spellDir = (mousePos - _originPos).normalized;
            if (_spellDir != Vector3.zero)
                _hero.FaceDir(Quaternion.LookRotation(_spellDir));

            if (_spellData.MyDragType == HeroSpellJsonData.DragType.Rot) {
                float angle = Mathf.Atan2(_spellDir.x, _spellDir.z) * Mathf.Rad2Deg;
                SpellIndicator.Instance.RotateIndicator(Quaternion.Euler(0, angle, 0));
            }
            else {
                SpellIndicator.Instance.MoveIndicator(_spellPos);
            }
        }
        //施放技能-放開
        public void OnPointerUp(bool cancel = false) {
            if (ControlLock) return;
            if (!_isSkillMode) return;
            _isSkillMode = false;
            SpellIndicator.Instance.Hide();
            // 回到原位，否則旋轉的Indicator會有錯誤
            SpellIndicator.Instance.MoveIndicator(_hero.transform.position);

            cancel |= _spellDir == Vector3.zero;

            if (!cancel) {
                var position = _spellData.MyDragType == HeroSpellJsonData.DragType.Rot ? _hero.transform.position : _spellPos;
                //設定技能
                OnSetSpell(position, _spellDir);
            }
            _spellPos = Vector3.zero;
            _spellDir = Vector3.zero;
        }

        public void OnSetSpell(Vector3 _attackerPos, Vector3 _attackDir) {
            //播放腳色動作(targetPos - TmpHero.transform.position).normalized
            _hero.PlaySpell(_spellData.SpellName);
            _hero.FaceDir(Quaternion.LookRotation(_attackDir));
            //設定ECS施法資料
            SetECSSpellData(_attackerPos, _attackDir);
        }

        private bool CheckSpell(SpellName spellName) {
            if (BattleManager.Instance == null) return false;
            var hero = BattleManager.Instance.GetHero(0);
            if (hero == null) { WriteLog.LogError("玩家英雄不存在"); return false; }
            _hero = hero;
            var spellData = HeroSpellJsonData.GetSpell(_hero.MyData.ID, spellName);
            if (spellData == null) { WriteLog.LogErrorFormat("玩家英雄的 {0} 不存在", spellName); return false; }
            _spellData = spellData;
            return true;
        }

        private void SetECSSpellData(Vector3 _attackPos, Vector3 _attackDir) {
            if (_spellData.Spell == null) return;
            var spell = _spellData.Spell;
            var monsterIdx = _targetMonster != null && _lockAttack ? _targetMonster.MonsterIdx : -1;
            spell.Play(new SpellPlayData {
                lockAttack = _lockAttack,
                heroIndex = 0,
                monsterIdx = monsterIdx,
                attackID = _attackID,
                attackPos = _attackPos,
                heroPos = _hero.transform.position,
                direction = _attackDir
            });


            if (spell.Move != null) {
                if (_currentMove == null)
                    _currentMove = _hero.GetComponent<HeroMoveBehaviour>();
                if (_currentMove == null)
                    _currentMove = _hero.gameObject.AddComponent<HeroMoveBehaviour>();
                spell.Move.Play(_attackPos, _hero.transform.position, _attackDir, _currentMove);
            }

            if (spell.ShakeCamera != null)
                spell.ShakeCamera.Play();
            if (GameConnector.Connected)
                GameConnector.Instance.Attack(_attackID, _spellData.ID, monsterIdx, _lockAttack, _attackPos, _attackDir);
            _attackID++;
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