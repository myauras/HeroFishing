using UnityEngine;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

namespace Scoz.Func {
    public class PressKey : MonoBehaviour {

        private void Update() {
            KeyDetector();
        }
        // Update is called once per frame
        void KeyDetector() {


            if (Input.GetKeyDown(KeyCode.Q)) {
                string path = "Assets/CaptureScreenshot.png";
                Debug.LogError("存圖片至路徑: " + path);
                ScreenCapture.CaptureScreenshot(path);

            } else if (Input.GetKeyDown(KeyCode.T)) {

            } else if (Input.GetKeyDown(KeyCode.W)) {

            } else if (Input.GetKeyDown(KeyCode.E)) {


            } else if (Input.GetKeyDown(KeyCode.R)) {


            } else if (Input.GetKeyDown(KeyCode.P)) {
            } else if (Input.GetKeyDown(KeyCode.O)) {

            } else if (Input.GetKeyDown(KeyCode.I)) {

            } else if (Input.GetKeyDown(KeyCode.L)) {

            }
        }

    }
}
