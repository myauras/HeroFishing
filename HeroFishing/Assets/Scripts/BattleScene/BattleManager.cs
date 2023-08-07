using HeroFishing.Main;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HeroFishing.Battle {
    public class BattleManager : MonoBehaviour {
        public static BattleManager Instance;

        [SerializeField] Hero[] MyHeros;
        [SerializeField] public Transform MonsterParent;

        public MonsterScheduler MyMonsterScheduler { get; private set; }



        public void Init() {
            Instance = this;
            InitMonsterScheduler();
            InitPlayerHero();
        }
        void InitPlayerHero() {
            GetHero(0).SetData(19, "19_2");
        }
        void InitMonsterScheduler() {
            MyMonsterScheduler = new MonsterScheduler();
            MyMonsterScheduler.Init(MapData.GetData(7));
        }
        void LoadAllMonster() {

            AddressablesLoader.GetPrefab("Monsters", (go, handle) => {
            });
        }
        public Hero GetHero(int _index) {
            if (_index < 0 || _index >= MyHeros.Length) return null;
            return MyHeros[_index];
        }
    }
}