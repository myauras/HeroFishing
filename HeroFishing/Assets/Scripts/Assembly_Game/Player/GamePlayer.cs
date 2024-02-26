using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using System;
using System.Linq;
using Service.Realms;
using Realms;
using System.Data.SqlTypes;
using Cysharp.Threading.Tasks;
using HeroFishing.Socket;

namespace HeroFishing.Main {

    public partial class GamePlayer : MyPlayer {
        public new static GamePlayer Instance { get; private set; }
        Dictionary<DBPlayerCol, IRealmObject> DBPlayerDatas = new Dictionary<DBPlayerCol, IRealmObject>();
        Dictionary<DBGameSettingDoc, IRealmObject> DBGameSettingDatas = new Dictionary<DBGameSettingDoc, IRealmObject>();

        /// <summary>
        /// 登入後會先存裝置UID到DB，存好後AlreadSetDeviceUID會設為true，所以之後從DB取到的裝置的UID應該都跟目前的裝置一致，若不一致代表是有其他裝置登入同個帳號
        /// </summary>
        public bool AlreadSetDeviceUID { get; set; } = false;

        public GamePlayer()
        : base() {
            Instance = this;
        }
        public override void LoadLocoData() {
            base.LoadLocoData();
            LoadAllDataFromLoco();
        }
        /// <summary>
        /// 初始化玩家自己的資料字典，有錯誤時返回false
        /// </summary>
        public bool InitDBPlayerDocs() {
            DBPlayerDatas.Clear();
            // DBPlayer
            var dbPlayer = RealmManager.MyRealm.Find<DBPlayer>(RealmManager.MyApp.CurrentUser.Id);
            if (dbPlayer == null) {
                WriteLog.LogError("InitDBPlayerDatas時，取得DBPlayer為null");
                return false;
            }
            DBPlayerDatas.Add(DBPlayerCol.player, dbPlayer);

            // DBPlayerState
            var dbPlayerState = RealmManager.MyRealm.Find<DBPlayerState>(RealmManager.MyApp.CurrentUser.Id);
            if (dbPlayerState == null) {
                WriteLog.LogError("InitDBPlayerDatas時，取得DBPlayerState為null");
                return false;
            }
            DBPlayerDatas.Add(DBPlayerCol.playerState, dbPlayerState);

            return true;
        }
        /// <summary>
        /// 取得玩家自己的資料
        /// </summary>
        public T GetDBPlayerDoc<T>(DBPlayerCol _col) where T : class, IRealmObject {
            if (!DBPlayerDatas.ContainsKey(_col)) { WriteLog.LogError("GetDBPlayerData時，要取的資料為null，可能是InitDBPlayerDocs沒有初始化到該資料或是該資料不存在DB中"); return default(T); }
            var result = DBPlayerDatas[_col] as T;
            if (result == null)
                WriteLog.LogError($"Casting 錯誤. 轉型失敗: '{DBPlayerDatas[_col].GetType().Name}' -> '{typeof(T).Name}' 可能是T類型或是DBPlayerCol傳入不一致");
            return result;
        }

        /// <summary>
        /// 初始化遊戲設定資料字典
        /// </summary>
        public void InitDBGameSettingDcos() {
            DBGameSettingDatas.Clear();
            var settings = RealmManager.MyRealm.All<DBGameSetting>();
            if (settings == null) {
                WriteLog.LogError("InitDBDocs時，取得資料為null");
                return;
            }
            foreach (var setting in settings) {
                if (Enum.TryParse<DBGameSettingDoc>(setting.ID, out var _type)) {
                    DBGameSettingDatas[_type] = setting;
                } else {
                    WriteLog.LogErrorFormat("傳入DBGameSetting的_id:{0} 無法轉為DBGameSettingDoc列舉", setting.ID);
                }
            }
        }
        /// <summary>
        /// 取得遊戲設定資料
        /// </summary>
        public T GetDBGameSettingDoc<T>(DBGameSettingDoc _col) where T : class, IRealmObject {
            if (!DBGameSettingDatas.ContainsKey(_col)) { WriteLog.LogError("GetDBGameSettingDoc時，要取的資料為null，可能是InitDBGameSettingDcos沒有初始化到該資料或是該資料不存在DB中"); return default(T); }
            var result = DBGameSettingDatas[_col] as T;
            if (result == null)
                WriteLog.LogError($"Casting 錯誤. 轉型失敗: '{DBGameSettingDatas[_col].GetType().Name}' -> '{typeof(T).Name}' 可能是T類型或是DBPlayerCol傳入不一致");
            return (T)DBGameSettingDatas[_col];
        }

        /// <summary>
        /// 取得目前所在遊戲房資料，沒在遊戲房中就返回null
        /// </summary>
        public async UniTask<DBMatchgame> GetMatchGame() {
            var dbPlayer = GetDBPlayerDoc<DBPlayer>(DBPlayerCol.player);
            if (string.IsNullOrEmpty(dbPlayer.InMatchgameID)) return null;
            var bsonDoc = await RealmManager.Query_GetDoc(DBGameCol.matchgame.ToString(), dbPlayer.InMatchgameID);
            if (bsonDoc == null) return null;
            var dbMatchgame = new DBMatchgame(bsonDoc);
            return dbMatchgame;
        }


    }
}