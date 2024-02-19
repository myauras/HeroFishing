using HeroFishing.Battle;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class RankUI : BaseUI {
    [SerializeField]
    private RankItemUI[] _rankItems;

    private int _playerCount;
    public int PlayerCount => _playerCount;
    private int[] _currentPlayerIdxs;
    public override void RefreshText() {

    }

    public override void Init() {
        base.Init();
        for (int i = 0; i < _rankItems.Length; i++) {
            _rankItems[i].Init();
            _rankItems[i].SetActive(false);
        }

        BattleManager.Instance.OnHeroAdd += AddHero;
        BattleManager.Instance.OnHeroRemove += RemoveHero;

        var id = BattleManager.Instance.GetHero(0).MyData.ID;
        AddHero(id, BattleManager.Instance.Index);
    }

    public void AddHero(int heroID, int playerIdx) {
        _playerCount++;
        bool isPlayer = BattleManager.Instance.Index == playerIdx;
        for (int i = 0; i < _rankItems.Length; i++) {
            var item = _rankItems[i];
            if (item.PlayerIdx == -1) {
                item.SetContent(heroID, playerIdx, isPlayer);
                item.SetActive(true);
                break;
            }
        }
    }

    public void RemoveHero(int playerIdx) {
        _playerCount--;
        bool found = false;
        for (int i = 0; i < _rankItems.Length; i++) {
            var item = _rankItems[i];
            // 先找離開的player idx
            if (item.PlayerIdx == playerIdx) {
                found = true;
            }
            // 找到後，如果它後面的順位有資料，往前一名移動
            if (found) {
                if (i < _rankItems.Length - 1 && _rankItems[i + 1].PlayerIdx != -1) {
                    var nextItem = _rankItems[i + 1];
                    item.SetContent(nextItem);
                }
            }
        }

        // 最後的那個排名關閉
        _rankItems[_playerCount].ResetContent();
        _rankItems[_playerCount].SetActive(false);
    }

    public void SetRank(int[] playerIdxs) {
        bool firstSwap = false;
        for (int i = 0; i < _rankItems.Length - 1; i++) {
            int index = i;
            var item = _rankItems[i];
            var itemPlayerIndex = item.PlayerIdx;
            if (itemPlayerIndex == -1) continue;
            if (itemPlayerIndex == playerIdxs[i]) continue;
            if (!firstSwap) {
                item.PlayAnimation();
                firstSwap = true;
            }

            Observable.ReturnUnit().SkipWhile(_ => RankItemUI.s_IsChanging).Subscribe(_ => {
                for (int j = index + 1; j < _rankItems.Length; j++) {
                    var swapItem = _rankItems[j];
                    if (swapItem.PlayerIdx == playerIdxs[index]) {
                        var heroID = swapItem.HeroID;
                        var playerIndex = swapItem.PlayerIdx;
                        var isPlayer = swapItem.IsPlayer;
                        swapItem.SetContent(item);
                        item.SetContent(heroID, playerIndex, isPlayer);
                        break;
                    }
                }
            });
        }
    }

    //public void AddHeroTest() {
    //    int[] allIdxs = new int[] { 0, 1, 2, 3 };
    //    var idxs = _rankItems.Select(item => item.PlayerIdx).ToArray();
    //    var exceptIdxs = allIdxs.Except(idxs).ToArray();
    //    var idx = exceptIdxs[Random.Range(0, exceptIdxs.Length)];
    //    int[] heroIDs = new int[] { 1, 2, 7, 19 };
    //    int heroID = heroIDs[idx];
    //    AddHero(heroID, idx);
    //    Debug.Log("possible idxs " + string.Join(", ", exceptIdxs));
    //    Debug.Log("add idx " + idx);
    //}

    //public void RemoveRandomHeroTest() {
    //    var idxs = _rankItems.Where(item => item.PlayerIdx != -1).Select(item => item.PlayerIdx).ToArray();
    //    var idx = idxs[Random.Range(0, idxs.Length)];
    //    RemoveHero(idx);
    //    Debug.Log("remove idx " + idx);
    //}

    //public void SetRankTest() {
    //    var idxs = _rankItems.Where(item => item.PlayerIdx != -1).Select(item => item.PlayerIdx).ToList();
    //    Debug.Log("before: " + string.Join(", ", idxs));
    //    idxs.Shuffle();
    //    Debug.Log("after: " + string.Join(", ", idxs));
    //    SetRank(idxs.ToArray());
    //}
}
