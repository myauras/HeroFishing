using UnityEngine;

namespace HeroFishing.Main {
    public class StartSceneManager : MonoBehaviour {
        private void Start() {
            BaseManager.CreateNewInstance();
        }
    }
}