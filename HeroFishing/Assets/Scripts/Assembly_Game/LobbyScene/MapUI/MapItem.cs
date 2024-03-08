using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using UnityEngine.UI;
using System;
using TMPro;
using FancyScrollView;
using JoshH.UI;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;

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

        private bool _hasAnimation;

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

        public override async void UpdateContent(MapItemData itemData) {
            _txtName.text = itemData.Name;
            int id = itemData.Id;
            int bet = itemData.Bet;

            MyJsonMap = MapJsonData.GetData(id);
            string str_id = id.ToString("d2");

            // 檢查是否有動畫
            string animationPath = $"Assets/AddressableAssets/Animations/MapMonsters/Map_{str_id}.controller";
            _hasAnimation = HasKey(animationPath, typeof(RuntimeAnimatorController));

            // 加入圖片
            AddressablesLoader.GetSpriteAtlas("MapItem", atlas => {
                _imgBackground.sprite = atlas.GetSprite($"Img_Map_Bg{str_id}");
                _imgForeground.sprite = atlas.GetSprite($"Img_Map_Fg{str_id}");
                _imgForeground.SetNativeSize();
                _imgForeground.rectTransform.anchoredPosition = itemData.Position;
                _imgFrame.sprite = atlas.GetSprite($"Img_Container_{bet}");
                _imgBet.sprite = atlas.GetSprite($"Img_Map_Bet{bet}");
            });

            // 如果需要漸層色，則開啟漸層色
            _imgGlow.GetComponent<UIGradient>().enabled = itemData.IsGradient;
            _imgGlow.color = itemData.glowColor;

            // 替換文字顏色
            _txtName.color = itemData.txtColor;
            var outlineColor = itemData.txtColor * 0.05f;
            outlineColor.a = 1;
            _outlineName.OutlineColor = outlineColor;

            // 開啟動畫
            var animator = _imgForeground.GetComponent<Animator>();
            animator.enabled = false;
            if (_hasAnimation) {
                var handle = Addressables.LoadAssetAsync<RuntimeAnimatorController>(animationPath);
                var ac = await handle.Task;
                animator.runtimeAnimatorController = ac;
                animator.enabled = Context.SelectedIndex == Index;
            }
            //if (Context.SelectedIndex == Index) {
            //    Debug.Log("selected " + Index);
            //}
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

        private static bool HasKey(object key, Type type) {
            foreach (var l in Addressables.ResourceLocators) {
                if (l.Locate(key, type, out var loc)) {
                    return true;
                }
            }
            return false;
        }
    }
}
