using HeroFishing.Battle;
using HeroFishing.Main;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using UnityEngine.UIElements;

public class CoinEffectUI : BaseUI, IUpdate {
    public class CoinEffectItemData {
        public string key;
        public Vector3 position;
        public int points;
    }

    private const string LARGE_COIN_EFFECT = "OtherEffect/Script_UI_Group_Coin_Big";
    private const string MID_COIN_EFFECT = "OtherEffect/Script_UI_Group_Coin_M";
    private const string SMALL_COIN_EFFECT = "OtherEffect/Script_UI_Group_Coin_S";
    private Canvas _canvas;
    private Queue<CoinEffectItemData> _queue;

    public int Order => 0;

    public override void RefreshText() {

    }

    protected override void OnDestroy() {
        base.OnDestroy();
        UpdateSystem.Instance.UnregisterUpdate(this);
    }

    public override void Init() {
        base.Init();
        _canvas = GetComponent<Canvas>();
        var hero = BattleManager.Instance.GetHero(0);
        hero.OnPointGet += OnPointGet;
        _queue = new Queue<CoinEffectItemData>();
        UpdateSystem.Instance.RegisterUpdate(this);
    }

    private void OnPointGet(int monsterIdx, int points) {
        if (Monster.TryGetMonsterByIdx(monsterIdx, out Monster monster)) {
            string key = GetCoinEffectKey(monster.MyData.MyMonsterSize);
            var monsterPos = monster.transform.position;
            monsterPos.y = GameSettingJsonData.GetFloat(GameSetting.Bullet_PositionY);
            var screenPosition = BattleManager.Instance.BattleCam.WorldToScreenPoint(monsterPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, screenPosition, _canvas.worldCamera, out var position);
            _queue.Enqueue(new CoinEffectItemData { key = key, position = position, points = points });
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

    public void OnUpdate(float deltaTime) {
        if (_queue.TryDequeue(out var data)) {
            PoolManager.Instance.Pop(data.key, data.position, Quaternion.identity, transform, go => {
                var item = go.GetComponent<CoinEffectItemUI>();
                item.SetPoints(data.points);
            });
        }
    }
}
