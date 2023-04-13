using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Backgammon_2_4_20.DownloadBackground
{
    [Service]
    public class DownloadService : Service
    {
        string url;
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            url = intent.GetStringExtra("url");
            Thread downloadThread = new Thread(Download);
            downloadThread.Start();
            return base.OnStartCommand(intent, flags, startId);
        }
        public void Download()
        {
            WebClient webClient = new WebClient();
            byte[] bytes;
            try
            {
                Uri uri = new Uri(url);
                bytes = webClient.DownloadData(uri);
            }
            catch
            {
                return;
            }

            string filePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string fileName = "background.png";
            string fullPath = System.IO.Path.Combine(filePath, fileName);

            FileStream fileStream = new FileStream(fullPath, FileMode.OpenOrCreate);
            fileStream.Write(bytes, 0, bytes.Length);
            fileStream.Close();

            Intent intent = new Intent("download_finished");
            intent.PutExtra("fullPath", fullPath);
            SendBroadcast(intent);
            StopSelf();
        }
    }
}