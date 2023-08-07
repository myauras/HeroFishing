using DG.Tweening;
using HeroFishing.Main;
using Scoz.Func;
using UnityEngine;

namespace HeroFishing.Battle {
    public class MonsterSpecialize : MonoBehaviour {
        [SerializeField] ParticleSystem DieDissolveParticle;

        void Start() {
            if (DieDissolveParticle != null) DieDissolveParticle.gameObject.SetActive(false);
        }
        public void PlayDissolveEffect(SkinnedMeshRenderer _renderer) {
            if (_renderer.material != null && DieDissolveParticle != null) {
                if (DieDissolveParticle != null) DieDissolveParticle.gameObject.SetActive(true);
                //將本來material更改為死亡material並改變texture
                var tex = _renderer.material.GetTexture("_MainTex");
                _renderer.material = ResourcePreSetter.GetMaterial("MonsterDie");
                _renderer.material.SetTexture("_MainTex", tex);
                //設定material的shader參數
                var material = _renderer.material;
                material.SetFloat("_Progress", 0);
                UniTaskManager.StartTask(GetInstanceID().ToString(), () => {
                    DOTween.To(() => 0f, x => material.SetFloat("_Progress", x), 1f, GameSettingData.GetFloat(GameSetting.DieEffect_DissolveDecaySec));
                }, 500);

            }
        }
    }
}