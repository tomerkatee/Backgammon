using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Backgammon_2_4_20.DataModels;
using Backgammon_2_4_20.FirebaseHelpers;
using Backgammon_2_4_20.BoardViews;
using Firebase.Auth;
using System.Threading.Tasks;

namespace Backgammon_2_4_20.Activities
{
    [Activity(Label = "ReplayActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape, Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
    public class ReplayActivity : GameActivity
    {
        BoardViewReplay boardViewReplay;
        string matchId;
        string username;
        protected override void InitializeBoardView()
        {
            Point point = new Point();
            WindowManager.DefaultDisplay.GetSize(point);
            boardView = new BoardViewReplay(this, point.X, point.Y);
            boardViewReplay = (BoardViewReplay)boardView;
        }
        public override void ToastCurrentPlayer()
        {
        }
        protected override async void OnResume()
        {
            base.OnResume();
            matchId = Intent.GetStringExtra("id");
            username = Intent.GetStringExtra("username");
            boardViewReplay.SetBoardStates(await FirebaseHandler.FetchBoardStates(username, matchId));
            boardViewReplay.isValid = true;
        }
    }
}