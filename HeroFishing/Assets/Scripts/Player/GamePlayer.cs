using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scoz.Func;
using System;
using System.Linq;
using Service.Realms;
using Realms;

namespace HeroFishing.Main {

    public partial class GamePlayer : MyPlayer {
        public new static GamePlayer Instance { get; private set; }
        Dictionary<DBPlayerCol, IRealmObject> PlayerDatas = new Dictionary<DBPlayerCol, IRealmObject>();
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
            PlayerDatas.Clear();
            DBPlayer myPlayer = RealmManager.MyRealm.Find<DBPlayer>(RealmManager.MyApp.CurrentUser.Id);
            if (myPlayer == null) {
                WriteLog.LogError("InitDBPlayerDatas時，取得myPlayer為null");
                return false;
            }
            PlayerDatas.Add(DBPlayerCol.player, myPlayer);
            return true;
        }
        /// <summary>
        /// 取得玩家自己的資料
        /// </summary>
        public T GetDBPlayerDoc<T>(DBPlayerCol _col) {
            if (!PlayerDatas.ContainsKey(_col)) WriteLog.LogError("GetDBPlayerData時，要取的資料為null");
            return (T)PlayerDatas[_col];
        }

        /// <summary>
        /// 初始化遊戲設定資料字典，有錯誤時返回false
        /// </summary>
        public bool InitDBGameSettingDcos() {
            DBGameSettingDatas.Clear();
            var settings = RealmManager.MyRealm.All<DBGameSetting>();
            if (settings == null) {
                WriteLog.LogError("InitDBDocs時，取得資料為null");
                return false;
            }
            foreach (var setting in settings) {
                if (MyEnum.TryParseEnum<DBGameSettingDoc>(setting.ID, out var _type)) {
                    DBGameSettingDatas[_type] = setting;
                }
            }
            return true;
        }
        /// <summary>
        /// 取得遊戲設定資料
        /// </summary>
        public T GetDBGameSettingDoc<T>(DBGameSettingDoc _col) {
            if (!DBGameSettingDatas.ContainsKey(_col)) WriteLog.LogError("GetDBGameSettingDoc時，要取的資料為null");
            return (T)DBGameSettingDatas[_col];
        }
    }
}