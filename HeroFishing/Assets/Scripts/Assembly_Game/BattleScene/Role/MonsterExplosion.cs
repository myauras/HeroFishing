using RayFire;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterExplosion : MonoBehaviour {
    [SerializeField]
    private Renderer _originRenderer;
    [SerializeField]
    private RayfireBomb _bomb;
    [SerializeField]
    private RayfireRigid[] _rigids;
    [SerializeField]
    private List<Renderer> _renderers;

    private void Reset() {
        _originRenderer = GetComponentInChildren<Renderer>();
        _rigids = GetComponentsInChildren<RayfireRigid>();
        _bomb = GetComponentInChildren<RayfireBomb>();
    }

    public void SetMaterials(int rigidIndex, List<Material> materials) {
        if (rigidIndex >= _rigids.Length) {
            WriteLog.LogErrorFormat("out of range for rigid count {0}", rigidIndex);
            return;
        }

        var fragments = _rigids[rigidIndex].fragments;
        for (int i = 0; i < fragments.Count; i++) {
            var f = fragments[i];
            var r = f.meshRenderer;
            List<Material> matList = new List<Material>();
            for (int j = 0; j < r.sharedMaterials.Length; j++) {
                Material material = null;
                if (r.sharedMaterials[j].GetTag("RenderType", false) == "Opaque") {
                    material = new Material(ResourcePreSetter.GetMaterial("FreezeOpaque"));

                }else if (r.sharedMaterials[j].GetTag("RenderType", false) == "Transparent") {
                    material = new Material(ResourcePreSetter.GetMaterial("FreezeTransparent"));
                }
                material.SetFloat("_EdgePower", 2.8f);
                material.SetFloat("_FreezeMask", 2f);
                material.SetFloat("_IceAmount", 0.25f);
                material.SetFloat("_IcicleAmount", 1);
                material.SetFloat("_Metallic", 0.2f);
                material.SetFloat("_Smooth", 0.4f);
                matList.Add(material);
            }
            r.SetSharedMaterials(matList);
            //if (r.sharedMaterials.Length == materials.Count)
            //    r.SetSharedMaterials(materials);
            //else {
            //    r.SetSharedMaterials(materials.GetRange(0, r.materials.Length));
            //}
        }
    }

    public void Explode() {
        foreach (var rigid in _rigids) {
            rigid.gameObject.SetActive(true);
            if (!rigid.initialized)
                rigid.Initialize();
        }

        _originRenderer.enabled = false;

        _bomb.Explode(0.5f);
    }
}
