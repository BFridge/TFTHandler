using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleMThreads
{
    struct ConnectInfo
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public int PassWord { get; set; }

        
        public ConnectInfo(string ip, int port, int passWord)
        {
            this.Ip = ip;
            this.Port = port;
            this.PassWord = passWord;
        }
    }
}
