using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace HeroFishing.Socket.Matchmaker {
    public class CREATEROOM : SocketContent {
        //class名稱就是封包的CMD名稱

        /// <summary>
        /// DB Map文件ID
        /// </summary>
        public string DBMapID { get; private set; }
        /// <summary>
        /// 開房者ID
        /// </summary>
        public string CreaterID { get; private set; }

        public CREATEROOM(string _dbMapID, string _createrID) {
            DBMapID = _dbMapID;
            CreaterID = _createrID;
        }

    }
}