using DG.Tweening;
using HeroFishing.Main;
using Scoz.Func;
using System;
using UniRx;
using UnityEngine;

namespace HeroFishing.Battle {
    public class MonsterSpecialize : MonoBehaviour {
        [SerializeField] ParticleSystem DieDissolveParticle;
        [SerializeField] ParticleSystem CoinParticle;

        private const float COIN_EFFECT_STRENGTH = 1f;
        private const string COIN_EFFECT_KEY = "OtherEffect/Script_CoinEffect0{0}";
        private const string COIN_EFFECT_HIT_KEY = "OtherEffect/Script_CoinHitEffect0{0}";
        private const string FLYING_COIN = "OtherEffect/Script_FlyingCoin";

        void Start() {
            if (DieDissolveParticle != null) DieDissolveParticle.gameObject.SetActive(false);
        }
        public void PlayDissolveEffect(SkinnedMeshRenderer _renderer) {
            //if (DieDissolveParticle != null) DieDissolveParticle.gameObject.SetActive(true);

            Material[] materials = _renderer.materials;  // 取得材質的複本
            for (int i = 0; i < materials.Length; i++) {
                if (_renderer.materials[i] != null) {
                    //將本來material更改為死亡material並改變texture
                    var tex = materials[i].GetTexture("_MainTex");
                    materials[i] = new Material(ResourcePreSetter.GetMaterial("MonsterDie"));
                    materials[i].SetTexture("_MainTex", tex);
                    //設定material的shader參數
                    materials[i].SetFloat("_Progress", 0);
                    int index = i;
                    UniTaskManager.StartTask(GetInstanceID() + "_" + index.ToString(), () => {
                        DOTween.To(() => 0f, x => materials[index].SetFloat("_Progress", x), 1f, GameSettingJsonData.GetFloat(GameSetting.DieEffect_DissolveDecaySec));
                    }, 500);
                }
            }
            _renderer.materials = materials;  // 將修改後的材質陣列設回Renderer的材質陣列

        }

        public void PlayCoinEffect(MonsterJsonData.MonsterSize size, SkinnedMeshRenderer smr, int heroIndex) {
            PoolManager.Instance.Pop(GetCoinEffectKey(size), popCallback: go => {
                go.transform.position = transform.position;
                if (size == MonsterJsonData.MonsterSize.Large) {
                    var ps = go.GetComponentInChildren<ParticleSystem>();
                    var shape = ps.shape;
                    shape.skinnedMeshRenderer = smr;
                }
            });
            PoolManager.Instance.Pop(GetCoinHitEffectKey(size), transform.position);

            double delay = size == MonsterJsonData.MonsterSize.Large ? 2000 : 1700;
            Observable.Timer(TimeSpan.FromMilliseconds(delay)).Subscribe(_ => {
                PoolManager.Instance.Pop(FLYING_COIN, transform.position, Quaternion.identity, null, go => {
                    var flyingCoin = go.GetComponent<FlyingCoin>();
                    flyingCoin.Init(size, heroIndex);
                });
            });
            //if (CoinParticle == null) return;
            ////Debug.Log("coin hit direction " + hitDirection);
            //var deltaPos = hitDirection.normalized * COIN_EFFECT_STRENGTH;
            ////設定力道
            //var velocity = CoinParticle.velocityOverLifetime;
            //velocity.x = new ParticleSystem.MinMaxCurve(velocity.x.constantMin + deltaPos.x, velocity.x.constantMax + deltaPos.x);
            //velocity.z = new ParticleSystem.MinMaxCurve(velocity.z.constantMin + deltaPos.z, velocity.z.constantMax + deltaPos.z);
            ////設定數量
            //var emission = CoinParticle.emission;
            //emission.SetBurst(0, new ParticleSystem.Burst(0, emitCount));

            //CoinParticle.transform.parent.gameObject.SetActive(true);
        }

        private string GetCoinEffectKey(MonsterJsonData.MonsterSize size) {
            switch (size) {
                case MonsterJsonData.MonsterSize.Small:
                    return string.Format(COIN_EFFECT_KEY, 1);
                case MonsterJsonData.MonsterSize.Mid:
                    return string.Format(COIN_EFFECT_KEY, 2);
                case MonsterJsonData.MonsterSize.Large:
                    return string.Format(COIN_EFFECT_KEY, 3);
            }
            throw new System.Exception("size not match coin effect: " + size);
        }

        private string GetCoinHitEffectKey(MonsterJsonData.MonsterSize size) {
            switch (size) {
                case MonsterJsonData.MonsterSize.Small:
                    return string.Format(COIN_EFFECT_HIT_KEY, 1);
                case MonsterJsonData.MonsterSize.Mid:
                    return string.Format(COIN_EFFECT_HIT_KEY, 2);
                case MonsterJsonData.MonsterSize.Large:
                    return string.Format(COIN_EFFECT_HIT_KEY, 3);
            }
            throw new System.Exception("size not match coin hit effect: " + size);
        }
    }
}