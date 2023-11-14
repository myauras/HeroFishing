using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using UnityEngine.UI;
using System;
using TMPro;

namespace HeroFishing.Main {

    public class MapItem : MonoBehaviour, IItem {
        [SerializeField] TextMeshProUGUI NameText;
        [SerializeField] Image MapImg;
        [SerializeField] Button EnterBtn;

        public bool IsActive { get; set; }


        public DBMap MyDBMap { get; private set; }
        public MapJsonData MyJsonMap { get; private set; }

        public void Init(DBMap _data) {
            MyDBMap = _data;
            MyJsonMap = MapJsonData.GetData(MyDBMap.JsonMapID);
            RefreshItem();
        }


        void RefreshItem() {
            NameText.text = MyJsonMap.Name;
            AddressablesLoader.GetSpriteAtlas("MapUI", atlas => {
                MapImg.sprite = atlas.GetSprite(MyJsonMap.Ref);
            });
        }

        public void OnClick() {
            LobbySceneUI.Instance.SwitchUI(LobbySceneUI.LobbyUIs.Hero);
        }
    }
}
