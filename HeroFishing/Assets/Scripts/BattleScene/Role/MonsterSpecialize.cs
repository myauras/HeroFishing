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
            if (DieDissolveParticle != null) DieDissolveParticle.gameObject.SetActive(true);

            Material[] materials = _renderer.materials;  // 取得材質的複本
            for (int i = 0; i < materials.Length; i++) {
                if (_renderer.materials[i] != null) {
                    //將本來material更改為死亡material並改變texture
                    var tex = materials[i].GetTexture("_MainTex");
                    materials[i] = ResourcePreSetter.GetMaterial("MonsterDie");
                    materials[i].SetTexture("_MainTex", tex);
                    //設定material的shader參數
                    materials[i].SetFloat("_Progress", 0);
                    int index = i;
                    UniTaskManager.StartTask(GetInstanceID() + "_" + index.ToString(), () => {
                        DOTween.To(() => 0f, x => materials[index].SetFloat("_Progress", x), 1f, GameSettingJsonData.GetFloat(GameSetting.DieEffect_DissolveDecaySec));
                    }, 500);
                }
            }
            _renderer.materials = materials;  // 將修改後的材質陣列設回Renderer的材質陣列

        }
    }
}