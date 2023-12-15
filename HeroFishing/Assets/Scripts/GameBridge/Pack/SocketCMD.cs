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
            AUTH_TOCLIENT,
            CREATEROOM_TOCLIENT,
        }
        public enum MatchgameCMD_TCP {
            AUTH_TOCLIENT,
            SETHERO_TOCLIENT,
            HIT_TOCLIENT,
            UPDATEPLAYER_TOCLIENT,
            SPAWN_TOCLIENT,
        }
        public enum MatchgameCMD_UDP {
            UPDATEGAME_TOCLIENT,
            UPDATEPLAYER_TOCLIENT,
            UPDATESCENE_TOCLIENT,
        }
    }
}