using Cysharp.Threading.Tasks;
using Scoz.Func;
using Service.Realms;
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

        /// <summary>
        /// 初始化玩家目前所在遊戲房間的資料，CreateRoom後會從Matchmaker回傳取得資料
        /// </summary>
        public void Init(string _createID, string[] _playerIDs, string _dbMapID, string _dbMatchgameID, string _ip, int _port, string _podName) {
            CreaterID = _createID;
            PlayerIDs = _playerIDs;
            DBMapID = _dbMapID;
            DBMatchgameID = _dbMatchgameID;
            IP = _ip;
            Port = _port;
            PodName = _podName;
            WriteLog.LogColorFormat("設定被分配的房間資料: {0}", WriteLog.LogType.Debug, DebugUtils.ObjToStr(Instance));

            var dbPlayerState = GamePlayer.Instance.GetDBPlayerDoc<DBPlayerState>(DBPlayerCol.playerState);
            if (dbPlayerState == null) return;
            dbPlayerState.SetInMatchgameID(DBMatchgameID).Forget();

        }
    }
}

