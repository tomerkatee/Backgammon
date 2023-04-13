using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Backgammon_2_4_20.Networking
{
    public class Host : GameNetworkEntity
    {
        public int LocalPort { get; private set; }
        public IPAddress LocalIP { get; private set; }
        public const int DEFAULT_PORT = 30000;
        protected override void InitSocket()
        {
            base.InitSocket();
            LocalIP = FindLocalIP();
            LocalPort = DEFAULT_PORT;
            socket.Bind(new IPEndPoint(LocalIP, LocalPort));
        }
        public async Task<bool> WaitForClient()
        {
            try
            {
                socket.Listen(1);
                gameSocket = await socket.AcceptAsync();
                return true;
            }
            catch 
            {
                return false;
            }
        }
    }
}