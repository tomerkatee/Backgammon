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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Backgammon_2_4_20.FirebaseHelpers;
using Firebase.Auth;
using System.Runtime.ExceptionServices;
using Android.Accounts;
using System.Net.NetworkInformation;
using Android.Support.Design.Internal;
using Android.Content.Res;
using Xamarin.Essentials;
using Android.Support.Design.Widget;
using Android.Media;
using Backgammon_2_4_20.BoardViews;
using Backgammon_2_4_20.DataModels;

namespace Backgammon_2_4_20
{
    [Activity(Label = "GameActivity",ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape, Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
    public class GameActivity : Activity
    { 
        protected BoardView boardView;
        protected virtual void InitializeBoardView()
        {
            Point point = new Point();
            WindowManager.DefaultDisplay.GetSize(point);
            boardView = new BoardView(this, point.X, point.Y);
        }
        public virtual void Win(GameColor winnerColor)
        {
            Intent intent = new Intent(this, typeof(GameOverActivity));
            intent.PutExtra("winnerColor", winnerColor.ToString());
            StartActivityForResult(intent, 0);
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            bool restart = data.GetBooleanExtra("restart", false);
            if (!restart)
            {
                StartActivity(typeof(MainActivity));
                Finish();
            }
        }
        public virtual void ToastCurrentPlayer()
        {
            RunOnUiThread(() =>
            {
                Toast.MakeText(this, string.Format("{0}, its your turn!", boardView.CurrentPlayer), ToastLength.Short).Show();
            });
        }
        protected override void OnResume()
        {
            base.OnResume();
            InitializeBoardView();
            SetContentView(boardView);
        }
    }
}