using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket {
    public class CreateRoom_Reply : SocketContent {
        /// <summary>
        /// 創房者ID
        /// </summary>
        public string CreaterID { get; private set; }
        /// <summary>
        /// 房間內的所有PlayerID
        /// </summary>
        public string[] PlayerIDs { get; private set; }
        /// <summary>
        /// DB地圖ID
        /// </summary>
        public string DBMapID { get; private set; }
        /// <summary>
        /// DBMatchgame的ID(由Matchmaker產生，格視為[玩家ID]_[累加數字]_[日期時間])
        /// </summary>
        public string DBMatchgameID { get; private set; }
        /// <summary>
        /// Matchmaker派發Matchgame的IP
        /// </summary>
        public string IP { get; private set; }
        /// <summary>
        /// Matchmaker派發Matchgame的Port
        /// </summary>
        public int Port { get; private set; }
        /// <summary>
        /// Matchmaker派發Matchgame的Pod名稱
        /// </summary>
        public string PodName { get; private set; }


        public CreateRoom_Reply() {
        }
    }
}