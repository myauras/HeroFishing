using HeroFishing.Main;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoItemUI : MonoBehaviour {
    [SerializeField]
    private Image _img;
    [SerializeField]
    private Text _txtName;
    [SerializeField]
    private GameObject[] _activeObjs;

    public void Init() {

    }

    public void SetActive(bool active) {
        for (int i = 0; i < _activeObjs.Length; i++) {
            _activeObjs[i].SetActive(active);
        }
    }

    public void SetImage(int heroID) {
        var data = HeroJsonData.GetData(heroID);
        AddressablesLoader.GetSpriteAtlas("HeroIcon", atlas => {
            _img.sprite = atlas.GetSprite(data.Ref);
        });
    }

    public void Clear() {
        _txtName.text = string.Empty;
        _img.sprite = null;
    }
}
