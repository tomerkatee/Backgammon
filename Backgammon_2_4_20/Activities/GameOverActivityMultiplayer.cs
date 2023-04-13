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
using Backgammon_2_4_20.FirebaseHelpers;
using Backgammon_2_4_20.DataModels;
using Firebase.Firestore;
using Backgammon_2_4_20.BoardViews;

namespace Backgammon_2_4_20.Activities
{
    [Activity(Label = "GameOverActivityMultiplayer", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class GameOverActivityMultiplayer : GameOverActivity
    {
        GameColor myColor;
        string username;
        string opponentUsername;
        bool victory;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            opponentUsername = Intent.GetStringExtra("opponentUsername");
            myColor = GameColorHandler.GetColorFromString(Intent.GetStringExtra("myColor"));
            victory = winnerColor == myColor;
            if (Intent.GetBooleanExtra("opponentLeft", true))
            {
                buttonRestart.Visibility = ViewStates.Gone;
                textViewWinner.Text = opponentUsername + "Left!";
            }
            else
            {
                textViewWinner.Text = victory ? "You Won!" : opponentUsername + " Won!";
            }

            username = FirebaseHandler.GetCurrentUsername();
            if (username != null)
            {
                Match match = new Match();
                match.myColor = myColor;
                match.victory = victory;
                match.date = DateTime.Now;
                match.opponentUsername = opponentUsername;
                match.boardStates = Intent.GetStringArrayExtra("boardStates").ToList().ConvertAll(s => BoardState.Deserialize(s));
                await FirebaseHandler.SaveMatchInHistory(username, match);
            }
        }
    }
}