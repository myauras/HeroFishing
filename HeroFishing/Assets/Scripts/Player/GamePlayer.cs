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
            //var query = RealmManager.MyRealm.All<DBPlayer>();
            //var list = query.ToList();
            //WriteLog.Log(list.Count);
            //foreach (var p in list) {
            //    WriteLog.Log(p);
            //}

            //var query2 = RealmManager.MyRealm.All<DBGameSetting>();
            //var list2 = query2.ToList();
            //WriteLog.Log(list2.Count);
            //foreach (var p in list2) {
            //    WriteLog.Log(p);
            //}

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

    }
}