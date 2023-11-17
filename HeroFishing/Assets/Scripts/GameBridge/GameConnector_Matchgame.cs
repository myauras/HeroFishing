using Cysharp.Threading.Tasks;
using HeroFishing.Main;
using Newtonsoft.Json.Linq;
using Scoz.Func;
using Service.Realms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeroFishing.Socket {
    public partial class GameConnector : MonoBehaviour {
        /// <summary>
        /// 設定使用英雄ID
        /// </summary>
        public void SetHero(int _heroID, string _heroSkinID) {
            HeroFishingSocket.GetInstance().SetHero(_heroID, _heroSkinID);
        }
        /// <summary>
        /// 離開遊戲房
        /// </summary>
        public void LeaveRoom() {
            HeroFishingSocket.GetInstance().Leave();
        }
    }
}