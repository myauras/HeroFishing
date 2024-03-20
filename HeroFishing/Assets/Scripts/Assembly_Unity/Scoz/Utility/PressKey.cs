using UnityEngine;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

namespace Scoz.Func {
    public class PressKey : MonoBehaviour {

        private void Update() {
            KeyDetector();
        }

        [ContextMenu("Snap Shot")]
        public void SnapShot() {
            string date = DateTime.Now.ToString("MMdd-HHmmss");
            string path = $"Assets/CaptureScreenshot_{date}.png";
            Debug.LogError("存圖片至路徑: " + path);
            ScreenCapture.CaptureScreenshot(path);
        }
        // Update is called once per frame
        void KeyDetector() {


            if (Input.GetKeyDown(KeyCode.Q)) {
                SnapShot();

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
