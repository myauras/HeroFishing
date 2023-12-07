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
            SocketCMD<SETHERO> cmd = new SocketCMD<SETHERO>(new SETHERO(_heroID, _heroSkinID));
            Socket.TCPSend(cmd);
        }
        /// <summary>
        /// 離開遊戲房
        /// </summary>
        public void LeaveRoom() {
            SocketCMD<LEAVE> cmd = new SocketCMD<LEAVE>(new LEAVE());
            Socket.TCPSend(cmd);
        }
        /// <summary>
        /// 擊中
        /// </summary>
        public void Hit(string _attackID, int[] _monsterIdxs, string _spellJsonID) {
            SocketCMD<HIT> cmd = new SocketCMD<HIT>(new HIT(_attackID, _monsterIdxs, _spellJsonID));
            Socket.TCPSend(cmd);
        }
    }
}