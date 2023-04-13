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
using Firebase.Firestore;
using Backgammon_2_4_20.DataModels;
using Backgammon_2_4_20.BoardViews;
using Backgammon_2_4_20.FirebaseHelpers;

namespace Backgammon_2_4_20.Listeners
{
    public class MatchesEventListener : Java.Lang.Object, IEventListener
    {
        public event EventHandler<MatchesEventArgs> onMatchesRetrieved;
        public class MatchesEventArgs : EventArgs
        {
            public List<Match> matches;
        }

        public MatchesEventListener(string username)
        {
            ListenToUserMatches(username);
        }
        async void ListenToUserMatches(string username)
        {
            (await FirebaseHandler.GetUserMatches(username)).AddSnapshotListener(this);
        }
        public void OnEvent(Java.Lang.Object value, FirebaseFirestoreException error)
        {
            QuerySnapshot querySnapshot = (QuerySnapshot)value;
            MatchesEventArgs matchesEventArgs = new MatchesEventArgs();
            List<Match> matches = new List<Match>();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Match match = new Match();
                match.date = DateTime.Parse(documentSnapshot.GetString("date"));
                match.myColor = GameColorHandler.GetColorFromString(documentSnapshot.GetString("myColor"));
                match.victory = (bool)documentSnapshot.GetBoolean("victory");
                match.opponentUsername = documentSnapshot.GetString("opponentUsername");
                match.id = documentSnapshot.Id;
                matches.Add(match);
            }
            matchesEventArgs.matches = matches.OrderByDescending(m => m.date).ToList();
            onMatchesRetrieved?.Invoke(this, matchesEventArgs);
        }
    }
}