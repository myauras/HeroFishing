using HeroFishing.Battle;
using HeroFishing.Main;
using HeroFishing.Socket;
using Scoz.Func;
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

        if (!GameConnector.Connected) {
            var id = BattleManager.Instance.GetHero(0).MyData.ID;
            AddHero(id, BattleManager.Instance.Index);
        }
        else {
            if (AllocatedRoom.Instance.CurGameState == AllocatedRoom.GameState.Playing)
                if (AllocatedRoom.Instance.HeroIDs != null && AllocatedRoom.Instance.HeroIDs.Length > 0) {
                    for (int i = 0; i < AllocatedRoom.Instance.HeroIDs.Length; i++) {
                        int id = AllocatedRoom.Instance.HeroIDs[i];
                        int index = i;
                        AddHero(id, index);
                    }
                }
                else {
                    AddHero(AllocatedRoom.Instance.MyHeroID, AllocatedRoom.Instance.Index);
                }
        }
    }

    public void AddHero(int heroID, int playerIdx) {
        //Debug.Log("add hero: " + playerIdx);
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
        //Debug.Log("remove hero: " + playerIdx);
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
        //Debug.Log("set rank: " + string.Join(", ", playerIdxs));
        bool firstSwap = false;
        for (int i = 0; i < _rankItems.Length - 1; i++) {
            int index = i;
            var item = _rankItems[i];
            var itemPlayerIndex = item.PlayerIdx;
            //Debug.Log(i + " item player index: " + itemPlayerIndex);
            if (itemPlayerIndex == -1) continue;
            //Debug.Log(i + " cur player index: " + playerIdxs[i]);
            if (itemPlayerIndex == playerIdxs[i]) continue;
            if (!firstSwap) {
                //Debug.Log(i + " play animation");
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
}
