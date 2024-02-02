using DG.Tweening;
using HeroFishing.Main;
using JetBrains.Annotations;
using Scoz.Func;
using System;
using System.Security.Cryptography;
using UniRx;
using UnityEngine;

namespace HeroFishing.Battle {
    public class MonsterSpecialize : MonoBehaviour {
        [SerializeField] ParticleSystem DieDissolveParticle;
        [SerializeField] ParticleSystem CoinParticle;
        [SerializeField]
        private GameObject[] AddOnObjs;

        private const float COIN_EFFECT_STRENGTH = 1f;
        private const string COIN_EFFECT_KEY = "OtherEffect/Script_CoinEffect0{0}";
        private const string COIN_EFFECT_HIT_KEY = "OtherEffect/Script_CoinHitEffect0{0}";
        private const string FLYING_COIN_KEY = "OtherEffect/Script_FlyingCoin";

        private const string DROP_EFFECT_HIT_KEY = "OtherEffect/Script_Hit{0}";
        private const string FLYING_DROP_KEY = "OtherEffect/Script_FlyingDrop";

        void Start() {
            if (DieDissolveParticle != null) DieDissolveParticle.gameObject.SetActive(false);
        }

        public void CloseAddOnObjs() {
            for (int i = 0; i < AddOnObjs.Length; i++) {
                AddOnObjs[i].SetActive(false);
            }
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

        public void PlayDropEffect(int dropID, int heroIndex) {
            var dropData = DropJsonData.GetData(dropID);
            string key = string.Format(DROP_EFFECT_HIT_KEY, dropData.Ref);
            PoolManager.Instance.Pop(key, transform.position);

            Observable.Timer(TimeSpan.FromMilliseconds(500)).Subscribe(_ => {
                PoolManager.Instance.Pop(FLYING_DROP_KEY, transform.position, popCallback: go => {
                    var flyingDrop = go.GetComponent<FlyingDrop>();
                    flyingDrop.Init(dropID, dropData.Ref, heroIndex);
                });
            });
        }

        public void PlayCoinEffect(MonsterJsonData.MonsterSize size, SkinnedMeshRenderer smr, int heroIndex, int monsterIndex) {
            PoolManager.Instance.Pop(GetCoinEffectKey(size), popCallback: go => {
                go.transform.position = transform.position;
                if (size == MonsterJsonData.MonsterSize.Large) {
                    var ps = go.GetComponentInChildren<ParticleSystem>();
                    var shape = ps.shape;
                    shape.skinnedMeshRenderer = smr;
                }
            });
            PoolManager.Instance.Pop(GetCoinHitEffectKey(size), transform.position);

            double delay = size == MonsterJsonData.MonsterSize.Large ? 1900 : 1700;
            Observable.Timer(TimeSpan.FromMilliseconds(delay)).Subscribe(_ => {
                PoolManager.Instance.Pop(FLYING_COIN_KEY, transform.position, Quaternion.identity, null, go => {
                    var flyingCoin = go.GetComponent<FlyingCoin>();
                    flyingCoin.Init(size, heroIndex, monsterIndex);
                });
            });
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