using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using HeroFishing.Main;
using SimpleJSON;

namespace Scoz.Func {
    public partial class Debugger : MonoBehaviour {

        [SerializeField] GameObject ToolGO;

        public static Animator MyAni;
        // Update is called once per frame
        void KeyDetector() {


            if (Input.GetKeyDown(KeyCode.Q)) {
                CamManager.ShakeCam(CamManager.CamNames.Battle, 3, 3, 2f);
            } else if (Input.GetKeyDown(KeyCode.W)) {


            } else if (Input.GetKeyDown(KeyCode.E)) {


            } else if (Input.GetKeyDown(KeyCode.R)) {


            } else if (Input.GetKeyDown(KeyCode.P)) {
                ToolGO?.SetActive(!ToolGO.activeSelf);


            } else if (Input.GetKeyDown(KeyCode.O)) {

            } else if (Input.GetKeyDown(KeyCode.I)) {

            }
        }

        public void OnModifyHP(int _value) {
        }
        public void OnModifySanP(int _value) {
        }
        public void ClearLocoData() {
            PlayerPrefs.DeleteAll();
        }

    }
}
