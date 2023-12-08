using RayFire;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterExplosion : MonoBehaviour
{
    [SerializeField]
    private Renderer _originRenderer;
    [SerializeField]
    private RayfireRigid _rigid;
    [SerializeField]
    private RayfireBomb _bomb;

    public void Explode() {
        _rigid.gameObject.SetActive(true);
        if (!_rigid.initialized)
            _rigid.Initialize();

        _originRenderer.enabled = false;

        _bomb.Explode(0.5f);
    }
}
