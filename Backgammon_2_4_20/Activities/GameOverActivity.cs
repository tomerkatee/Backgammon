using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Backgammon_2_4_20.DataModels;
using Backgammon_2_4_20.FirebaseHelpers;
using Firebase.Firestore;
using Backgammon_2_4_20.BoardViews;

namespace Backgammon_2_4_20
{
    [Activity(Label = "GameOverActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class GameOverActivity : Activity
    {
        protected TextView textViewWinner;
        protected Button buttonRestart;
        Button buttonBackToMenu;
        protected GameColor winnerColor;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.SetContentView(Resource.Layout.layoutGameOver);
            textViewWinner = FindViewById<TextView>(Resource.Id.winnerTextView);
            winnerColor = GameColorHandler.GetColorFromString(Intent.GetStringExtra("winnerColor"));
            textViewWinner.Text = string.Format("{0} Wins!", winnerColor);
            buttonRestart = (Button)this.FindViewById(Resource.Id.restartButton);
            buttonBackToMenu = (Button)this.FindViewById(Resource.Id.buttonBackToMenu);
            buttonBackToMenu.Click += (sender, e) => { FinishActivityByResult(false); };
            buttonRestart.Click += (sender, e) => { FinishActivityByResult(true); };
        }
        void FinishActivityByResult(bool restart)
        {
            Intent intent = new Intent();
            intent.PutExtra("restart", restart);
            SetResult(0, intent);
            Finish();
        }
        public override void OnBackPressed()
        {
        }
    }
}