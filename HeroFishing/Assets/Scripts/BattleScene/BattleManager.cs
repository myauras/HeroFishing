using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HeroFishing.Battle {
    public class BattleManager : MonoBehaviour {
        public static BattleManager Instance;

        [SerializeField]
        Hero[] MyHeros;



        // Start is called before the first frame update
        void Awake() {
            Init();
        }
        public void Init() {
            Instance = this;
        }
        public Hero GetHero(int _index) {
            if (_index < 0 || _index >= MyHeros.Length) return null;
            return MyHeros[_index];
        }
    }
}