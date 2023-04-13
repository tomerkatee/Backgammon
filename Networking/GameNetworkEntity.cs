using System;
using System.Collections.Generic;
using System.Linq;
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
    public abstract class GameNetworkEntity : NetworkEntity, IReceiver, ISender
    {
        protected Socket gameSocket;
        public bool Connected { get { return gameSocket != null && gameSocket.Connected; } }
        protected override void InitSocket()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        public string ReceiveBytesAsString(int size)
        {
            int receivedLength = gameSocket.Receive(dataBuffer, size, SocketFlags.None);
            return GetMessageString(receivedLength);
        }

        public string ReceiveMessage()
        {
            int lengthSize = int.Parse(ReceiveBytesAsString(1));
            int length = int.Parse(ReceiveBytesAsString(lengthSize));
            return ReceiveBytesAsString(length);
        }

        public void SendMessage(string message)
        {
            gameSocket.Send(GetMessageBytes(message));
        }
        public async Task SendMessageAsync(string message)
        {
            await Task.Run(() => SendMessage(message));
        }
        public async Task<string> ReceiveMessageAsync()
        {
            return await Task.Run(ReceiveMessage);
        }
        public byte[] GetMessageBytes(string message)
        {
            string messageLength = message.Length.ToString();
            return Encoding.ASCII.GetBytes(messageLength.Length + messageLength + message);
        }
        public override void Close()
        {
            base.Close();
            gameSocket?.Close();
        }
    }
}