using Scoz.Func;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeroFishing.Main {
    /// <summary>
    /// 玩家目前所在遊戲房間的資料，CreateRoom後會從Matchmaker回傳取得資料
    /// </summary>
    public class AllocatedRoom {
        private static AllocatedRoom instance = null;
        public static AllocatedRoom Instance {
            get {
                if (instance == null)
                    instance = new AllocatedRoom();
                return instance;
            }
        }

        /// <summary>
        /// DB地圖ID
        /// </summary>
        public string DBMapID { get; private set; }

        /// <summary>
        /// 房主ID或排隊玩家ID
        /// </summary>
        public string CreaterID { get; private set; }

        /// <summary>
        ///  Matchmaker派發Matchgame的IP
        /// </summary>
        public string IP { get; private set; }

        /// <summary>
        ///  Matchmaker派發Matchgame的Port
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Matchmaker派發Matchgame的Pod名稱
        /// </summary>
        public string PodName { get; private set; }

        public void Init(string _dbMapID, string _createID, string _ip, int _port, string _podName) {

            DBMapID = _dbMapID;
            CreaterID = _createID;
            IP = _ip;
            Port = _port;
            PodName = _podName;

            WriteLog.LogColorFormat("設定被分配的房間資料: {0}", WriteLog.LogType.Debug, DebugUtils.ObjToStr(Instance));
        }
    }
}

