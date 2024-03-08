using HeroFishing.Main;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapPlayerInfoUI : BaseUI {
    [SerializeField]
    private Text _txtName;
    [SerializeField]
    private Text _txtLv;
    [SerializeField]
    private TextMeshProUGUI _txtCoin;
    [SerializeField]
    private TextMeshProUGUI _txtTime;
    private int _points;
    
    public override void RefreshText() {
        
    }

    public override void Init() {
        base.Init();
        Refresh();
    }

    public void Refresh() {
        var dbPlayer = GamePlayer.Instance.GetDBPlayerDoc<DBPlayer>(Service.Realms.DBPlayerCol.player);
        SetPoints((int)(dbPlayer.Point ?? 0));
    }

    public void AddPoints(int points) {
        _points += points;
        _txtCoin.text = _points.ToString();
    }

    public void SetPoints(int points) {
        _points = points;
        _txtCoin.text = _points.ToString();
    }
}
