using FancyScrollView;
using HeroFishing.Main;
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
    public Vector2 Position;
    public DBMap dbMap;
}

public class Context {
    public Action<int> OnClick;
    public int SelectedIndex;
    public DBMap SelectedMap;
}

public class MapScrollView : FancyScrollView<MapItemData, Context> {
    [SerializeField]
    private Scroller _scroller;
    [SerializeField]
    private AssetReference _prefabRef;

    private GameObject _prefab;

    protected override GameObject CellPrefab => _prefab;

    public DBMap SelectedMap => Context.SelectedMap;

    protected override void Initialize() {
        base.Initialize();

        _scroller.OnValueChanged(UpdatePosition);
        _scroller.OnSelectionChanged(UpdateSelection);

        Context.OnClick += SelectCell;
    }

    private void UpdateSelection(int index) {
        if (Context.SelectedIndex == index) return;

        Context.SelectedIndex = index;
        Context.SelectedMap = ItemsSource[index].dbMap;
        Refresh();
    }

    public void ResetPos() {
        Refresh();
        var position = _scroller.Position;
        GlassController.Instance.Select(Mathf.Abs(Mathf.Round(position) - position) < 0.001f);
    }

    public void UpdateData(IList<MapItemData> itemData) {
        if (_prefab == null) {
            Addressables.LoadAssetAsync<GameObject>(_prefabRef).Completed += handle => {
                _prefab = handle.Result;
                UpdateContents(itemData);
            };
        } else {
            UpdateContents(itemData);
        }
    }

    protected override void UpdateContents(IList<MapItemData> itemsSource) {
        base.UpdateContents(itemsSource);
        _scroller.SetTotalCount(itemsSource.Count);
        Context.SelectedMap = itemsSource[Context.SelectedIndex].dbMap;

    }

    protected override void UpdatePosition(float position) {
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
