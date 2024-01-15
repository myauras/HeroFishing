using Cysharp.Threading.Tasks;
using DG.Tweening;
using HeroFishing.Battle;
using HeroFishing.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SpellActivationData {
    public SpellName spellName;
    public GameObject target;
    public float delay;
    public float duration;
}

public class SpellActivationBehaviour : MonoBehaviour {
    [SerializeField]
    private SpellActivationData[] _spellActivationData;

    private void Start() {
        var hero = GetComponentInParent<Hero>();
        hero.Register(this);
        foreach(var data in _spellActivationData) {
            data.target.SetActive(false);
        }
    }

    public async void OnSpellPlay(SpellName spellName) {
        foreach (var data in _spellActivationData) {
            if (data.spellName == spellName) {
                await UniTask.WaitForSeconds(data.delay);
                data.target.SetActive(true);
                await UniTask.WaitForSeconds(data.duration);
                data.target.SetActive(false);
            }
        }
    }
}
