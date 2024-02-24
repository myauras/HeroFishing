using HeroFishing.Main;
using HeroFishing.Socket;
using HeroFishing.Socket.Matchgame;
using Scoz.Func;
using Service.Realms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
namespace HeroFishing.Battle {
    public class BattleManager : MonoBehaviour {
        public static BattleManager Instance;
        [SerializeField] Camera MyCam;
        [SerializeField] Hero[] MyHeros;
        [SerializeField] public Transform MonsterParent;
        [SerializeField]
        private SpellIndicator _spellIndicator;
        [SerializeField]
        private int _testHeroID;
        //[SerializeField]
        //private string _testHeroSkinID;
        [SerializeField]
        private bool _rotateTest;
        [SerializeField]
        private int _rotateTestIndex;
        [SerializeField, Min(1)]
        private int _testBet;
        [SerializeField]
        private bool _isSpellTest;
        [SerializeField]
        private float _localDieThreshold;
        public bool IsSpellTest => _isSpellTest;
        public float LocalDieThreshold => _localDieThreshold;

        private int _bet = 1;
        public int Bet => _bet;

        private int _playerCount;
        public int PlayerCount => _playerCount;

        public int Index {
            get {
                if (_rotateTest || !GameConnector.Connected) return _rotateTestIndex;
                return AllocatedRoom.Instance.Index;
            }
        }
        //private EntityManager _entityManager;

        public MonsterScheduler MyMonsterScheduler { get; private set; }
        public static float3 MonsterCollisionPosOffset { get; private set; }//因為怪物的位置是在地板 所以檢測碰撞半徑時以地板為圓心的話子彈會打不到 所以碰撞檢測時要將判定的圓心高度提高到子彈高度
        public Camera BattleCam => MyCam;

        private const int MAX_HERO_COUNT = 4;

        public Action<int, int> OnHeroAdd;
        public Action<int> OnHeroRemove;
        Action OnLeaveGameAC;

        public void Init() {
            Instance = this;
            PoolManager.Instance.ResetBattlePool();//清除物件池
            Monster.ResetMonsterStaticDatas();//清除場上怪物清單
            SetCam();//設定攝影機模式
            InitMonsterScheduler();
            InitPlayerHero();
            MonsterCollisionPosOffset = new float3(0, GameSettingJsonData.GetFloat(GameSetting.Bullet_PositionY), 0);
            //_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _spellIndicator.Init();
            DeviceManager.AddOnFocusAction(() => {
                if (GameConnector.Connected)
                    GameConnector.Instance.UpdateScene();
            });

            if (GameConnector.Connected) {
                UpdateHeros();
                GameConnector.Instance.UpdateScene();
            }
        }
        public void RegisterOnLeaveGameEvent(Action _ac) {
            if (_ac == null) return;
            OnLeaveGameAC += _ac;
        }
        void SetCam() {
            //因為戰鬥場景的攝影機有分為場景與UI, 要把場景攝影機設定為Base, UI設定為Overlay, 並在BaseCamera中加入Camera stack
            UICam.Instance.SetRendererMode(CameraRenderType.Overlay);
            AddCamStack(UICam.Instance.MyCam);
        }
        public void LeaveGame() {
            PoolManager.Instance.ResetBattlePool();//清除物件池
            Monster.ResetMonsterStaticDatas();//清除場上怪物清單
            OnLeaveGameAC?.Invoke();
            OnLeaveGameAC = null;
            AllocatedRoom.Instance.ClearRoom();
            GameConnector.Instance.LeaveRoom();
            PopupUI.InitSceneTransitionProgress(0);
            PopupUI.CallSceneTransition(MyScene.LobbyScene);
            UICam.Instance.SetRendererMode(CameraRenderType.Base);//把攝影機mode設定回base
        }
        /// <summary>
        /// 將指定camera加入到MyCam的CameraStack中
        /// </summary>
        void AddCamStack(Camera _cam) {
            if (_cam == null) return;
            var cameraData = MyCam.GetUniversalAdditionalCameraData();
            if (cameraData == null) return;
            cameraData.cameraStack.Add(_cam);
        }

        private void InitPlayerHero() {
            var hero = GetHero(0);
            if (!GameConnector.Connected) {//測試流程

                hero.SetData(_testHeroID, $"{_testHeroID}_1");
                hero.UpdatePoints(10);
                _bet = _testBet;
            } else {
                hero.SetData(AllocatedRoom.Instance.MyHeroID, AllocatedRoom.Instance.MyHeroSkinID);
                var map = RealmManager.MyRealm.Find<DBMap>(AllocatedRoom.Instance.DBMapID);
                _bet = map.Bet ?? 1;
                var player = GamePlayer.Instance.GetDBPlayerDoc<DBPlayer>(DBPlayerCol.player);
                if (player != null && player.Point.HasValue)
                    hero.UpdatePoints((int)player.Point.Value);
            }
        }

