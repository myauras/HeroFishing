using HeroFishing.Main;
using Scoz.Func;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class MapPlayerInfoUI : BaseUI {
    [SerializeField]
    private Text _txtName;
    [SerializeField]
    private Text _txtLv;
    [SerializeField]
    private Text _txtTime;
    [SerializeField]
    private TextMeshProUGUI _txtCoin;

    private int _points;

    private const string TIME_ZONE_ID = "Taipei Standard Time";

    public override void RefreshText() {
        
    }

    public override void Init() {
        base.Init();
        Refresh();
    }

    public void Refresh() {
        var dbPlayer = GamePlayer.Instance.GetDBPlayerDoc<DBPlayer>(Service.Realms.DBPlayerCol.player);
        SetPoints((int)(dbPlayer.Point ?? 0));
        SetLevel(dbPlayer.HeroExp ?? 0);
        SetDateCountdown();
    }

    public void AddPoints(int points) {
        _points += points;
        _txtCoin.text = _points.ToString();
    }

    public void SetPoints(int points) {
        _points = points;
        _txtCoin.text = _points.ToString();
    }

    public void SetLevel(int exp) {
        int remainingExp = exp;
        int level = 1;
        for (int i = 1; i < 10; i++) {
            var levelExp = HeroEXPJsonData.GetData(i).EXP;
            if (remainingExp >= levelExp) {
                level++;
                remainingExp -= levelExp;
            }
            else break;
        }
        _txtLv.text = "Lv." + level.ToString();
    }

    public void SetDateCountdown() {
        Observable.EveryUpdate().TakeUntilDisable(this).Subscribe(_ => {
            TimeZoneInfo tst = TimeZoneInfo.FindSystemTimeZoneById(TIME_ZONE_ID);
            DateTime now = TimeZoneInfo.ConvertTime(DateTime.Now, tst);
            DateTime endDate = now.AddDays(1).Date;
            var remainingTime = endDate.Subtract(now);
            _txtTime.text = $"{remainingTime.Hours:00}:{remainingTime.Minutes:00}:{remainingTime.Seconds:00}";
        });
    }
}
