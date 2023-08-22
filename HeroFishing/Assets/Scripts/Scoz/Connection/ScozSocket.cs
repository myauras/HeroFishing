using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;

namespace Scoz.Func {
    public class ScozSocket {

        protected TcpClient Client;


        public void Init(string _ip, int _port) {
            Client = new TcpClient(_ip, _port);
            NetworkStream stream = Client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);
        }



    }
}