        private void InitMonsterScheduler() {
            MyMonsterScheduler = new MonsterScheduler();
            MyMonsterScheduler.Init(MapJsonData.GetData(7), !GameConnector.Connected);
        }

        private void Update() {
            SpawnMonster();
        }

        private void SpawnMonster() {
            if (MyMonsterScheduler == null) return;
            if (!MyMonsterScheduler.TryDequeueMonster(out var monsterInfo)) return;
            MonsterSpawner.Spawn(monsterInfo, out var monster);
            //if (!MyMonsterScheduler.TryDequeueMonster(out SpawnData spawnData)) return;

            //var entity = _entityManager.CreateEntity();
            //_entityManager.AddComponentData(entity, spawnData);
            //_entityManager.AddComponent<SpawnTag>(entity);

            //是BOSS就會攝影機震動
            //if (spawnData.IsBoss)
            CamManager.ShakeCam(CamManager.CamNames.Battle,
                GameSettingJsonData.GetFloat(GameSetting.CamShake_BossDebut_AmplitudeGain),
                GameSettingJsonData.GetFloat(GameSetting.CamShake_BossDebut_FrequencyGain),
                GameSettingJsonData.GetFloat(GameSetting.CamShake_BossDebut_Duration));
        }

        public void UpdateHeros() {
            int count = 1;
            if (GameConnector.Connected || AllocatedRoom.Instance.HeroIDs == null) return;
            for (int i = 0; i < MAX_HERO_COUNT; i++) {
                int playerIndex = i;
                if (Index == playerIndex) continue;

                int heroIndex = GetHeroIndex(playerIndex);
                Hero hero = GetHero(heroIndex);
                // id為0，代表沒有這個hero
                int heroId = AllocatedRoom.Instance.HeroIDs[i];
                if (heroId == 0) {
                    if (hero.IsLoaded) {
                        hero.ResetData();
                        OnHeroRemove?.Invoke(playerIndex);
                    }
                    continue;
                }

                count++;
                if (hero.IsLoaded) continue;
                hero.SetData(heroId, AllocatedRoom.Instance.HeroSkinIDs[i]);
                OnHeroAdd?.Invoke(heroId, playerIndex);
            }
            _playerCount = count;
        }

        // 取得Hero的Index值，若自己的player index為1，那hero index是0
        // 對方的player index為2時，hero index為1
        // 相反的，對方的player index為0，hero index為3
        public int GetHeroIndex(int playerIndex) {
            return (playerIndex - Index + MAX_HERO_COUNT) % MAX_HERO_COUNT;
        }

        public Hero GetHero(int _index) {
            if (_index < 0 || _index >= MyHeros.Length) return null;
            return MyHeros[_index];
        }

        public void Attack(ATTACK_TOCLIENT content) {
            var spellData = HeroSpellJsonData.GetData(content.SpellJsonID);
            var heroIndex = GetHeroIndex(content.PlayerIdx);
            var hero = GetHero(heroIndex);
            Quaternion rotation = Quaternion.AngleAxis(heroIndex * -90, Vector3.up);
            hero.FaceDir(rotation);
            hero.PlaySpell(spellData.SpellName);
            Vector3 attackPos = rotation * new Vector3((float)content.AttackPos[0], (float)content.AttackPos[1], (float)content.AttackPos[2]);
            Vector3 attackDir = rotation * new Vector3((float)content.AttackDir[0], (float)content.AttackDir[1], (float)content.AttackDir[2]);
            SpellPlayData playData = new SpellPlayData {
                lockAttack = content.AttackLock,
                monsterIdx = content.MonsterIdx,
                heroIndex = heroIndex,
                heroPos = hero.transform.position,
                attackPos = attackPos,
                direction = attackDir,
            };
            spellData.Spell.Play(playData);
        }

