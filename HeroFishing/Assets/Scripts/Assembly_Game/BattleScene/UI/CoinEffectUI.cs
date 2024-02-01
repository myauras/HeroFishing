using HeroFishing.Battle;
using HeroFishing.Main;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CoinEffectUI : BaseUI {

    private const string LARGE_COIN_EFFECT = "OtherEffect/Script_UI_Group_Coin_Big";
    private const string MID_COIN_EFFECT = "OtherEffect/Script_UI_Group_Coin_M";
    private const string SMALL_COIN_EFFECT = "OtherEffect/Script_UI_Group_Coin_S";
    private Canvas _canvas;
    public override void RefreshText() {

    }

    public override void Init() {
        base.Init();
        _canvas = GetComponentInParent<Canvas>();
        var hero = BattleManager.Instance.GetHero(0);
        hero.OnPointGet += OnPointGet;
    }

    private void OnPointGet(int monsterIdx, int points) {
        if (Monster.TryGetMonsterByIdx(monsterIdx, out Monster monster)) {
            string key = GetCoinEffectKey(monster.MyData.MyMonsterSize);
            var monsterPos = monster.transform.position;
            monsterPos.y = GameSettingJsonData.GetFloat(GameSetting.Bullet_PositionY);
            var screenPosition = BattleManager.Instance.BattleCam.WorldToScreenPoint(monsterPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, screenPosition, _canvas.worldCamera, out var position);
            PoolManager.Instance.Pop(key, position, Quaternion.identity, transform, go => {
                var item = go.GetComponent<CoinEffectItemUI>();
                item.SetPoints(points);
            });

        }
    }

    private string GetCoinEffectKey(MonsterJsonData.MonsterSize size) {
        switch (size) {
            case MonsterJsonData.MonsterSize.Small:
                return SMALL_COIN_EFFECT;
            case MonsterJsonData.MonsterSize.Mid:
                return MID_COIN_EFFECT;
            case MonsterJsonData.MonsterSize.Large:
                return LARGE_COIN_EFFECT;
            default:
                break;
        }
        throw new System.Exception("no coin effect key in size: " + size);
    }
}
