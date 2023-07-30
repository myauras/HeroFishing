using HeroFishing.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HeroFishing.Battle {
    public class BattleManager : MonoBehaviour {
        public static BattleManager Instance;

        [SerializeField] Hero[] MyHeros;

        public MonsterScheduler MyMonsterScheduler { get; private set; }


        public void Init() {
            Instance = this;
            InitMonsterScheduler();
        }
        void InitMonsterScheduler() {
            MyMonsterScheduler = new MonsterScheduler();
            MyMonsterScheduler.Init(MapData.GetData(7));
        }
        public Hero GetHero(int _index) {
            if (_index < 0 || _index >= MyHeros.Length) return null;
            return MyHeros[_index];
        }
    }
}