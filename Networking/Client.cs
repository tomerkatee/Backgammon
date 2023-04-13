using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;

namespace Backgammon_2_4_20.Networking
{
    public class Client: GameNetworkEntity
    {
        public async Task<bool> ConnectToHostSocketAsync(IPAddress hostIP, int hostPort)
        {
            IPEndPoint ipep = new IPEndPoint(hostIP, hostPort);
            try
            {
                await socket.ConnectAsync(ipep);
            }
            catch
            {
                return false;
            }
            gameSocket = socket;
            return true;
        }
    }
}