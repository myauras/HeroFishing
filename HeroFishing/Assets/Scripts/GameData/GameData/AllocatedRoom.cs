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
        /// 房間內的所有PlayerID, 索引就是玩家的座位, 一進房間後就不會更動 PlayerIDs[0]就是在座位0玩家的PlayerID
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
        /// 房間內的英雄IDs, 索引就是玩家的座位, 一進房間後就不會更動 所以HeroIDs[0]就是在座位0玩家的英雄ID
        /// </summary>
        public int[] HeroIDs { get; private set; }
        /// <summary>
        /// 玩家自己在房間的索引(座位))
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// 設定被Matchmaker分配到的房間資料，CreateRoom後會從Matchmaker回傳取得此資料
        /// </summary>
        public void InitRoom(string _createID, string[] _playerIDs, string _dbMapID, string _dbMatchgameID, string _ip, int _port, string _podName) {

            CreaterID = _createID;
            PlayerIDs = _playerIDs;
            DBMapID = _dbMapID;
            DBMatchgameID = _dbMatchgameID;
            IP = _ip;
            Port = _port;
            PodName = _podName;
            WriteLog.LogColorFormat("設定被Matchmaker分配到的房間資料: {0}", WriteLog.LogType.Debug, DebugUtils.ObjToStr(Instance));

            var dbPlayerState = GamePlayer.Instance.GetDBPlayerDoc<DBPlayerState>(DBPlayerCol.playerState);
            if (dbPlayerState == null) return;
            dbPlayerState.SetInMatchgameID(DBMatchgameID).Forget();
        }
        /// <summary>
        /// 設定房間內玩家的索引, 也就是玩家的座位, 一進房間後就不會更動
        /// </summary>
        public void SetPlayerIndex(int _playerIndex) {
            Index = _playerIndex;
        }

        /// <summary>
        /// 設定房間內目前使用的英雄IDs, 玩家加進Matchgame後回從Matchgame回傳取得此資料, 索引就是玩家的座位, 一進房間後就不會更動 所以HeroIDs[0]就是在座位0玩家的英雄ID
        /// </summary>
        public void SetHeroID(int _index, int _id) {
            if (HeroIDs == null || HeroIDs.Length == 0)
                HeroIDs = new int[4];
            if (_index < 0 || _index > HeroIDs.Length) {
                WriteLog.LogErrorFormat("傳入的英雄索引錯誤: {0}", _index);
                return;
            }
            HeroIDs[_index] = _id;
        }
        /// <summary>
        /// 清空配對房間(AllocatedRoom)資訊
        /// </summary>
        public void ClearRoom() {
            CreaterID = null;
            PlayerIDs = null;
            DBMapID = null;
            DBMatchgameID = null;
            HeroIDs = null;
            IP = null;
            Port = 0;
            PodName = null;
            WriteLog.LogColorFormat("清空配對房間(AllocatedRoom)資訊: {0}", WriteLog.LogType.Debug, DebugUtils.ObjToStr(Instance));

            var dbPlayerState = GamePlayer.Instance.GetDBPlayerDoc<DBPlayerState>(DBPlayerCol.playerState);
            if (dbPlayerState == null) return;
            dbPlayerState.SetInMatchgameID(null).Forget();
        }
    }
}

