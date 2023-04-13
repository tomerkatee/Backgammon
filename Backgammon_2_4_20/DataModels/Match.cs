using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text.Format;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using Backgammon_2_4_20.BoardViews;

namespace Backgammon_2_4_20.DataModels
{
    public class Match
    {
        public bool victory;
        public DateTime date;
        public GameColor myColor;
        public string opponentUsername;
        public string id;
        public List<BoardState> boardStates;
    }
}