using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Backgammon_2_4_20.DownloadBackground
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { "download_finished" })]
    public class DownloadReceiver : BroadcastReceiver
    {
        public event EventHandler<ReceiveEventArgs> OnImageReceived;
        public class ReceiveEventArgs : EventArgs
        {
            public string path;
            public ReceiveEventArgs(string path)
            {
                this.path = path;
            }
        }
        public override void OnReceive(Context context, Intent intent)
        {
            string fullPath = intent.GetStringExtra("fullPath");
            OnImageReceived?.Invoke(this, new ReceiveEventArgs(fullPath));
        }
    }
}