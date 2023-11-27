using DG.Tweening;
using HeroFishing.Main;
using JetBrains.Annotations;
using Scoz.Func;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HeroFishing.Battle {
    public class Hero : Role {

        public HeroJsonData MyData { get; private set; }
        HeroSkinJsonData MyHeroSkinData;
        private SpellActivationBehaviour ActivationBehaviour;

        private const int SPELL_COUNT = 4;

        public void Register(SpellActivationBehaviour activationBehaviour) {
            ActivationBehaviour =  activationBehaviour;
        }

        public void SetData(int _heroID, string _heroSkinID) {
            MyData = HeroJsonData.GetData(_heroID);
            MyHeroSkinData = HeroSkinJsonData.GetData(_heroSkinID);
            LoadModel();
        }
        void LoadModel() {
            string path = string.Format("Role/{0}/{1}.prefab", MyData.Ref, MyHeroSkinData.Prefab);
            AddressablesLoader.GetPrefabResourceByPath<GameObject>(path, (prefab, handle) => {
                var go = Instantiate(prefab, transform);
                go.transform.localPosition = prefab.transform.localPosition;
                go.transform.localRotation = prefab.transform.localRotation;
                go.transform.localScale = prefab.transform.localScale;
                Addressables.Release(handle);
                SetModel();
            });

            for (int i = 0; i < SPELL_COUNT; i++) {
                var spellData = HeroSpellJsonData.GetSpell(MyData.ID, (SpellName)i);
                PoolManager.Instance.InitHeroSpell(spellData);
            }
        }
        protected override void SetModel() {
            base.SetModel();
            string path = string.Format("Role/{0}/{1}.png", MyData.Ref, MyHeroSkinData.Texture);
            AddressablesLoader.GetPrefabResourceByPath<Texture>(path, (texture, handle) => {
                PropertyBlock.SetTexture("_MainTex", texture);
                SetPropertyBlock(PropertyBlock);
                Addressables.Release(handle);
                LoadDone();
            });
        }

        public void PlayIdleMotion() {
            string rndMotion = Prob.GetRandomTFromTArray(MyData.IdleMotions);
            SetAniTrigger(rndMotion);
        }

        public void OnSpellPlay(SpellName _spellName) {
            ActivationBehaviour?.OnSpellPlay(_spellName);

            var spellData = HeroSpellJsonData.GetSpell(MyData.ID, _spellName);
            if (MyData == null) { WriteLog.LogErrorFormat("HeroData尚未設定"); return; }
            if (spellData == null) { WriteLog.LogErrorFormat("此HeroID {0} 無此 SpellName:{1}", MyData.ID, _spellName); return; }
            PlaySpellMotion(spellData);
            PlaySpellEffect(spellData);
        }

        private void PlaySpellMotion(HeroSpellJsonData spellData) {
            if (spellData.Motions.Length == 0) { WriteLog.LogErrorFormat("SpellID為 {0} 的Motions欄位沒有填", spellData.ID); return; }
            string rndMotion = Prob.GetRandomTFromTArray(spellData.Motions);
            SetAniTrigger(rndMotion);
        }

        public void PlaySpellEffect(HeroSpellJsonData spellData) {
            if (spellData.HeroShaderSettings == null || spellData.HeroShaderSettings.Length == 0) return;

            Color32 specularColor = new Color32((byte)spellData.HeroShaderSettings[0], (byte)spellData.HeroShaderSettings[1], (byte)spellData.HeroShaderSettings[2], (byte)spellData.HeroShaderSettings[3]);
            float hdrIntensity = spellData.HeroShaderSettings[4]; // hdr color intensity設定
            //設定HDR Color
            Color32 specularHDRColor = new Color32(
               (byte)(specularColor.r * hdrIntensity),
               (byte)(specularColor.g * hdrIntensity),
               (byte)(specularColor.b * hdrIntensity),
               specularColor.a
            );

            Color32 rimColor = new Color32((byte)spellData.HeroShaderSettings[5], (byte)spellData.HeroShaderSettings[6], (byte)spellData.HeroShaderSettings[7], (byte)spellData.HeroShaderSettings[8]);
            hdrIntensity = spellData.HeroShaderSettings[9]; // hdr color intensity設定
            //設定HDR Color
            Color32 rimHDRColor = new Color32(
               (byte)(specularColor.r * hdrIntensity),
               (byte)(specularColor.g * hdrIntensity),
               (byte)(specularColor.b * hdrIntensity),
               specularColor.a
            );

            PropertyBlock.SetColor("_SpecularColor", specularHDRColor);
            PropertyBlock.SetColor("_RimColor", rimHDRColor);
            PropertyBlock.SetFloat("_RimValue", 1);

            DOTween.To(() => 1f, x => {
                PropertyBlock.SetFloat("_RimValue", x);
                SetPropertyBlock(PropertyBlock);
            }, 0, spellData.HeroShaderSettings[10]);
        }
    }
}