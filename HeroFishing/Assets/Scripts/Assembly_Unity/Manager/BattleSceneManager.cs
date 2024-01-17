using HeroFishing.Main;
using UnityEngine;

namespace HeroFishing.Battle {
    public class BattleSceneManager : MonoBehaviour {
        void Start() {
            BaseManager.CreateNewInstance();
        }
    }
}