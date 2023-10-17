using DG.Tweening;
using HeroFishing.Main;
using Scoz.Func;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;
using static DG.DemiLib.External.DeHierarchyComponent;

namespace HeroFishing.Battle {
    public class Monster : Role {

        public MonsterJsonData MyData { get; private set; }

        MonsterSpecialize MyMonsterSpecialize;

        public void SetData(int _monsterID, Action _ac) {
            MyData = MonsterJsonData.GetData(_monsterID);
            LoadModel(_ac);
        }
        void LoadModel(Action _ac) {
            string path = string.Format("Monster/{0}/{0}.prefab", MyData.Ref);
            AddressablesLoader.GetPrefabResourceByPath<GameObject>(path, (prefab, handle) => {
                var go = Instantiate(prefab, transform);
                go.transform.localPosition = prefab.transform.localPosition;
                go.transform.localRotation = prefab.transform.localRotation;
                go.transform.localScale = prefab.transform.localScale;
                var monsterSpecialize = go.GetComponent<MonsterSpecialize>();
                MyMonsterSpecialize = monsterSpecialize;
                AddressableManage.SetToChangeSceneRelease(handle);//切場景再釋放資源
                SetModel();
                LoadDone();
                _ac?.Invoke();
            });
        }


        public void OnHit() {
            HitEffect();
        }

        void HitEffect() {
            if (MySkinnedMaterial == null) return;
            string spellID = "1_attack";
            var spellData = HeroSpellJsonData.GetData(spellID);
            //Color color = TextManager.ParseTextToColor32(GameSettingJsonData.GetStr(GameSetting.HitEffect_OutlineColor)); 已不使用Gamesetting的設定
            //MySkinnedMaterial.SetFloat("_FresnelPower", GameSettingJsonData.GetFloat(GameSetting.HitEffect_FresnelPower));已不使用Gamesetting的設定

            //設定怪物被擊中Shader效果
            Color32 color = new Color32((byte)spellData.HitMonsterShaderSetting[0], (byte)spellData.HitMonsterShaderSetting[1], (byte)spellData.HitMonsterShaderSetting[2], (byte)spellData.HitMonsterShaderSetting[3]);
            float hdrIntensity = spellData.HitMonsterShaderSetting[4]; // hdr color intensity設定
            //設定HDR Color
            Color32 hdrColor = new Color32(
               (byte)(color.r * hdrIntensity),
               (byte)(color.g * hdrIntensity),
               (byte)(color.b * hdrIntensity),
               color.a
            );
            MySkinnedMaterial.SetVector("_OutlineColor", new Vector3(color.r, color.g, color.b));
            MySkinnedMaterial.SetFloat("_FresnelPower", spellData.HitMonsterShaderSetting[5]);
            MySkinnedMaterial.SetFloat("_Opacity", spellData.HitMonsterShaderSetting[6]);
            MySkinnedMaterial.SetFloat("_Smoothness", spellData.HitMonsterShaderSetting[7]);
            MySkinnedMaterial.SetFloat("_Metallic", spellData.HitMonsterShaderSetting[8]);
            DOTween.To(() => 1f, x => MySkinnedMaterial.SetFloat("_Opacity", x), 0f, GameSettingJsonData.GetFloat(GameSetting.HitEffect_DecaySec));
        }
        public void Die() {
            if (MyData.MyMonsterType == MonsterJsonData.MonsterType.Boss) MonsterScheduler.BossExist = false;
            SetAniTrigger("die");
            if (MyMonsterSpecialize != null) MyMonsterSpecialize.PlayDissolveEffect(MySkinnedMeshRenderer);
        }

    }

}