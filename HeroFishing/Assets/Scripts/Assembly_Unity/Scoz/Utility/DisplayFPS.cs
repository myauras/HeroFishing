using UnityEngine;
using UnityEngine.UI;

public class DisplayFPS : MonoBehaviour {
    public Text fpsText;
    private float deltaTime = 0.0f;
    private void Start() {
        Application.targetFrameRate = 60;
    }

    void Update() {
        // 計算過去一幀所花的時間
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        // 計算FPS
        float fps = 1.0f / deltaTime;

        // 格式化文字並顯示
        fpsText.text = string.Format("{0:0.} fps", fps);
    }
}
