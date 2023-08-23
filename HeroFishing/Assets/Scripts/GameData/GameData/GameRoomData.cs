using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeroFishing.Main {
    /// <summary>
    /// 玩家目前所在遊戲房間的資料
    /// </summary>
    public class GameRoomData {
        private static GameRoomData instance = null;
        public static GameRoomData Instance {
            get {
                if (instance == null)
                    instance = new GameRoomData();
                return instance;
            }
        }

        /// <summary>
        /// 房型資料
        /// </summary>
        //public MaJamRoomData MaJamRoomData { get; private set; }

        /// <summary>
        /// 配對玩家資料
        /// Key為PlayerID，紀錄此房間內的玩家字典
        /// </summary>
        //public Dictionary<string, PlayerData> MatchingPlayers { get; private set; }

        /// <summary>
        /// 房主ID或排隊玩家ID
        /// </summary>
        public string CreaterID { get; private set; }

        /// <summary>
        /// 配對伺服器派發的GamServerIP
        /// </summary>
        public string GameServerIP { get; private set; }

        /// <summary>
        /// 配對伺服器派發的Port
        /// </summary>
        public int GameServerPort { get; private set; }

        /// <summary>
        /// 配對伺服器派發的GameServer名稱
        /// </summary>
        public string GameServerName { get; private set; }

        public void Init() {

        }
    }
}

