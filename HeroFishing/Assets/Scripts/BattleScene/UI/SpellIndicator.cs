using UnityEngine;
using Scoz.Func;
using HeroFishing.Main;
using System;
using static HeroFishing.Main.HeroSpellData;

namespace HeroFishing.Battle {
    public class SpellIndicator : MonoBehaviour {
        public enum IndicatorType {
            Line,
            Circle,
        }
        [Serializable] public class IndicatorDicClass : SerializableDictionary<IndicatorType, GameObject> { }
        [SerializeField] IndicatorDicClass MyIndicatorDic;//施法指示UI字典

        Material TmpMaterial;

        public static SpellIndicator Instance { get; private set; }

        private void Start() {
            Instance = this;
        }
        void HideIndicators() {
            foreach (var go in MyIndicatorDic.Values)
                go.SetActive(false);
        }
        public void ShowIndicator(HeroSpellData _spellData) {
            HideIndicators();

            switch (_spellData.MySpellType) {
                case SpellType.Line:
                    MyIndicatorDic[IndicatorType.Line].SetActive(true);
                    TmpMaterial = MyIndicatorDic[IndicatorType.Line].GetComponent<MeshRenderer>().material;
                    MyIndicatorDic[IndicatorType.Line].transform.localScale = new Vector3(float.Parse(_spellData.SpellValues[1]), MyIndicatorDic[IndicatorType.Line].transform.localScale.y, MyIndicatorDic[IndicatorType.Line].transform.localScale.z);
                    TmpMaterial.SetTextureOffset("_MainTex", new Vector2(0, float.Parse(_spellData.SpellValues[0])));
                    break;
                case SpellType.Sector:
                    MyIndicatorDic[IndicatorType.Circle].SetActive(true);
                    TmpMaterial = MyIndicatorDic[IndicatorType.Circle].GetComponent<MeshRenderer>().material;
                    break;
            }
        }

    }
}