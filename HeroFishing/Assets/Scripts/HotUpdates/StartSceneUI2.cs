using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HeroFishing.Main {
    public class StartSceneUI2 : MonoBehaviour {

        public static StartSceneUI2 Instance { get; private set; }
        [SerializeField] Text MyText;
        [SerializeField] Text MyText2;
        [SerializeField] Text MyText3;
        [SerializeField] Text MyText4;

        private void Start() {
            Instance = this;
            MyText.text = "測試1";
            MyText2.text = "測試2";
            MyText3.text = "測試3";
            MyText4.text = "測試4";
        }


    }
}