        public void UpdateScene(Spawn[] spawns, SceneEffect[] effects) {
            try {
                if (spawns == null || spawns.Length == 0) return;

                // 取出server有的monster索引
                List<int> serverMonsterIdxs = new List<int>(128);
                var clientMonsterIdxs = Monster.IdxToMonsterMapping.Keys;
 
                for (int i = 0; i < spawns.Length; i++) {
                    var spawn = spawns[i];
                    var routeData = RouteJsonData.GetData(spawn.RID);
                    var rotation = Quaternion.AngleAxis(Index * 90f, Vector3.up);
                    var initRotation = Quaternion.LookRotation(routeData.TargetPos - routeData.SpawnPos);
                    bool found = false;
                    for (int j = 0; j < spawn.Ms.Length; j++) {
                        var spawnMonster = spawn.Ms[j];
                        if (!spawnMonster.Death) serverMonsterIdxs.Add(spawnMonster.Idx);
                        // Server端有的怪物, 且client端也有就更新怪物狀態
                        if (Monster.TryGetMonsterByIdx(spawnMonster.Idx, out Monster monster)) {
                            var monsterData = monster.MyData;
                            float deltaTime = GameTime.Current - (float)spawn.STime;
                            Vector3 deltaPosition = deltaTime * monsterData.Speed * (initRotation * Vector3.forward);
                            // 先註解掉, 因為要超出邊界的怪物也需要移動位置, 否則可能發生client端的怪物還在邊界內但server端已經在邊界外的情形
                            // 可能會導致玩家A看玩家B打死邊界外的怪物
                            //if (Vector3.SqrMagnitude(deltaPosition) > Vector3.SqrMagnitude(routeData.TargetPos - routeData.SpawnPos))
                            //    continue;
                            monster.GetComponent<MonsterGrid>().Teleport(rotation * (routeData.SpawnPos + deltaPosition));
                            found = true;
                        }
                    }

                    // Server端有的怪物, 但client端沒有就要補
                    if (!found) {
                        MyMonsterScheduler.EnqueueMonster(spawn, Index);
                    }

                    //NativeArray<MonsterData> monsterDatas = new NativeArray<MonsterData>(spawns[i].Monsters.Length, Allocator.Persistent);
                    //for (int j = 0; j < monsterDatas.Length; j++) {
                    //    var monster = spawns[i].Monsters[j];
                    //    if (monster == null || monster.Death) continue;
                    //    MonsterData monsterData = new MonsterData() {
                    //        ID = monster.JsonID,
                    //        Idx = monster.Idx,
                    //    };
                    //    monsterDatas[j] = monsterData;
                    //}
                    //var entity = _entityManager.CreateEntity();
                    //_entityManager.AddComponentData(entity, new SpawnData {
                    //    Monsters = monsterDatas,
                    //    RouteID = spawns[i].RouteJsonID,
                    //    SpawnTime = (float)spawns[i].SpawnTime,
                    //    IsBoss = spawns[i].IsBoss,
                    //    PlayerIndex = Index,
                    //});
                    //_entityManager.AddComponent<RefreshSceneTag>(entity);
                }

                var clientExclusiveMonsters = clientMonsterIdxs.Except(serverMonsterIdxs).ToList(); //client有但server沒有的怪物
                // Server端沒有的怪物, 但client端有就要將client端的怪物移除
                for (int i = 0; i < clientExclusiveMonsters.Count; i++) {
                    if (Monster.TryGetMonsterByIdx(clientExclusiveMonsters[i], out var monster)) {
                        monster.DestroyGOAfterDelay(0.1f * i);
                    }
                }
            } catch (Exception ex) {
                Debug.LogError("update scene error " + ex);
            }
        }

        public void SetMonsterDead(int playerIndex, int[] monsterIdxs, long[] gainPoints, int[] gainHeroExps, int[] gainSpellCharge, int[] gainDrops) {
            int heroIndex = GetHeroIndex(playerIndex);
            var hero = GetHero(heroIndex);
            int totalExp = 0;
            for (int i = 0; i < monsterIdxs.Length; i++) {
                if (heroIndex == 0) {
                    hero.HoldStoredPoints(monsterIdxs[i], (int)gainPoints[i]);
                }
                if (Monster.TryGetMonsterByIdx(monsterIdxs[i], out Monster monster)) {
                    monster.Die(heroIndex);
                }
                totalExp += gainHeroExps[i];
            }
            //var entity = _entityManager.CreateEntity();

            //long totalPoints = 0;
            //int totalExp = 0;
            //NativeArray<KillMonsterData> killMonsters = new(monsterIdxs.Length, Allocator.Persistent);
            //for (int i = 0; i < killMonsters.Length; i++) {
            //    var killMonster = new KillMonsterData {
            //        HeroIndex = heroIndex,
            //        KillMonsterIdx = monsterIdxs[i],
            //        GainPoints = gainPoints[i],
            //        GainHeroExp = gainHeroExps[i],
            //        GainSpellCharge = gainSpellCharge[i],
            //        GainDrop = gainDrops[i],
            //    };
            //    killMonsters[i] = killMonster;
            //    totalPoints += gainPoints[i];
            //    totalExp += gainHeroExps[i];
            //}

            //MonsterDieNetworkData monsterDieNetworkData = new MonsterDieNetworkData {
            //    KillMonsters = killMonsters,
            //};

            //_entityManager.AddComponentData(entity, monsterDieNetworkData);

            if (heroIndex == 0) {
                hero.AddExp(totalExp);
                hero.ChargeSpell(gainSpellCharge);
            }
        }
    }
}