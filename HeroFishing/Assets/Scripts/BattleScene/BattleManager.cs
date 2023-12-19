using HeroFishing.Main;
using Scoz.Func;
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

        public MonsterScheduler MyMonsterScheduler { get; private set; }
        public static float3 MonsterCollisionPosOffset { get; private set; }//因為怪物的位置是在地板 所以檢測碰撞半徑時以地板為圓心的話子彈會打不到 所以碰撞檢測時要將判定的圓心高度提高到子彈高度

        public void Init() {
            Instance = this;
            InitMonsterScheduler();
            InitPlayerHero();
            MonsterCollisionPosOffset = new float3(0, GameSettingJsonData.GetFloat(GameSetting.Bullet_PositionY), 0);
        }
        void InitPlayerHero() {
            if (AllocatedRoom.Instance == null) //測試流程
                GetHero(0).SetData(BattleSceneManager.Instance.HeroID, BattleSceneManager.Instance.HeroSkin);
            else
                GetHero(0).SetData(AllocatedRoom.Instance.MyHeroID, AllocatedRoom.Instance.MyHeroSkinID);
        }
        void InitMonsterScheduler() {
            MyMonsterScheduler = new MonsterScheduler();
            MyMonsterScheduler.Init(MapJsonData.GetData(7), AllocatedRoom.Instance == null);
        }
        void LoadAllMonster() {

            AddressablesLoader.GetPrefab("Monsters", (go, handle) => {
            });
        }
        public Hero GetHero(int _index) {
            if (_index < 0 || _index >= MyHeros.Length) return null;
            return MyHeros[_index];
        }
        public void SetMonsterDead(int[] monsterIdxs, long[] gainPoints, int[] gainHeroExps, int[] gainSpellCharge, int[] gainDrops) {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entity = entityManager.CreateEntity();

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

            entityManager.AddComponentData(entity, monsterDieNetworkData);
        }
    }
}