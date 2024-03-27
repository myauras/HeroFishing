using HeroFishing.Battle;
using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBannerUI : BaseUI
{
    [SerializeField]
    private Animator _animator;
    public override void RefreshText() {
        Debug.Log("boss banner refresh text");
    }

    public override void Init() {
        base.Init();
        BattleManager.Instance.OnBossSpawn += SpawnBoss;        
    }

    public void SpawnBoss() {
        _animator.SetTrigger("Trigger");
    }

    public void Shake() {
        var amp = GameSettingJsonData.GetFloat(GameSetting.CamShake_BossDebut_AmplitudeGain);
        var freq = GameSettingJsonData.GetFloat(GameSetting.CamShake_BossDebut_FrequencyGain);
        var duration = GameSettingJsonData.GetFloat(GameSetting.CamShake_BossDebut_Duration);
        CamManager.ShakeCam(CamManager.CamNames.Battle, amp, freq, duration);
    }
}
