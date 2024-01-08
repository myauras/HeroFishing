using Cysharp.Threading.Tasks;
using DG.Tweening;
using HeroFishing.Battle;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

public class FrozenTest : MonoBehaviour
{
    [SerializeField]
    private Material _frozenMat;
    [SerializeField]
    private Material _frozenMat2;
    [SerializeField]
    private Material _frozenTest;
    [SerializeField]
    private Material _frozenTestURP;
    [SerializeField]
    private Transform _scene;
    [SerializeField]
    private GameObject _snowEffect;
    private Renderer[] _renderers;
    private Dictionary<Renderer, List<Material>> _originMatDic = new Dictionary<Renderer, List<Material>>();
    private Dictionary<Renderer, List<Material>> _frozenMatDic = new Dictionary<Renderer, List<Material>>();
    private bool _isConvert;
    private bool _isMonsterFrozen;
    private static FrozenTest Instance;
    public bool SetURPMat { get; set; }
    //public static bool IsFrozen => Instance._isMonsterFrozen;

    private void Start() {
        Instance = this;
        //_renderers = _scene.GetComponentsInChildren<Renderer>();
        //foreach(var r in _renderers) {
        //    var list = new List<Material>(r.sharedMaterials);
        //    _originMatDic.Add(r, list);
        //}
    }

    public void ConvertM() {
        _isMonsterFrozen = !_isMonsterFrozen;
        WorldStateManager.Instance.Freeze(_isMonsterFrozen);
        
    }

    [ContextMenu("Convert")]
    public void Convert() {
        //if (!_isConvert) {
        //    _snowEffect.SetActive(true);
        //    foreach (var r in _renderers) {
        //        var materials = r.materials;
        //        for (int i = 0; i < materials.Length; i++) {
        //            if (r.materials[i] != null) {
        //                var tex = materials[i].mainTexture;
        //                var tag = materials[i].GetTag("RenderType", false);
        //                if (tag == "Opaque")
        //                    materials[i] = SetURPMat ? new Material(_frozenTestURP) : new Material(_frozenTest);
        //                else if (tag == "Transparent")
        //                    materials[i] = SetURPMat ? new Material(_frozenTestURP) : new Material(_frozenTest);
        //                materials[i].mainTexture = tex;
        //                //Debug.Log(materials[i].name);
        //            }
        //        }
        //        r.materials = materials;
        //        //_frozenMatDic.Add(r, )
        //    }

        //    //if (_frozenMatDic.Count == 0) {
        //    //    foreach (var r in _renderers) {
        //    //        List<Material> list = new List<Material>(r.materials);
        //    //        //int index = 0;
        //    //        for (int i = 0; i < list.Count; i++) {
        //    //            var m = list[i];
        //    //            var tag = m.GetTag("RenderType", false);
        //    //            Material srcMat = null;
        //    //            if (tag == "Opaque")
        //    //                srcMat = _frozenMat;
        //    //            else if (tag == "Transparent")
        //    //                srcMat = _frozenMat2;
        //    //            if (srcMat == null)
        //    //                Debug.Log("error");
        //    //            //var frozenMat = srcMat;
        //    //            list[i] = srcMat;
        //    //            //var frozenMat = new Material(srcMat);
        //    //            srcMat.mainTexture = m.mainTexture;
        //    //        }
        //    //        r.SetMaterials(list);
        //    //        _frozenMatDic.Add(r, list);
        //    //    }
        //    //}
        //    //else {
        //    //    foreach(var r in _renderers) {
        //    //        r.SetSharedMaterials(_frozenMatDic[r]);
        //    //    }
        //    //}
        //    Test();
        //    _isConvert = true;
        //}
        //else {
        //    _snowEffect.SetActive(false);
        //    foreach (var r in _renderers) {
        //        foreach (var material in r.materials) {
        //            material.SetFloat("_EdgePower", 50);
        //            material.SetFloat("_FreezeMask", 0);
        //            material.SetFloat("_IceAmount", 0);
        //        }
        //        r.SetSharedMaterials(_originMatDic[r]);
        //    }
        //    _isConvert = false;
        //}
    }

    private void Test() {
        var materials = _renderers.SelectMany(r => r.sharedMaterials);
        foreach (var material in materials) {
            material.SetFloat("_EdgePower", 50);
            material.SetFloat("_FreezeMask", 0);
            material.SetFloat("_IceAmount", 0);
        }
        //Debug.Log("test");
        //UniTask.RunOnThreadPool(() => {
        //    Debug.Log("task");
        DOTween.To(() => 0f, value => {
                //Debug.Log(value);
                foreach(var  material in materials) {
                    material.SetFloat("_EdgePower", Mathf.Lerp(50, 8f, value));
                    material.SetFloat("_FreezeMask", value * 1.8f);
                    material.SetFloat("_IceAmount", value * 0.2f);
                }
            }, 1.0f, 1.0f);

        //});
    }
}
