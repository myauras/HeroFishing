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
            Debug.Log(_renderer.materials.Length);
            if (DieDissolveParticle != null) DieDissolveParticle.gameObject.SetActive(true);
            for (int i = 0; i < _renderer.materials.Length; i++) {
                if (_renderer.materials[i] != null) {
                    //將本來material更改為死亡material並改變texture
                    var tex = _renderer.materials[i].GetTexture("_MainTex");
                    _renderer.materials[i] = ResourcePreSetter.GetMaterial("MonsterDie");
                    _renderer.materials[i].SetTexture("_MainTex", tex);
                    //設定material的shader參數
                    _renderer.materials[i].SetFloat("_Progress", 0);
                    int index = i;
                    UniTaskManager.StartTask(GetInstanceID() + "_" + index.ToString(), () => {
                        DOTween.To(() => 0f, x => _renderer.materials[index].SetFloat("_Progress", x), 1f, GameSettingData.GetFloat(GameSetting.DieEffect_DissolveDecaySec));
                    }, 500);

                }
            }

        }
    }
}