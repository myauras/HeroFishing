using UnityEngine;
using Scoz.Func;
using HeroFishing.Main;
using System;
using static HeroFishing.Main.HeroSpellData;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;

namespace HeroFishing.Battle {
    public class SpellIndicator : MonoBehaviour {
        public enum IndicatorType {
            Line,
            Cone,
            Circle,
        }
        [Serializable] public class IndicatorDicClass : SerializableDictionary<IndicatorType, AssetReference> { }
        [SerializeField] IndicatorDicClass MyIndicatorPrfabDic;//施法指示UI字典

        HeroSpellData TmpSpellData;
        public Dictionary<IndicatorType, List<GameObject>> Indicators = new Dictionary<IndicatorType, List<GameObject>>();


        public static SpellIndicator Instance { get; private set; }


        public void Init() {
            Instance = this;
            Hide();
        }
        public void Hide() {
            gameObject.SetActive(false);
        }
        public void Show() {
            gameObject.SetActive(true);
        }
        void HideIndicators() {
            foreach (var key in Indicators.Keys)
                foreach (var go in Indicators[key])
                    go.SetActive(false);
        }
        void SpawnNewIndicator(IndicatorType _type, Action<GameObject> _ac) {
            if (!MyIndicatorPrfabDic.ContainsKey(_type)) {
                WriteLog.LogErrorFormat("尚未指定{0}類型施法指示物的AssetReference", _type);
                _ac?.Invoke(null);
            }
            AddressablesLoader.GetPrefabByRef(MyIndicatorPrfabDic[_type], (prefab, handle) => {
                var go = Instantiate(prefab, transform);
                _ac?.Invoke(go);
            });
        }
        void GetAvailableIndicator(IndicatorType _type, Action<GameObject> _ac) {
            if (!Indicators.ContainsKey(_type) || Indicators[_type] == null) {
                SpawnNewIndicator(_type, _ac);
                return;
            }
            foreach (var go in Indicators[_type]) {
                if (!go.activeInHierarchy) continue;
                _ac?.Invoke(go);
                return;
            }
            SpawnNewIndicator(_type, _ac);
        }
        public void ShowIndicator(HeroSpellData _spellData) {
            Show();
            HideIndicators();
            TmpSpellData = _spellData;
            switch (TmpSpellData.MySpellType) {
                case SpellType.LineShot:
                    GetAvailableIndicator(IndicatorType.Line, go => {
                        var mat = go.GetComponent<MeshRenderer>().material;
                        mat.SetTextureOffset("_MainTex", new Vector2(0, -float.Parse(TmpSpellData.SpellTypeValues[0])));
                        go.transform.localScale = new Vector3(float.Parse(TmpSpellData.SpellTypeValues[1]), go.transform.localScale.y, go.transform.localScale.z);
                    });
                    break;
            }
        }
        public void RotateLineIndicator(Quaternion _rotation) {
            transform.localRotation = _rotation;
        }

    }
}