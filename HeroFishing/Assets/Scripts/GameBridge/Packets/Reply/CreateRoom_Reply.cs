using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket {
    public class CreateRoom_Reply : SocketContent {
        /// <summary>
        /// 房內玩家ID
        /// </summary>
        public string[] PlayerIDs { get; private set; }
        /// <summary>
        /// 地圖ID
        /// </summary>
        public string MapID { get; private set; }
        /// <summary>
        /// 遊戲ServerIP
        /// </summary>
        public string GameServerIP { get; private set; }
        /// <summary>
        /// 遊戲ServerPort
        /// </summary>
        public int GameServerPort { get; private set; }
        /// <summary>
        /// 遊戲Server名稱
        /// </summary>
        public string GameServerName { get; private set; }


        public CreateRoom_Reply() {
        }
    }
}