using DG.Tweening;
using HeroFishing.Main;
using Scoz.Func;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

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
            Color color = TextManager.ParseTextToColor32(GameSettingJsonData.GetStr(GameSetting.HitEffect_OutlineColor));
            MySkinnedMaterial.SetVector("_OutlineColor", new Vector3(color.r, color.g, color.b));
            MySkinnedMaterial.SetFloat("_FresnelPower", GameSettingJsonData.GetFloat(GameSetting.HitEffect_FresnelPower));
            MySkinnedMaterial.SetFloat("_Intensity", 0);
            DOTween.To(() => 1f, x => MySkinnedMaterial.SetFloat("_Intensity", x), 0f, GameSettingJsonData.GetFloat(GameSetting.HitEffect_DecaySec));
        }
        public void Die() {
            if (MyData.MyMonsterType == MonsterJsonData.MonsterType.Boss) MonsterScheduler.BossExist = false;
            SetAniTrigger("die");
            if (MyMonsterSpecialize != null) MyMonsterSpecialize.PlayDissolveEffect(MySkinnedMeshRenderer);
        }

    }

}