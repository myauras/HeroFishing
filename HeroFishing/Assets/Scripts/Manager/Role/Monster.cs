using DG.Tweening;
using HeroFishing.Main;
using Scoz.Func;
using UnityEngine;
using UnityEngine.UIElements;

namespace HeroFishing.Battle {
    public class Monster : Role {
        public MonsterData MyData { get; private set; }
        [SerializeField] SkinnedMeshRenderer MyMeshRenderer;
        [SerializeField] ParticleSystem DieDissolveParticle;



        public override void Start() {
            base.Start();
            if (DieDissolveParticle != null) DieDissolveParticle.gameObject.SetActive(false);
        }


        public void SetData(MonsterData data) {
            MyData = data;
        }
        public void OnHit() {
            HitEffect();
        }

        void HitEffect() {
            var material = MyMeshRenderer.material;
            if (material == null) return;
            Color color = TextManager.ParseTextToColor32(GameSettingData.GetStr(GameSetting.HitEffect_OutlineColor));
            material.SetVector("_OutlineColor", new Vector3(color.r, color.g, color.b));
            material.SetFloat("_FresnelPower", GameSettingData.GetFloat(GameSetting.HitEffect_FresnelPower));
            material.SetFloat("_Intensity", 0);
            DOTween.To(() => 1f, x => material.SetFloat("_Intensity", x), 0f, GameSettingData.GetFloat(GameSetting.HitEffect_DecaySec));
        }
        public void Die() {
            if (MyData.MyMonsterType == MonsterData.MonsterType.Boss) MonsterScheduler.BossExist = false;
            SetAniTrigger("die");

            if (MyMeshRenderer.material != null && DieDissolveParticle != null) {
                if (DieDissolveParticle != null) DieDissolveParticle.gameObject.SetActive(true);
                //將本來material更改為死亡material並改變texture
                var tex = MyMeshRenderer.material.GetTexture("_MainTex");
                MyMeshRenderer.material = ResourcePreSetter.GetMaterial("MonsterDie");
                MyMeshRenderer.material.SetTexture("_MainTex", tex);
                //設定material的shader參數
                var material = MyMeshRenderer.material;
                material.SetFloat("_Progress", 0);
                UniTaskManager.StartTask(GetInstanceID().ToString(), () => {
                    DOTween.To(() => 0f, x => material.SetFloat("_Progress", x), 1f, GameSettingData.GetFloat(GameSetting.DieEffect_DissolveDecaySec));
                }, 500);

            }
        }

    }

}