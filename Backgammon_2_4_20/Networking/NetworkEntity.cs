using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Net.Sockets;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading;
using Xamarin.Essentials;
using Android.Support.Design.Internal;
using System.Threading.Tasks;

namespace Backgammon_2_4_20.Networking
{
    public abstract class NetworkEntity
    {
        protected byte[] dataBuffer;
        protected Socket socket;
        public NetworkEntity()
        {
            InitSocket();
            dataBuffer = new byte[1024];
        }
        protected abstract void InitSocket();
        public static IPAddress FindLocalIP()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList.ToList().Find(address => address.AddressFamily == AddressFamily.InterNetwork);
        }
        public virtual void Close()
        {
            socket?.Close();
        }
        protected string GetMessageString(int msgLength)
        {
            byte[] receivedBytes = new byte[msgLength];
            Array.Copy(dataBuffer, receivedBytes, msgLength);
            return Encoding.ASCII.GetString(receivedBytes);
        }
    }
}