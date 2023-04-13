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
using Backgammon_2_4_20.DataModels;
using Backgammon_2_4_20.Activities;
using Backgammon_2_4_20.BoardViews;

namespace Backgammon_2_4_20.Adapters
{
    class MatchesAdapter : BaseAdapter<Match>
    {

        Context context;
        List<Match> matches;

        public MatchesAdapter(Context context, List<Match> matches)
        {
            this.context = context;
            this.matches = matches;
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
            LayoutInflater layoutInflater = ((HistoryActivity)context).LayoutInflater;
            View view = layoutInflater.Inflate(Resource.Layout.MatchRow, parent, false);
            ImageView resultImageView = view.FindViewById<ImageView>(Resource.Id.resultImageView);
            ImageView colorImageView = view.FindViewById<ImageView>(Resource.Id.colorImageView);
            TextView opponentTextView = view.FindViewById<TextView>(Resource.Id.opponentTextView);
            TextView dateTextView = view.FindViewById<TextView>(Resource.Id.dateTextView);
            Button buttonReplay = view.FindViewById<Button>(Resource.Id.buttonReplay);
            Match match = matches[position];
            resultImageView.SetImageResource(match.victory ? Resource.Drawable.victory : Resource.Drawable.defeat);
            colorImageView.SetImageResource(match.myColor == GameColor.Black ? Resource.Drawable.black : Resource.Drawable.white);
            opponentTextView.Text = match.opponentUsername;
            dateTextView.Text = match.date.ToShortDateString();
            buttonReplay.Tag = match.id;
            buttonReplay.Click += ((HistoryActivity)context).ReplayClick;
            return view;
        }

        public override int Count
        {
            get
            {
                return matches.Count;
            }
        }
        public override Match this[int position]
        {
            get
            {
                return this.matches[position];
            }
        }
    }
}