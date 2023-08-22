using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace HeroFishing.Socket {
    public abstract class HeroFishingNetwork {

        public enum SendTarget {
            Master,
            Other,
            All,//include self.
            One
        }

        public abstract void Init();
        public abstract void Release();
        public abstract void RegistDisconnectCallback(Action callback);
        public abstract void SetServerIP(string _ip, int _port);
        public abstract void Dispose();
        public abstract void Login(string _name, Action<bool> callback);
        public abstract void CreateRoom(string roomName, Action<bool, string> creatCallback);
        public abstract void LeaveRoom();
        public abstract void Disconnect();
        public abstract bool IsLogin();
        public abstract int GetPing();
    }
}