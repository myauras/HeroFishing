using System;
using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket {
    /// <summary>
    /// Socket封包
    /// </summary>
    public class SocketCMD<T> where T : SocketContent {
        public string CMD { get; private set; }
        public int PackID { get; private set; }
        public string ErrMsg { get; private set; }
        public string ConnToken { get; private set; }
        public T Content;

        public SocketCMD() {
        }

        public SocketCMD(T _content) {
            CMD = _content.GetType().Name;
            Content = _content;
        }
        public void SetPackID(int _value) {
            PackID = _value;
        }
        public void SetConnToken(string _value) {
            ConnToken = _value;
        }
        public SocketCMD(Type _type) {
            Content = (T)Activator.CreateInstance(_type);
        }
    }

    /// <summary>
    /// Socket封包的內容
    /// </summary>
    public class SocketContent {

        public enum MatchmakerCMD_TCP {
            AUTH_TOCLIENT,//身分驗證
            CREATEROOM_TOCLIENT,//加房間or開房
        }
        public enum MatchgameCMD_TCP {
            AUTH_TOCLIENT, //身分驗證
            SETHERO_TOCLIENT,//設定玩家英雄
            ATTACK_TOCLIENT,//攻擊
            HIT_TOCLIENT,//擊中
            UPDATEPLAYER_TOCLIENT,//更新玩家
            SPAWN_TOCLIENT,//生怪
            UPDATESCENE_TOCLIENT,//場景狀態更新
            MONSTERDIE_TOCLIENT,//怪物死亡時送Client
            AUTO_TOCLIENT,//Auto模式
            LEAVE_TOCLIENT,//離開遊戲
            LVUPSPELL,//升級技能
            LVUPSPELL_TOCLIENT, //升級技能結果回傳
            DROPSPELL,//掉落施法
            DROPSPELL_TOCLIENT,//掉落施法-送Client
            ADDBOT,//加入電腦
            ADDBOT_TOCLIENT,//加入電腦-送Client
        }
        public enum MatchgameCMD_UDP {
            //ATTACK_TOCLIENT,//攻擊
            UPDATEGAME_TOCLIENT,//遊戲狀態更新
            UPDATEPLAYER_TOCLIENT,//更新玩家
            UPDATESCENE_TOCLIENT,//場景狀態更新
        }
    }
}