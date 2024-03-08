using HeroFishing.Battle;
using HeroFishing.Main;
using HeroFishing.Socket;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : BaseUI {
    [SerializeField]
    private PlayerInfoItemUI[] _playerInfoItems;

    public override void RefreshText() {

    }

    public override void Init() {
        base.Init();
        for (int i = 0; i < _playerInfoItems.Length; i++) {
            _playerInfoItems[i].Init();
            if (i != 0) {
                _playerInfoItems[i].SetActive(false);
            }
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

    private void AddHero(int heroID, int playerIndex) {
        int heroIndex = BattleManager.Instance.GetHeroIndex(playerIndex);
        _playerInfoItems[heroIndex].SetActive(true);
        _playerInfoItems[heroIndex].SetImage(heroID);
    }

    private void RemoveHero(int playerIndex) {
        int heroIndex = BattleManager.Instance.GetHeroIndex(playerIndex);
        _playerInfoItems[heroIndex].SetActive(false);
        _playerInfoItems[heroIndex].Clear();
    }
}
