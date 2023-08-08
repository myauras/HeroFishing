using UnityEngine;
using Scoz.Func;
using HeroFishing.Main;
using System;
using static HeroFishing.Main.HeroSpellData;

namespace HeroFishing.Battle {
    public class SpellIndicator : MonoBehaviour {
        public enum IndicatorType {
            Line,
            Cone,
            Circle,
        }
        [Serializable] public class IndicatorDicClass : SerializableDictionary<IndicatorType, GameObject> { }
        [SerializeField] IndicatorDicClass MyIndicatorDic;//施法指示UI字典

        Material TmpMaterial;
        HeroSpellData TmpSpellData;
        IndicatorType TmpIndicatorType;

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
            foreach (var go in MyIndicatorDic.Values)
                go.SetActive(false);
        }
        public void ShowIndicator(HeroSpellData _spellData) {
            Show();
            HideIndicators();
            TmpSpellData = _spellData;
            switch (TmpSpellData.MySpellType) {
                case SpellType.LineShot:
                    TmpIndicatorType = IndicatorType.Line;
                    MyIndicatorDic[TmpIndicatorType].SetActive(true);
                    TmpMaterial = MyIndicatorDic[TmpIndicatorType].GetComponent<MeshRenderer>().material;
                    MyIndicatorDic[TmpIndicatorType].transform.localScale = new Vector3(float.Parse(TmpSpellData.SpellTypeValues[1]), MyIndicatorDic[IndicatorType.Line].transform.localScale.y, MyIndicatorDic[IndicatorType.Line].transform.localScale.z);
                    TmpMaterial.SetTextureOffset("_MainTex", new Vector2(0, -float.Parse(TmpSpellData.SpellTypeValues[0])));
                    break;
            }
        }
        public void RotateLineIndicator(Quaternion _rotation) {
            transform.localRotation = _rotation;
        }

    }
}