using UnityEngine;
using UnityEditor;
using Scoz.Func;
using HeroFishing.Battle;

namespace HeroFishing.Main {
    [CustomPropertyDrawer(typeof(PostProcessingManager.BloomSettingDicClass))]
    [CustomPropertyDrawer(typeof(ResourcePreSetter.MaterialDicClass))]
    [CustomPropertyDrawer(typeof(SpellIndicator.IndicatorDicClass))]

    public class CustomInfoSerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer { }
}