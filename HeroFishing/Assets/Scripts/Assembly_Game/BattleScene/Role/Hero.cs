using DG.Tweening;
using HeroFishing.Main;
using HeroFishing.Socket;
using JetBrains.Annotations;
using Scoz.Func;
using Service.Realms;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace HeroFishing.Battle {
    public class Hero : Role {
        public HeroJsonData MyData { get; private set; }
        HeroSkinJsonData MyHeroSkinData;
        private SpellActivationBehaviour ActivationBehaviour;

        private int _exp;
        private int _level = 1;
        private int _points;
        private int _storedPoints;
        private GameObject _model;
        private Dictionary<int, int> _storedMonsterPoints = new Dictionary<int, int>();

        public int Exp => _exp;
        public int Level => _level;
        public int Points => _points;
        public bool IsLoaded => _model != null;
        public int TotalPoints => _points + _storedPoints;
        public float AttackSpeedMultiplier { get; set; } = 1;


        public event Action<int> OnLevelUp;
        public event Action<int, int> OnExpUpdate;
        public event Action<SpellName> OnSpellCharge;
        public event Action<SpellName> OnSpellPlay;
        public event Action<int, int> OnPointGet;
        public event Action<int, bool> OnPointUpdate;

        public static int Bet => BattleManager.Instance.Bet;

        private const int SPELL_COUNT = 4;
        private const int MAX_LEVEL = 10;

        public void Register(SpellActivationBehaviour activationBehaviour) {
            ActivationBehaviour = activationBehaviour;
        }

        public void SetData(int _heroID, string _heroSkinID) {
            MyData = HeroJsonData.GetData(_heroID);
            MyHeroSkinData = HeroSkinJsonData.GetData(_heroSkinID);
            LoadModel();
        }

        public void ResetData() {
            if (_model != null)
                Destroy(_model);
            _model = null;
        }

        void LoadModel() {
            string path = string.Format("Role/{0}/{1}.prefab", MyData.Ref, MyHeroSkinData.Prefab);
            AddressablesLoader.GetPrefabResourceByPath<GameObject>(path, (prefab, handle) => {
                _model = Instantiate(prefab, transform);
                _model.transform.localPosition = prefab.transform.localPosition;
                _model.transform.localRotation = prefab.transform.localRotation;
                _model.transform.localScale = prefab.transform.localScale;
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

        public void PlaySpell(SpellName _spellName) {
            ActivationBehaviour?.OnSpellPlay(_spellName);

            var spellData = HeroSpellJsonData.GetSpell(MyData.ID, _spellName);
            if (MyData == null) { WriteLog.LogErrorFormat("HeroData尚未設定"); return; }
            if (spellData == null) { WriteLog.LogErrorFormat("此HeroID {0} 無此 SpellName:{1}", MyData.ID, _spellName); return; }
            PlaySpellMotion(spellData);
            PlaySpellEffect(spellData);
            OnSpellPlay?.Invoke(_spellName);
        }

        private void PlaySpellMotion(HeroSpellJsonData spellData) {
            if (spellData.Motions.Length == 0) { WriteLog.LogErrorFormat("SpellID為 {0} 的Motions欄位沒有填", spellData.ID); return; }
            string rndMotion = Prob.GetRandomTFromTArray(spellData.Motions);
            SetAniTrigger(rndMotion);
        }

        public void PlaySpellEffect(HeroSpellJsonData spellData) {
            if (spellData.HeroShaderSettings == null || spellData.HeroShaderSettings.Length == 0) return;

            Color32 specularColor = new Color32((byte)spellData.HeroShaderSettings[0], (byte)spellData.HeroShaderSettings[1], (byte)spellData.HeroShaderSettings[2], (byte)spellData.HeroShaderSettings[3]);
            float hdrIntensity = Mathf.Pow(2, spellData.HeroShaderSettings[4]); // hdr color intensity設定
            //設定HDR Color
            Color specularHDRColor = new Color(
               (specularColor.r * hdrIntensity / 255),
               (specularColor.g * hdrIntensity / 255),
               (specularColor.b * hdrIntensity / 255),
               specularColor.a / 255
            );
            //Debug.Log(specularHDRColor);

            Color32 rimColor = new Color32((byte)spellData.HeroShaderSettings[5], (byte)spellData.HeroShaderSettings[6], (byte)spellData.HeroShaderSettings[7], (byte)spellData.HeroShaderSettings[8]);
            hdrIntensity = Mathf.Pow(2, spellData.HeroShaderSettings[9]); // hdr color intensity設定
            //設定HDR Color
            Color rimHDRColor = new Color(
               (rimColor.r * hdrIntensity / 255),
               (rimColor.g * hdrIntensity / 255),
               (rimColor.b * hdrIntensity / 255),
               rimColor.a / 255
            );
            //Debug.Log(rimHDRColor);

            PropertyBlock.SetColor("_SpecularColor", specularHDRColor);
            PropertyBlock.SetColor("_RimColor", rimHDRColor);
            PropertyBlock.SetFloat("_RimValue", 1);

            DOTween.To(() => 1f, x => {
                PropertyBlock.SetFloat("_RimValue", x);
                SetPropertyBlock(PropertyBlock);
            }, 0, spellData.HeroShaderSettings[10]);
        }

        public void AddExp(int exp) {
            if (_level == MAX_LEVEL) return;
            _exp += exp;
            var nextLevelExp = HeroEXPJsonData.GetData(_level).EXP;
            if (_exp >= nextLevelExp) {
                LevelUp();
                _exp -= nextLevelExp;
            }
            OnExpUpdate?.Invoke(_exp, nextLevelExp);
        }

        public void HoldStoredPoints(int monsterIdx, int points) {
            _storedMonsterPoints.Add(monsterIdx, points);
            _storedPoints += points;

            OnPointGet?.Invoke(monsterIdx, points);
        }

        public void ReleaseStoredPoints(int monsterIdx) {
            if (_storedMonsterPoints.TryGetValue(monsterIdx, out int points)) {
                _points += points;
                _storedMonsterPoints.Remove(monsterIdx);
                _storedPoints -= points;

                OnPointUpdate?.Invoke(_points, true);
            }
        }

        public void UsePoints() {
            _points -= Bet;
            OnPointUpdate?.Invoke(_points, false);
        }

        public void UpdatePoints(int points) {
            _points = points;
            OnPointUpdate?.Invoke(points, false);
        }

        [ContextMenu("Level Up")]
        public void LevelUp() {
            _level = Mathf.Min(_level + 1, MAX_LEVEL);
            OnLevelUp?.Invoke(_level);
        }

        public void ChargeSpell(int[] spellIndices) {
            for (int i = 0; i < spellIndices.Length; i++) {
                int spellIndex = spellIndices[i];
                if (spellIndex <= 0) continue;
                OnSpellCharge?.Invoke((SpellName)spellIndex);
            }
        }

        public void ChargeSpell(int spellIndex) {
            OnSpellCharge?.Invoke((SpellName)spellIndex);
        }
    }
}