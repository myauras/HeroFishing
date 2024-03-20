using DG.Tweening;
using Scoz.Func;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class SceneMaterialConverter : MonoBehaviour {
    [SerializeField]
    private GraphicConverter _graphicConverter;
    private Material _matFreezeOpaque;
    private Material _matFreezeTransparent;
    private Renderer[] _renderers;

    private static SceneMaterialConverter _instance;
    public static SceneMaterialConverter Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<SceneMaterialConverter>();
            }
            if (_instance == null) {
                WriteLog.LogErrorFormat("scene material converter is null");
                return null;
            }
            _instance.Init();
            return _instance;
        }
    }

    private List<Material> _originMats = new List<Material>();
    private List<Material> _freezeMats = new List<Material>();

    private const int SMOOTH_TIME = 1500;

    public void Init() {
        _matFreezeOpaque = ResourcePreSetter.GetMaterial("FreezeOpaque");
        _matFreezeTransparent = ResourcePreSetter.GetMaterial("FreezeTransparent");
        _renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in _renderers) {
            _originMats.AddRange(r.sharedMaterials);
        }
    }

    public void Freeze() {
        if (_freezeMats.Count == 0)
            SetupFreezeMats();
        else {
            Debug.Log("Freeze");
            int startIndex = 0;
            for (int i = 0; i < _renderers.Length; i++) {
                var renderer = _renderers[i];
                var matList = _freezeMats.GetRange(startIndex, renderer.materials.Length);
                renderer.SetSharedMaterials(matList);
                startIndex += renderer.materials.Length;
            }
            FreezeSmoothly();
        }
    }

    public void Unfreeze() {
        var materials = _renderers.SelectMany(r => r.sharedMaterials);
        double timer = 0;
        Observable.EveryUpdate().TakeUntil(Observable.Timer(TimeSpan.FromMilliseconds(SMOOTH_TIME))).TimeInterval().Subscribe(t => {
            var deltaTime = t.Interval.TotalMilliseconds;
            timer += deltaTime;
            var value = timer / SMOOTH_TIME;
            foreach (var material in materials) {
                material.SetFloat("_EdgePower", Mathf.Lerp(8, 2, (float)value));
                material.SetFloat("_FreezeMask", Mathf.Lerp(2, 0, (float)value));
                material.SetFloat("_IceAmount", Mathf.Lerp(0.25f, 0, (float)value));
            }

            if (_graphicConverter != null) {
                _graphicConverter.ChangeValue(1 - (float)value);
            }
        }, () => {
            int matIndex = 0;
            foreach (var renderer in _renderers) {
                for (int j = 0; j < renderer.sharedMaterials.Length; j++) {
                    var material = renderer.sharedMaterials[j];
                    material.SetFloat("_EdgePower", 2);
                    material.SetFloat("_FreezeMask", 0);
                    material.SetFloat("_IceAmount", 0);
                }
                var matList = _originMats.GetRange(matIndex, renderer.materials.Length);
                renderer.SetSharedMaterials(matList);
                matIndex += renderer.materials.Length;
            }

            if (_graphicConverter != null) {
                _graphicConverter.ChangeValue(0);
            }
        });

    }

    private void SetupFreezeMats() {
        int startIndex = 0;
        foreach (var renderer in _renderers) {
            var matList = new List<Material>();
            for (int i = 0; i < renderer.sharedMaterials.Length; i++) {
                var oldMat = renderer.sharedMaterials[i];
                if (oldMat != null) {
                    var texture = oldMat.mainTexture;
                    var tag = oldMat.GetTag("RenderType", false);
                    Material newMat;

                    if (tag == "Transparent")
                        newMat = new Material(_matFreezeTransparent);
                    else
                        newMat = new Material(_matFreezeOpaque);

                    //if (newMat == null) {
                    //    // 存失敗，為了不打亂list順序，還是要存個null
                    //    WriteLog.LogErrorFormat("new mat is null.");
                    //    matList.Add(null);
                    //    continue;
                    //}
                    newMat.mainTexture = texture;
                    matList.Add(newMat);
                }
            }
            // 設定renderer
            renderer.SetSharedMaterials(matList);
            _freezeMats.AddRange(matList);
            startIndex += renderer.materials.Length;
        }
        FreezeSmoothly();
    }

    private void FreezeSmoothly() {
        var materials = _renderers.SelectMany(r => r.sharedMaterials);

        double timer = 0;
        Observable.EveryUpdate().TakeUntil(Observable.Timer(TimeSpan.FromMilliseconds(SMOOTH_TIME))).TimeInterval().Subscribe(t => {
            var deltaTime = t.Interval.TotalMilliseconds;
            timer += deltaTime;
            float value = (float)timer / SMOOTH_TIME;
            foreach (var material in materials) {
                material.SetFloat("_EdgePower", Mathf.Lerp(0, 8f, value));
                material.SetFloat("_FreezeMask", value * 2f);
                material.SetFloat("_IceAmount", value * 0.25f);
            }

            if (_graphicConverter != null) {
                _graphicConverter.ChangeValue((float)value);
            }
        }, () => {
            foreach (var material in materials) {
                material.SetFloat("_EdgePower", 8);
                material.SetFloat("_FreezeMask", 2);
                material.SetFloat("_IceAmount", 0.25f);
            }

            if (_graphicConverter != null) {
                _graphicConverter.ChangeValue(1);
            }
        });
    }
}
