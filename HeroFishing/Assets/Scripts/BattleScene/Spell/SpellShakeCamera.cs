using Cysharp.Threading.Tasks;
using DG.Tweening;
using HeroFishing.Main;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellShakeCamera
{
    private float _delay;
    private float _amplitude;
    private float _frequency;
    private float _duartion;
    private HeroSpellJsonData _data;
    public SpellShakeCamera(HeroSpellJsonData data) {
        _delay = data.CameraShakeSettings[0];
        _amplitude = data.CameraShakeSettings[1];
        _frequency = data.CameraShakeSettings[2];
        _duartion = data.CameraShakeSettings[3];

        _data = data;
    }

    public async void Play() {
        if (_delay == 0)
            CamManager.ShakeCam(CamManager.CamNames.Battle, _amplitude, _frequency, _duartion);
        else {
            await UniTask.WaitForSeconds(_delay);
            CamManager.ShakeCam(CamManager.CamNames.Battle, _amplitude, _frequency, _duartion);
        }            
    }
}
