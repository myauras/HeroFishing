using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace HeroFishing.Socket {
    public class CreateRoom : SocketContent {
        /// <summary>
        /// 地圖ID
        /// </summary>
        public string MapID { get; private set; }
        /// <summary>
        /// 玩家ID列表
        /// </summary>
        public string[] PlayerIDs { get; private set; }
        /// <summary>
        /// 開房者ID
        /// </summary>
        public string CreaterID { get; private set; }

        public CreateRoom(string roomID, string[] playerIDs, string masterID) {
            MapID = roomID;
            PlayerIDs = playerIDs;
            CreaterID = masterID;
        }

    }
}