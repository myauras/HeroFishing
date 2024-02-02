using HeroFishing.Main;
using Scoz.Func;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DropBtn : MonoBehaviour {
    [SerializeField]
    private GameObject _goDrop;
    [SerializeField]
    private GameObject _goDuration;
    [SerializeField]
    private TextMeshProUGUI _txtDuration;
    [SerializeField]
    private Image _imgDrop;

    private int _dropID = -1;
    public bool IsEmpty => _dropID == -1;
    public int DropID => _dropID;
    private const string PATH_DROP_ICON = "DropIcon";

    public void Init() {
        _goDrop.SetActive(false);
        _goDuration.SetActive(false);
    }

    public void SetContent(int dropID) {
        _dropID = dropID;
        _goDrop.SetActive(true);
        AddressablesLoader.GetSpriteAtlas(PATH_DROP_ICON, atlas => {
            _imgDrop.sprite = atlas.GetSprite("Icon_" + dropID.ToString());
        });
    }

    public void Play(float duration = 0) {
        _dropID = -1;
        if (duration > 0) {
            _goDuration.SetActive(true);
            _txtDuration.text = duration.ToString();
            float timer = duration;
            Observable.EveryUpdate().TakeUntil(Observable.Timer(TimeSpan.FromSeconds(duration))).TimeInterval().Subscribe(timeInterval => {
                var deltaTime = timeInterval.Interval;
                timer -= (float)deltaTime.TotalSeconds;
                if(timer > 1) {
                    _txtDuration.text = Mathf.Floor(timer).ToString();
                }
                else {
                    _txtDuration.text = timer.ToString("F2");
                }
            }, () => {
                _goDuration.SetActive(false);
                _goDrop.SetActive(false);
                _imgDrop.sprite = null;
            });
        }
        else {
            _goDrop.SetActive(false);
            _imgDrop.sprite = null;
        }
    }

    public void Press() {

    }
}
