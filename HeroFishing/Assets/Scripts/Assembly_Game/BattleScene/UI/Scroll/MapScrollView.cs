using FancyScrollView;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class MapItemData {
    public int Id;
    public string Name;
    public int Bet;
    public bool IsGradient;
    public Color glowColor;
    public Color txtColor;
}

public class Context {
    public Action<int> OnClick;
    public int SelectedIndex;
}

public class MapScrollView : FancyScrollView<MapItemData, Context> {
    [SerializeField]
    private Scroller _scroller;
    [SerializeField]
    private AssetReference _prefabRef;

    private GameObject _prefab;

    protected override GameObject CellPrefab => _prefab;

    protected override void Initialize() {
        base.Initialize();

        _scroller.OnValueChanged(UpdatePosition);
        _scroller.OnSelectionChanged(UpdateSelection);
    }

    private void UpdateSelection(int index) {
        if (Context.SelectedIndex == index) return;

        Context.OnClick += SelectCell;

        Context.SelectedIndex = index;
        Refresh();
    }

    public void UpdateData(IList<MapItemData> itemData) {
        if (_prefab == null) {
            Addressables.LoadAssetAsync<GameObject>(_prefabRef).Completed += handle => {
                _prefab = handle.Result;
                UpdateContents(itemData);
                _scroller.SetTotalCount(itemData.Count);
            };
        }
        else {
            UpdateContents(itemData);
            _scroller.SetTotalCount(itemData.Count);
        }
    }

    private void SelectCell(int index) {
        if (index < 0 || index >= ItemsSource.Count || index == Context.SelectedIndex) {
            return;
        }

        UpdateSelection(index);
        _scroller.ScrollTo(index, 0.35f, EasingCore.Ease.OutCubic);
    }

    private void LateUpdate() { }
}
