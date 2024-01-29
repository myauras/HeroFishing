using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

/// <summary>
/// Load Async or Sync the scene indicated in the sceneName variable.
/// </summary>
public class loadScene : MonoBehaviour {
    public string sceneName = "";
    public bool loadSceneAsync = true;
    public bool showAd = false;

    public void OnClick() {
        if (loadSceneAsync) {
            LevelManager.Load(sceneName);
        } else {
            SceneManager.LoadScene(sceneName);
        }
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
