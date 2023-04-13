using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Backgammon_2_4_20.Adapters;
using Backgammon_2_4_20.DataModels;
using Backgammon_2_4_20.FirebaseHelpers;
using Firebase.Firestore;
using Backgammon_2_4_20.Listeners;
using System.Threading.Tasks;

namespace Backgammon_2_4_20.Activities
{
    [Activity(Label = "HistoryActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class HistoryActivity : Activity
    {
        ListView matchesListView;
        MatchesAdapter matchesAdapter;
        MatchesEventListener matchesEventListener;
        Spinner spinnerFriends;
        string selectedUsername;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.historyLayout);
            matchesListView = (ListView)FindViewById(Resource.Id.listViewMatches);
            await InitializeSpinner();
            InitializeMatchesEventListener();
            FindViewById<LinearLayout>(Resource.Id.linearLayoutHistory).Background = MainActivity.BackgroundBitmapDrawable;
        }
        async Task InitializeSpinner()
        {
            spinnerFriends = (Spinner)FindViewById(Resource.Id.spinnerHistoryFriends);
            spinnerFriends.ItemSelected += SpinnerFriends_ItemSelected;
            ArrayAdapter arrayAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, await FirebaseHandler.GetCurrentUserFriends());
            arrayAdapter.Add(FirebaseHandler.GetCurrentUsername());
            this.spinnerFriends.Adapter = arrayAdapter;
            selectedUsername = spinnerFriends.SelectedItem.ToString();
        }
        private void SpinnerFriends_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            selectedUsername = spinnerFriends.SelectedItem.ToString();
            InitializeMatchesEventListener();
        }
        void InitializeMatchesEventListener()
        {
            matchesEventListener = new MatchesEventListener(selectedUsername);
            matchesEventListener.onMatchesRetrieved += (sender, e) => { SetListViewAdapter(e.matches); };
        }
        void SetListViewAdapter(List<Match> matches)
        {
            matchesAdapter = new MatchesAdapter(this, matches);
            matchesListView.Adapter = matchesAdapter;
        }
        public void ReplayClick(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ReplayActivity));
            intent.PutExtra("id", ((Button)sender).Tag.ToString());
            intent.PutExtra("username", spinnerFriends.SelectedItem.ToString());
            StartActivity(intent);
        }
    }
}