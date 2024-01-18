using HeroFishing.Battle;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class BattleSceneUI : BaseUI {
    [HeaderAttribute("==============AddressableAssets==============")]
    [SerializeField] AssetReference BattleManagerAsset;


    [Header("Settings")]
    private bool _isSpellTest;

    [HeaderAttribute("==============UI==============")]
    [SerializeField]
    private SpellUI _spellUI;
    [SerializeField]
    private LevelUI _levelUI;
    [SerializeField]
    private DeviceInfoUI _deviceInfoUI;

    private void Start() {
        Init();
    }
    public override void Init() {
        base.Init();
        SpawnBattleManager();
    }

    void SpawnBattleManager() {
        AddressablesLoader.GetPrefabByRef(BattleManagerAsset, (battleManagerPrefab, handle) => {
            GameObject go = Instantiate(battleManagerPrefab);
            var battleMaanger = go.GetComponent<BattleManager>();
            battleMaanger.Init();
            _spellUI.Init();
            _levelUI.Init();
            //_deviceInfoUI.Init();
        });
    }

    public override void RefreshText() {

    }
}
