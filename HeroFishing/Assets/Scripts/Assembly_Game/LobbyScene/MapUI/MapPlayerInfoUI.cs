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
    private const string INVALID_TIME = "--:--:--";

    public override void RefreshText() {
        
    }

    public override void Init() {
        base.Init();
        Refresh();
    }

    public void Refresh() {
        var dbPlayer = GamePlayer.Instance.GetDBPlayerDoc<DBPlayer>(Service.Realms.DBPlayerCol.player);
        Debug.Log($"left game at: {dbPlayer.LeftGameAt.Value}");
        for (int i = 0; i < dbPlayer.SpellLVs.Count; i++) {
            Debug.Log($"spell level {i}: {dbPlayer.SpellLVs[i]}");
        }

        for (int i = 0; i < dbPlayer.SpellCharges.Count; i++) {
            Debug.Log($"spell charge {i}: {dbPlayer.SpellCharges[i]}");
        }

        for (int i = 0; i < dbPlayer.Drops.Count; i++) {
            Debug.Log($"drops {i}: {dbPlayer.Drops[i]}");
        }

        SetPoints((int)(dbPlayer.Point ?? 0));
        SetLevel(dbPlayer.HeroExp ?? 0);
        SetDateCountdown(dbPlayer.LeftGameAt);
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

    public void SetDateCountdown(DateTimeOffset? leftGameAt) {
        if (!leftGameAt.HasValue) {
            _txtTime.text = INVALID_TIME;
            return;
        }

        Observable.EveryUpdate().TakeUntilDisable(this).Subscribe(_ => {
            var resetLevelAt = leftGameAt.Value.AddDays(1);
            //TimeZoneInfo tst = TimeZoneInfo.FindSystemTimeZoneById(TIME_ZONE_ID);
            //DateTime now = TimeZoneInfo.ConvertTime(DateTime.Now, tst);
            //DateTime endDate = now.AddDays(1).Date;
            var remainingTime = resetLevelAt.Subtract(DateTime.UtcNow);
            if (remainingTime.Ticks < 0) {
                _txtTime.text = INVALID_TIME;
            }
            else {
                _txtTime.text = $"{remainingTime.Hours:00}:{remainingTime.Minutes:00}:{remainingTime.Seconds:00}";
            }
        });
    }
}
