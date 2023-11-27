using System;
using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket {
    /// <summary>
    /// Socket封包
    /// </summary>
    public class SocketCMD<T> where T : SocketContent {
        public string CMD;
        public int PackID;
        public string ErrMsg;
        public T Content;

        public SocketCMD() {
        }

        public SocketCMD(T content) {
            CMD = content.GetType().Name;
            Content = content;
        }

        public SocketCMD(Type type) {
            Content = (T)Activator.CreateInstance(type);
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
        }
    }
}