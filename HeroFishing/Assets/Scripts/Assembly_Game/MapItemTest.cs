using Scoz.Func;
using UnityEngine;
using UnityEngine.UI;

public class MapItemTest : MonoBehaviour {
    [SerializeField]
    private int _bet = 1;
    [SerializeField]
    private int _id = 1;
    [SerializeField]
    private Text _txtName;
    [SerializeField]
    private CustomOutline _outlineName;
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
    private string[] _colorStrs = new string[] { "A39C9C", "8A5D4D", "AB8DBD", "b9904e", "00b4b3", "ffcd28", "fa41ff", "87ffff" };
    private string[] _txtColorStrs = new string[] { "9C9897", "be866d", "bbc1cf", "e7c266", "03fbfb", "ffbc06", "ff42ad", "c9cafc" };

    private void OnValidate() {
        //if (!Application.isPlaying) return;
        AddressablesLoader.GetSpriteAtlas("MapItem", atlas => {
            _imgBackground.sprite = atlas.GetSprite($"Img_Map_Bg0{_id}");
            _imgFrame.sprite = atlas.GetSprite($"Img_Container_{_bet}");
            if (_bet == 1) {
                _imgBet.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                _imgBet.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                _imgBet.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                _imgBet.rectTransform.anchoredPosition = new Vector2(0, _imgBet.rectTransform.anchoredPosition.y);
            }
            _imgBet.sprite = atlas.GetSprite($"Img_Map_Bet{_bet}");

            //Debug.Log(_colorStrs[_id - 1]);
            //Debug.Log(ColorUtility.TryParseHtmlString("#" + _colorStrs[_id - 1], out Color vcolor));
            if (ColorUtility.TryParseHtmlString("#" + _colorStrs[_id - 1], out Color color)) {
                _imgGlow.color = color;
            }

            if (ColorUtility.TryParseHtmlString("#" + _txtColorStrs[_id - 1], out Color txtColor)) {
                _txtName.color = txtColor;
                var outlineColor = txtColor * 0.05f;
                outlineColor.a = 1;
                _outlineName.OutlineColor = outlineColor;
            }
        });
    }
}
