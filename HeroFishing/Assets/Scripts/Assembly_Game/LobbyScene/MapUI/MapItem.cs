using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using UnityEngine.UI;
using System;
using TMPro;
using FancyScrollView;
using JoshH.UI;

namespace HeroFishing.Main {

    public class MapItem : FancyCell<MapItemData, Context> {
        [SerializeField]
        private Text _txtName;
        [SerializeField]
        private CustomOutline _outlineName;
        [SerializeField]
        private TextMeshProUGUI _txtOnlineCount;
        [SerializeField]
        private Image _imgFrame;
        [SerializeField]
        private Image _imgBackground;
        [SerializeField]
        private Image _imgForeground;
        [SerializeField]
        private Image _imgGlow;
        [SerializeField]
        private Image _imgBet;
        [SerializeField]
        private Animator _animator;
        [SerializeField, Range(0, 0.2f)]
        private float _hightlightDistance = 0.1f;
        [SerializeField]
        private Material _hightlightMatPrefab;
        [SerializeField]
        private Material _glowMatPrefab;

        private Material _hightlightMat;
        private Material _glowMat;
        

        //[SerializeField] Image MapImg;
        [SerializeField] Button _btnSelect;


        public DBMap MyDBMap { get; private set; }
        public MapJsonData MyJsonMap { get; private set; }

        private float _currentPosition = 0;
        private const string PARAM_HIGHTLIGHT = "_HightLight";

        //private void OnEnable() {
        //    UpdatePosition(_currentPosition);
        //}

        private void Start() {
            _btnSelect.onClick.AddListener(() => Context.OnClick?.Invoke(Index));
        }

        public override void Initialize() {
            base.Initialize();
            _hightlightMat = new Material(_hightlightMatPrefab);
            _glowMat = new Material(_glowMatPrefab);

            _imgBackground.material = _hightlightMat;
            _imgForeground.material = _hightlightMat;
            _imgBet.material = _hightlightMat;
            _imgFrame.material = _hightlightMat;
            _imgGlow.material = _glowMat;
        }

        public override void UpdateContent(MapItemData itemData) {
            _txtName.text = itemData.Name;
            int id = itemData.Id;
            int bet = itemData.Bet;

            MyJsonMap = MapJsonData.GetData(id);
            AddressablesLoader.GetSpriteAtlas("MapItem", atlas => {
                _imgBackground.sprite = atlas.GetSprite($"Img_Map_Bg0{id}");
                _imgFrame.sprite = atlas.GetSprite($"Img_Container_{bet}");
                _imgBet.sprite = atlas.GetSprite($"Img_Map_Bet{bet}");
            });

            _imgGlow.GetComponent<UIGradient>().enabled = itemData.IsGradient;
            _imgGlow.color = itemData.glowColor;

            _txtName.color = itemData.txtColor;
            var outlineColor = itemData.txtColor * 0.05f;
            outlineColor.a = 1;
            _outlineName.OutlineColor = outlineColor;

            if (Context.SelectedIndex == Index) {
                Debug.Log("selected " + Index);
            }
        }

        public override void UpdatePosition(float position) {
            _currentPosition = position;

            if (_animator.isActiveAndEnabled) {
                _animator.Play("scroll", -1, position);
            }

            _animator.speed = 0;

            SetHightlight(position);
        }

        private void SetHightlight(float position) {
            float value = Mathf.Clamp01(1 - Mathf.Abs(0.5f - position) / _hightlightDistance);
            _hightlightMat.SetFloat(PARAM_HIGHTLIGHT, value);
            _glowMat.SetFloat(PARAM_HIGHTLIGHT, value);
        }
    }
}
