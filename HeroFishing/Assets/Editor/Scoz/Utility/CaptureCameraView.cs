using UnityEngine;
using UnityEditor;
using System.IO;

public class CaptureCameraView : EditorWindow {
    Camera selectedCamera;

    [MenuItem("Scoz/Utility/Capture Selected Camera View as PNG")]
    public static void ShowWindow() {
        GetWindow<CaptureCameraView>("Capture Camera View");
    }

    void OnGUI() {
        GUILayout.Label("Capture Camera View as PNG", EditorStyles.boldLabel);

        selectedCamera = EditorGUILayout.ObjectField("Select Camera", selectedCamera, typeof(Camera), true) as Camera;

        if (GUILayout.Button("Capture and Save PNG")) {
            if (selectedCamera != null) {
                CaptureAndSave(selectedCamera);
            } else {
                Debug.LogError("No camera selected!");
            }
        }
    }

    void CaptureAndSave(Camera camera) {
        // ���Ĳ�oCinemachine��s
        

        RenderTexture currentRT = RenderTexture.active;

        // �T�O���@��targetTexture�Ӵ�V
        if (camera.targetTexture == null) {
            camera.targetTexture = new RenderTexture(camera.pixelWidth, camera.pixelHeight, 24);
        }

        camera.Render(); // �T�O��V��e�V

        RenderTexture.active = camera.targetTexture;

        Texture2D image = new Texture2D(camera.targetTexture.width, camera.targetTexture.height, TextureFormat.RGBA32, false);
        image.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
        image.Apply();

        byte[] bytes = image.EncodeToPNG();
        DestroyImmediate(image); // ����귽

        string path = EditorUtility.SaveFilePanel("Save PNG", "", "CameraCapture.png", "png");
        if (path.Length != 0) {
            File.WriteAllBytes(path, bytes);
            Debug.Log("Saved Camera View to: " + path);
        }

        // ��_��v�����]�m
        camera.targetTexture = null; // ���n�G�N��v����targetTexture�]�m�^null
        RenderTexture.active = currentRT; // ��_��l��RenderTexture
    }
}
