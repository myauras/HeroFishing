using UnityEngine;
using Scoz.Func;
using HeroFishing.Main;
using System;
using static HeroFishing.Main.HeroSpellJsonData;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace HeroFishing.Battle {
    public class SpellIndicator : MonoBehaviour {
        public enum IndicatorType {
            Line,
            Cone,
            Circle,
        }
        [Serializable] public class IndicatorDicClass : SerializableDictionary<IndicatorType, AssetReference> { }
        [SerializeField] IndicatorDicClass MyIndicatorPrfabDic;//施法指示UI字典

        HeroSpellJsonData TmpSpellData;
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
                if (Indicators.ContainsKey(_type)) Indicators[_type].Add(go);
                else Indicators.Add(_type, new List<GameObject>() { go });
                _ac?.Invoke(go);
            });
        }
        void GetAvailableIndicator(IndicatorType _type, Action<GameObject> _ac) {
            if (!Indicators.ContainsKey(_type) || Indicators[_type] == null) {
                SpawnNewIndicator(_type, _ac);
                return;
            }
            foreach (var go in Indicators[_type]) {
                if (go.activeSelf) continue;
                go.SetActive(true);
                _ac?.Invoke(go);
                return;
            }
            SpawnNewIndicator(_type, _ac);
        }
        public void ShowIndicator(HeroSpellJsonData _spellData) {
            Show();
            HideIndicators();
            TmpSpellData = _spellData;
            switch (TmpSpellData.MySpellType) {
                case SpellType.LineShot:
                    GetAvailableIndicator(IndicatorType.Line, go => {
                        var mr = go.GetComponentInChildren<MeshRenderer>();
                        var mat = mr.material;
                        mat.SetTextureOffset("_MainTex", new Vector2(0, -float.Parse(TmpSpellData.SpellTypeValues[0])));
                        mr.transform.localScale = new Vector3(float.Parse(TmpSpellData.SpellTypeValues[1]), mr.transform.localScale.y, mr.transform.localScale.z);
                        go.transform.localRotation = Quaternion.identity;
                    });
                    break;
                case SpellType.SpreadLineShot:
                    float intervalAngle = float.Parse(TmpSpellData.SpellTypeValues[3]);//射散間隔角度
                    int spreadLineCount = int.Parse(TmpSpellData.SpellTypeValues[4]);//射散數量

                    float startAngle = -intervalAngle * (spreadLineCount - 1) / 2.0f;//設定第一個指標的角度
                    for (int i = 0; i < spreadLineCount; i++) {
                        float curAngle = startAngle + intervalAngle * i;
                        GetAvailableIndicator(IndicatorType.Line, go => {
                            var mr = go.GetComponentInChildren<MeshRenderer>();
                            var mat = mr.material;
                            mat.SetTextureOffset("_MainTex", new Vector2(0, -float.Parse(TmpSpellData.SpellTypeValues[0])));
                            mr.transform.localScale = new Vector3(float.Parse(TmpSpellData.SpellTypeValues[1]), mr.transform.localScale.y, mr.transform.localScale.z);
                            go.transform.localRotation = Quaternion.Euler(new Vector3(0, curAngle, 0));
                        });
                    }


                    break;
            }
        }
        public void RotateLineIndicator(Quaternion _rotation) {
            transform.localRotation = _rotation;
        }

    }
}