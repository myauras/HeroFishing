using FancyScrollView;
using HeroFishing.Main;
using Scoz.Func;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SkinItemData {
    public HeroSkinJsonData MyJsonSkin { get; private set; }
}

public class SkinScrollContext {
    public Action<int> OnClick;
    public int SelectedIndex;
    public HeroSkinJsonData SelectedJsonSkin;
}

public class SkinScrollView : FancyScrollView<SkinItemData, SkinScrollContext> {
    [SerializeField]
    private Scroller _scroller;
    [SerializeField]
    private AssetReference _prefabRef;

    private GameObject _prefab;

    protected override GameObject CellPrefab => _prefab;

    public HeroSkinJsonData SelectedSkinJsonData => Context.SelectedJsonSkin;

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
    }

    public void UpdateData(IList<SkinItemData> itemData) {
        if (_prefab == null) {
            Addressables.LoadAssetAsync<GameObject>(_prefabRef).Completed += handle => {
                _prefab = handle.Result;
                UpdateContents(itemData);
            };
        } else {
            UpdateContents(itemData);
        }
    }

    protected override void UpdateContents(IList<SkinItemData> itemsSource) {
        base.UpdateContents(itemsSource);
        _scroller.SetTotalCount(itemsSource.Count);
        Context.SelectedJsonSkin = itemsSource[Context.SelectedIndex].MyJsonSkin;
    }

    protected override void UpdatePosition(float position) {
        WriteLog.LogError("position=" + position);
        base.UpdatePosition(position);
        GlassController.Instance.Select(Mathf.Abs(Mathf.Round(position) - position) < 0.001f);
    }

    private void SelectCell(int index) {
        if (index < 0 || index >= ItemsSource.Count || index == Context.SelectedIndex) {
            return;
        }

        UpdateSelection(index);
        _scroller.ScrollTo(index, 0.35f, EasingCore.Ease.OutCubic);
    }
}
