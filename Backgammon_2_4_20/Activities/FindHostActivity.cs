using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Backgammon_2_4_20.Networking;
using Backgammon_2_4_20.Adapters;
using static Backgammon_2_4_20.Networking.HostsReceiver;

namespace Backgammon_2_4_20.Activities
{
    [Activity(Label = "FindHostActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class FindHostActivity : Activity
    {
        HostsReceiver hostsReceiver;
        ListView hostsListView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutFindHosts);
            hostsListView = FindViewById<ListView>(Resource.Id.listViewHosts);
            hostsReceiver = new HostsReceiver();
            hostsReceiver.OnHostFound += (sender, e) => { SetListViewAdapter(); };
            FindViewById<Button>(Resource.Id.buttonPlayAsHost).Click += PlayAsHostClick;
        }

        private void PlayAsHostClick(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(GameActivityMultiplayer));
            intent.PutExtra("role", GameActivityMultiplayer.ConnectionRole.Host.ToString());
            StartActivity(intent);
        }
        void SetListViewAdapter()
        {
            HostsAdapter hostsAdapter = new HostsAdapter(this, hostsReceiver.hosts);
            hostsListView.Adapter = hostsAdapter;
        }

        public void JoinHostClick(object sender, EventArgs e)
        {
            HostInfo host = hostsReceiver.hosts[(int)((Button)sender).Tag];
            Intent intent = new Intent(this, typeof(GameActivityMultiplayer));
            intent.PutExtra("role", GameActivityMultiplayer.ConnectionRole.Client.ToString());
            intent.PutExtra("ip", host.ip.ToString());
            intent.PutExtra("port", host.port);
            intent.PutExtra("opponentUsername", host.username);
            StartActivity(intent);
        }
        protected override void OnResume()
        {
            base.OnResume();
            hostsReceiver.ListenToHosts();
        }
        protected override void OnPause()
        {
            hostsReceiver.StopListening();
            base.OnPause();
        }
        protected override void OnDestroy()
        {
            hostsReceiver.Close();
            base.OnDestroy();
        }
    }
}