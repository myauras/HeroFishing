using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using UnityEngine.UI;
using System;
using TMPro;
using FancyScrollView;
using JoshH.UI;
using UnityEngine.AddressableAssets;

namespace HeroFishing.Main {

    public class SkinItem : FancyCell<SkinItemData, SkinScrollContext> {
        [SerializeField] Image SkinBGImg;
        [SerializeField] Image SkinIconImg;
        [SerializeField] Animator ScrollAni;
        [SerializeField] Transform Parent;

        Material ImgMaterial;//Material副本
        public bool IsActive { get; set; }

        public SkinItemData MySkinItem { get; private set; }
        private void Start() {
            //Btn.onClick.AddListener(() => Context.OnClick?.Invoke(Index));

        }
        public override void Initialize() {
            base.Initialize();
        }

        public override void UpdateContent(SkinItemData _item) {
            MySkinItem = _item;
            if (ImgMaterial == null) ImgMaterial = new Material(SkinIconImg.material);


            if (Context.SelectedIndex == Index) {
                transform.SetAsLastSibling();
                ImgMaterial.SetFloat("_EffectAmount", 0f);
                Parent.localScale = Vector2.one;
                AddressablesLoader.GetSpriteAtlas("HeroSkinIcon", atlas => {
                    SkinBGImg.sprite = atlas.GetSprite("SkinBG_On");
                    SkinIconImg.sprite = atlas.GetSprite(MySkinItem.MyJsonSkin.ID);
                });
            } else {
                ImgMaterial.SetFloat("_EffectAmount", 0.3f);
                Parent.localScale = new Vector2(0.75f, 0.75f);
                AddressablesLoader.GetSpriteAtlas("HeroSkinIcon", atlas => {
                    SkinBGImg.sprite = atlas.GetSprite("SkinBG");
                    SkinIconImg.sprite = atlas.GetSprite(MySkinItem.MyJsonSkin.ID);
                });
            }
        }

        public override void UpdatePosition(float position) {
            if (ScrollAni.isActiveAndEnabled) {
                ScrollAni.Play("scroll", -1, position);
            }
            ScrollAni.speed = 0;
        }
    }
}
