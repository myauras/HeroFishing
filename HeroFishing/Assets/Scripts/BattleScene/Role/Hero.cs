using HeroFishing.Main;
using JetBrains.Annotations;
using Scoz.Func;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HeroFishing.Battle {
    public class Hero : Role {

        public HeroData MyData { get; private set; }
        HeroSkinData MyHeroSkinData;

        public void SetData(int _heroID, string _heroSkinID) {
            MyData = HeroData.GetData(_heroID);
            MyHeroSkinData = HeroSkinData.GetData(_heroSkinID);
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
        }
        protected override void SetModel() {
            base.SetModel();
            string path = string.Format("Role/{0}/{1}.png", MyData.Ref, MyHeroSkinData.Texture);
            AddressablesLoader.GetPrefabResourceByPath<Texture>(path, (texture, handle) => {
                MySkinnedMaterial.SetTexture("_MainTex", texture);
                Addressables.Release(handle);
                LoadDone();
            });
        }

        public void PlayIdleMotion() {
            string rndMotion = Prob.GetRandomTFromTArray(MyData.IdleMotions);
            SetAniTrigger(rndMotion);
        }

        public void PlaySpellMotion(SpellName _spellName) {

            var spellData = HeroSpellData.GetSpell(MyData.ID, _spellName);
            if (MyData == null) { WriteLog.LogErrorFormat("HeroData尚未設定"); return; }
            if (spellData == null) { WriteLog.LogErrorFormat("此HeroID {0} 無此 SpellName:{1}", MyData.ID, _spellName); return; }
            if (spellData.Motions.Length == 0) { WriteLog.LogErrorFormat("SpellID為 {0} 的Mostions欄位沒有填", spellData.ID); return; }
            string rndMotion = Prob.GetRandomTFromTArray(spellData.Motions);
            SetAniTrigger(rndMotion);
        }
    }
}