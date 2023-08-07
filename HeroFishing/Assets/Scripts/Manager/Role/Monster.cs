using DG.Tweening;
using HeroFishing.Main;
using Scoz.Func;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace HeroFishing.Battle {
    public class Monster : Role {

        public MonsterData MyData { get; private set; }

        MonsterSpecialize MyMonsterSpecialize;

        public void SetData(int _monsterID, Action _ac) {
            MyData = MonsterData.GetData(_monsterID);
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
                //Addressables.Release(handle);
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
            Color color = TextManager.ParseTextToColor32(GameSettingData.GetStr(GameSetting.HitEffect_OutlineColor));
            MySkinnedMaterial.SetVector("_OutlineColor", new Vector3(color.r, color.g, color.b));
            MySkinnedMaterial.SetFloat("_FresnelPower", GameSettingData.GetFloat(GameSetting.HitEffect_FresnelPower));
            MySkinnedMaterial.SetFloat("_Intensity", 0);
            DOTween.To(() => 1f, x => MySkinnedMaterial.SetFloat("_Intensity", x), 0f, GameSettingData.GetFloat(GameSetting.HitEffect_DecaySec));
        }
        public void Die() {
            if (MyData.MyMonsterType == MonsterData.MonsterType.Boss) MonsterScheduler.BossExist = false;
            SetAniTrigger("die");
            if (MyMonsterSpecialize != null) MyMonsterSpecialize.PlayDissolveEffect(MySkinnedMeshRenderer);
        }

    }

}