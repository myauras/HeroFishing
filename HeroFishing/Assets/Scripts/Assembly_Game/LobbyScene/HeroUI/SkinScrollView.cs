using FancyScrollView;
using HeroFishing.Main;
using Scoz.Func;
using Service.Realms;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SkinItemData {
    public HeroSkinJsonData MyJsonSkin { get; set; }
}

public class SkinScrollContext {
    public Action<int> OnClick;
    public int SelectedIndex;
    public HeroSkinJsonData SelectedJsonSkin;
}

public class SkinScrollView : FancyScrollView<SkinItemData, SkinScrollContext> {
    [SerializeField] Scroller _scroller;
    [SerializeField] AssetReference _prefabRef;
    [SerializeField] GameObject RightArrow;
    [SerializeField] GameObject LeftArrow;
    GameObject _prefab;

    protected override GameObject CellPrefab => _prefab;
    int CurItemIdx;
    public HeroSkinJsonData SelectedSkinJsonData => Context.SelectedJsonSkin;

    public void RefreshScrollView(int _heroID) {
        ResetPos();
        var skinItemList = new List<SkinItemData>();
        var skinDic = HeroSkinJsonData.GetSkinDic(_heroID);
        foreach (var skinData in skinDic.Values) {
            var itemData = new SkinItemData() {
                MyJsonSkin = skinData,
            };
            skinItemList.Add(itemData);
        }
        UpdateData(skinItemList);
    }


    protected override void Initialize() {
        base.Initialize();

        _scroller.OnValueChanged(UpdatePosition);
        _scroller.OnSelectionChanged(UpdateSelection);

        Context.OnClick += SelectCell;
    }


    private void UpdateSelection(int index) {
        if (Context.SelectedIndex == index) return;

        Context.SelectedIndex = index;
        Context.SelectedJsonSkin = ItemsSource[index].MyJsonSkin;
        Refresh();
    }

    public void ResetPos() {
        Refresh();
        CurItemIdx = 0;
        SelectCell(CurItemIdx);
    }
    public void LoadItemAsset(Action _cb = null) {
        if (_prefab != null) {
            _cb?.Invoke();
            return;
        }
        Addressables.LoadAssetAsync<GameObject>(_prefabRef).Completed += handle => {
            _prefab = handle.Result;
            _cb?.Invoke();
        };
    }

    public void UpdateData(IList<SkinItemData> itemData) {
        if (_prefab == null) {
            WriteLog.LogError("SkinItem AssetReference尚未載入");
            return;
        }
        UpdateContents(itemData);
    }
    protected override void UpdateContents(IList<SkinItemData> itemsSource) {
        base.UpdateContents(itemsSource);
        _scroller.SetTotalCount(itemsSource.Count);
        Context.SelectedJsonSkin = itemsSource[Context.SelectedIndex].MyJsonSkin;
    }

    protected override void UpdatePosition(float position) {
        base.UpdatePosition(position);
    }
    public void GoNext() {
        CurItemIdx++;
        if (CurItemIdx >= ItemsSource.Count) CurItemIdx = ItemsSource.Count - 1;
        SelectCell(CurItemIdx);
    }
    public void GoPrevious() {
        CurItemIdx--;
        if (CurItemIdx < 0) CurItemIdx = 0;
        SelectCell(CurItemIdx);
    }

    private void SelectCell(int index) {
        RightArrow.SetActive(CurItemIdx < (ItemsSource.Count - 1));
        LeftArrow.SetActive(CurItemIdx > 0);
        if (index < 0 || index >= ItemsSource.Count || index == Context.SelectedIndex) {
            return;
        }
        UpdateSelection(index);
        _scroller.ScrollTo(index, 0.35f, EasingCore.Ease.OutCubic);
        HeroUI.GetInstance<HeroUI>()?.SwitchHeroSkin(Context.SelectedJsonSkin);
    }
}
