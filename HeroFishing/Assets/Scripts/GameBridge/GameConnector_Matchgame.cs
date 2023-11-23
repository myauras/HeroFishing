using Cysharp.Threading.Tasks;
using HeroFishing.Main;
using HeroFishing.Socket.Matchgame;
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
            SocketCMD<ACTION_SETHERO> cmd = new SocketCMD<ACTION_SETHERO>(new ACTION_SETHERO(_heroID, _heroSkinID));
            HeroFishingSocket.GetInstance().TCPSend(cmd);
        }
        /// <summary>
        /// 離開遊戲房
        /// </summary>
        public void LeaveRoom() {
            SocketCMD<ACTION_LEAVE> cmd = new SocketCMD<ACTION_LEAVE>(new ACTION_LEAVE());
            HeroFishingSocket.GetInstance().TCPSend(cmd);
        }
        /// <summary>
        /// 擊中
        /// </summary>
        public void Hit(string _attackID, int[] _monsterIdxs, string _spellJsonID) {
            SocketCMD<ACTION_HIT> cmd = new SocketCMD<ACTION_HIT>(new ACTION_HIT(_attackID, _monsterIdxs, _spellJsonID));
            HeroFishingSocket.GetInstance().TCPSend(cmd);
        }
    }
}