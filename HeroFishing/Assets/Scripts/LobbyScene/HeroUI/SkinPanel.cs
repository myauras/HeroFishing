using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using UnityEngine.UI;
using System.Linq;
using Service.Realms;
using TMPro;

namespace HeroFishing.Main {



    public class SkinPanel : ItemSpawner_Remote<SkinItem> {

        [SerializeField] ScrollRect MyScrollRect;

        public override void RefreshText() {
            base.RefreshText();
        }
        public void SetHero(int _heroID) {
            if (!LoadItemFinished) {
                WriteLog.LogError("SkinItem尚未載入完成");
                return;
            }
            InActiveAllItem();
            var skinDic = HeroSkinJsonData.GetSkinDic(_heroID);
            if (skinDic == null || skinDic.Count == 0) return;
            var skinJsons = skinDic.Values.ToList();
            for (int i = 0; i < skinJsons.Count; i++) {
                if (i < ItemList.Count) {
                    ItemList[i].Init(skinJsons[i]);
                    ItemList[i].IsActive = true;
                    ItemList[i].gameObject.SetActive(true);
                } else {
                    SkinItem item = Spawn();
                    item.Init(skinJsons[i]);
                }
            }
            MyScrollRect.verticalNormalizedPosition = 1;//至頂
        }

    }


}