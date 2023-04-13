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
using Backgammon_2_4_20.Activities;
using static Backgammon_2_4_20.Networking.HostsReceiver;

namespace Backgammon_2_4_20.Adapters
{
    class HostsAdapter : BaseAdapter<HostInfo>
    {

        Context context;
        List<HostInfo> hosts;
        public HostsAdapter(Context context, List<HostInfo> hosts)
        {
            this.context = context;
            this.hosts = hosts;
        }


        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LayoutInflater layoutInflater = ((FindHostActivity)context).LayoutInflater;
            View view = layoutInflater.Inflate(Resource.Layout.layoutHostRow, parent, false);
            HostInfo host = hosts[position];
            view.FindViewById<TextView>(Resource.Id.textViewHostUsername).Text = host.username;
            Button buttonJoinHost = view.FindViewById<Button>(Resource.Id.buttonJoinHost);
            buttonJoinHost.Tag = position;
            buttonJoinHost.Click += ((FindHostActivity)context).JoinHostClick;
            return view;
        }
        public override int Count
        {
            get
            {
                return hosts.Count;
            }
        }
        public override HostInfo this[int position]
        {
            get
            {
                return hosts[position];
            }
        }
    }
}