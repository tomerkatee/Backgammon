using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Backgammon_2_4_20.Networking
{
    public class HostPublisher : NetworkEntity, ISender
    {
        string username;
        int port;
        bool publish = true;
        IPEndPoint broadcastEndPoint;
        public HostPublisher(string username, int port) : base()
        {
            this.username = username ?? "guest";
            this.port = port;
            broadcastEndPoint = new IPEndPoint(IPAddress.Broadcast, HostsReceiver.DEFAULT_PORT);
        }
        protected override void InitSocket()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.EnableBroadcast = true;
        }
        public async void PublishHost()
        {
            while (publish)
            {
                try
                {
                    await Task.Delay(2000);
                    SendMessage(username + "&" + port);
                }
                catch { }
            }
        }
        public void StopPublishing()
        {
            publish = false;
        }
        public void SendMessage(string message)
        {
            socket.SendTo(GetMessageBytes(message), broadcastEndPoint);
        }
        public byte[] GetMessageBytes(string message)
        {
            return Encoding.ASCII.GetBytes(message);
        }
        public override void Close()
        {
            StopPublishing();
            base.Close();
        }
    }
}