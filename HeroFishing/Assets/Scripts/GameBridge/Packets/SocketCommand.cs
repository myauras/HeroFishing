using System;
using System.Collections;
using UnityEngine;

namespace HeroFishing.Socket {
    public class SocketCommand<T> where T : BaseContent {
        public string Command;
        public int PacketID;
        public string ErrMsg;
        public T Content;

        public SocketCommand() {
        }

        public SocketCommand(T content) {
            Command = content.GetType().Name;
            Content = content;
        }

        public SocketCommand(Type type) {
            Content = (T)Activator.CreateInstance(type);
        }
    }

    public class BaseContent {
    }
}