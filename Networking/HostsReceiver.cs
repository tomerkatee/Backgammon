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
    public class HostsReceiver: NetworkEntity, IReceiver
    {
        public List<HostInfo> hosts = new List<HostInfo>();
        public event EventHandler<HostEventArgs> OnHostFound;
        public const int DEFAULT_PORT = 8000;
        EndPoint broadcastRemoteEP;
        bool listen = true;
        public class HostEventArgs : EventArgs
        {
            public HostInfo hostInfo;
            public HostEventArgs(HostInfo hostInfo)
            {
                this.hostInfo = hostInfo;
            }
        }
        public struct HostInfo
        {
            public IPAddress ip;
            public int port;
            public string username;
        }
        protected override void InitSocket()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), DEFAULT_PORT));
        }
        public async void ListenToHosts()
        {
            listen = true;
            while (listen)
            {
                try
                {
                    broadcastRemoteEP = new IPEndPoint(IPAddress.Any, DEFAULT_PORT);
                    string message = await Task.Run(ReceiveMessage);
                    string[] splitted = message.Split("&");
                    string username = splitted[0];
                    int port = int.Parse(splitted[1]);
                    IPEndPoint ipep = (IPEndPoint)broadcastRemoteEP;
                    IPAddress ip = ipep.Address;
                    if (!hosts.Exists(h => h.ip.ToString() == ipep.Address.ToString()) && ip.ToString() != FindLocalIP().ToString())
                    {
                        HostInfo hostInfo = new HostInfo();
                        hostInfo.ip = ip;
                        hostInfo.port = port;
                        hostInfo.username = username;
                        hosts.Add(hostInfo);
                        OnHostFound?.Invoke(this, new HostEventArgs(hostInfo));
                    }
                }
                catch { }
            }
        }
        public string ReceiveMessage()
        {
            return ReceiveBytesAsString(dataBuffer.Length);
            //int lengthSize = int.Parse(ReceiveBytesAsString(1));
            //int length = int.Parse(ReceiveBytesAsString(lengthSize));
            //return ReceiveBytesAsString(length);
        }
        public string ReceiveBytesAsString(int size)
        {
            int receivedLength = socket.ReceiveFrom(dataBuffer, size, SocketFlags.None, ref broadcastRemoteEP);
            return GetMessageString(receivedLength);
        }
        public void StopListening()
        {
            listen = false;
        }
        public override void Close()
        {
            StopListening();
            base.Close();
        }
    }
}