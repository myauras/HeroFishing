using HeroFishing.Main;
using HeroFishing.Socket;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class DropManager : MonoBehaviour {
    private static DropManager _instance;
    public static DropManager Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<DropManager>();
                if (_instance == null) {
                    var go = new GameObject("DropManager");
                    _instance = go.AddComponent<DropManager>();
                    _instance.Init();
                }
            }
            return _instance;
        }
    }

    private List<int> _currentDrops = new List<int>();
    private Dictionary<int, DropBase> _dropSpells = new Dictionary<int, DropBase>();

    //public event Action<int> OnSpellAdd;
    //public event Action<int> OnSpellPlay;

    private void Init() {

    }

    public void AddDrop(int dropID) {
        if (_currentDrops.Contains(dropID)) return;
        DropJsonData data = DropJsonData.GetData(dropID);
        if (data.MyDropType == DropJsonData.DropType.Spell) {
            DropSpellJsonData spellData = DropSpellJsonData.GetData(dropID);
            if (!_dropSpells.TryGetValue(dropID, out var drop)) {
                drop = GetDropSpell(data, spellData);
                _dropSpells[dropID] = drop;
            }

            drop.AddDrop();
            _currentDrops.Add(dropID);
        }
    }

    public void PlayDrop(int dropID) {
        if (_dropSpells.TryGetValue(dropID, out var drop) && _currentDrops.Contains(dropID)) {
            if (drop.PlayDrop()) {

                if (drop.Duration == 0)
                    _currentDrops.Remove(dropID);
                else {
                    Observable.Timer(TimeSpan.FromSeconds(drop.Duration)).Subscribe(_ => _currentDrops.Remove(dropID));
                }

                if (GameConnector.Connected) {
                    GameConnector.Instance.DropSpell(dropID);
                }
            }
        }
    }

    private DropSpellBase GetDropSpell(DropJsonData data, DropSpellJsonData spellData) {
        switch (spellData.MyEffectType) {
            case DropSpellJsonData.EffectType.Frozen:
                return new DropFrozen(data, spellData);
            case DropSpellJsonData.EffectType.Speedup:
                return new DropSpeedup(data, spellData);
            case DropSpellJsonData.EffectType.Circle:
                return new DropCircle(data, spellData);
        }
        throw new System.Exception("the drop type is not found: " + spellData.MyEffectType);
    }
}