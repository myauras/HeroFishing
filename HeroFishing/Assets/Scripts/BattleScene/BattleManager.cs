using HeroFishing.Main;
using HeroFishing.Socket;
using HeroFishing.Socket.Matchgame;
using Scoz.Func;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
namespace HeroFishing.Battle {
    public class BattleManager : MonoBehaviour {
        public static BattleManager Instance;

        [SerializeField] Hero[] MyHeros;
        [SerializeField] public Transform MonsterParent;
        [SerializeField]
        private bool _rotateTest;
        [SerializeField]
        private int _rotateTestIndex;

        public int Index {
            get {
                if (_rotateTest || AllocatedRoom.Instance == null) return _rotateTestIndex;
                return AllocatedRoom.Instance.Index;
            }
        }
        private EntityManager _entityManager;

        public MonsterScheduler MyMonsterScheduler { get; private set; }
        public static float3 MonsterCollisionPosOffset { get; private set; }//因為怪物的位置是在地板 所以檢測碰撞半徑時以地板為圓心的話子彈會打不到 所以碰撞檢測時要將判定的圓心高度提高到子彈高度

        private const int MAX_HERO_COUNT = 4;

        public void Init() {
            Instance = this;
            InitMonsterScheduler();
            InitPlayerHero();
            MonsterCollisionPosOffset = new float3(0, GameSettingJsonData.GetFloat(GameSetting.Bullet_PositionY), 0);
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            DeviceManager.AddOnFocusAction(() => {
                if (GameConnector.Connected)
                    GameConnector.Instance.UpdateScene();
            });

            if (GameConnector.Connected) {
                GameConnector.Instance.UpdateScene();
            }
        }

        private void InitPlayerHero() {
            if (AllocatedRoom.Instance == null) //測試流程
                GetHero(0).SetData(BattleSceneManager.Instance.HeroID, BattleSceneManager.Instance.HeroSkin);
            else
                GetHero(0).SetData(AllocatedRoom.Instance.MyHeroID, AllocatedRoom.Instance.MyHeroSkinID);
        }

        private void InitMonsterScheduler() {
            MyMonsterScheduler = new MonsterScheduler();
            MyMonsterScheduler.Init(MapJsonData.GetData(7), AllocatedRoom.Instance == null);
        }

        private void Update() {
            SpawnMonster();
        }

        private void SpawnMonster() {
            if (MyMonsterScheduler == null) return;
            if (!MyMonsterScheduler.TryDequeueMonster(out SpawnData spawnData)) return;

            var entity = _entityManager.CreateEntity();
            _entityManager.AddComponentData(entity, spawnData);
            _entityManager.AddComponent<SpawnTag>(entity);

            //是BOSS就會攝影機震動
            if (spawnData.IsBoss)
                CamManager.ShakeCam(CamManager.CamNames.Battle,
                    GameSettingJsonData.GetFloat(GameSetting.CamShake_BossDebut_AmplitudeGain),
                    GameSettingJsonData.GetFloat(GameSetting.CamShake_BossDebut_FrequencyGain),
                    GameSettingJsonData.GetFloat(GameSetting.CamShake_BossDebut_Duration));
        }

        public void UpdateHeros() {
            for (int i = 0; i < MAX_HERO_COUNT; i++) {
                int playerIndex = i;
                if (Index == playerIndex) continue;
                // id為0，代表沒有這個hero
                int id = AllocatedRoom.Instance.HeroIDs[i];
                if (id == 0) continue;
                int heroIndex = GetHeroIndex(playerIndex);
                Hero hero = GetHero(heroIndex);
                if (hero.IsLoaded) continue;
                hero.SetData(id, AllocatedRoom.Instance.HeroSkinIDs[i]);
            }
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
            for (int i = 0; i < spawns.Length; i++) {
                NativeArray<MonsterData> monsterDatas = new NativeArray<MonsterData>(spawns[i].Monsters.Length, Allocator.Persistent);
                for (int j = 0; j < monsterDatas.Length; j++) {
                    var monster = spawns[i].Monsters[j];
                    if (monster == null || monster.Death) continue;
                    MonsterData monsterData = new MonsterData() {
                        ID = monster.JsonID,
                        Idx = monster.Idx,
                    };
                    monsterDatas[j] = monsterData;
                }
                var entity = _entityManager.CreateEntity();
                _entityManager.AddComponentData(entity, new SpawnData {
                    Monsters = monsterDatas,
                    RouteID = spawns[i].RouteJsonID,
                    SpawnTime = (float)spawns[i].SpawnTime,
                    IsBoss = spawns[i].IsBoss,
                    PlayerIndex = Index,
                });
                _entityManager.AddComponent<RefreshSceneTag>(entity);
            }
        }

        public void SetMonsterDead(int[] monsterIdxs, long[] gainPoints, int[] gainHeroExps, int[] gainSpellCharge, int[] gainDrops) {
            var entity = _entityManager.CreateEntity();

            NativeArray<KillMonsterData> killMonsters = new(monsterIdxs.Length, Allocator.Persistent);
            for (int i = 0; i < killMonsters.Length; i++) {
                var killMonster = new KillMonsterData {
                    KillMonsterIdx = monsterIdxs[i],
                    GainPoints = gainPoints[i],
                    GainHeroExp = gainHeroExps[i],
                    GainSpellCharge = gainSpellCharge[i],
                    GainDrop = gainDrops[i],
                };
                killMonsters[i] = killMonster;
            }

            MonsterDieNetworkData monsterDieNetworkData = new MonsterDieNetworkData {
                KillMonsters = killMonsters,
            };

            _entityManager.AddComponentData(entity, monsterDieNetworkData);
        }
    }
}