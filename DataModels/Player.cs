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

namespace Backgammon_2_4_20.DataModels
{
    public class Player
    {
        public int chosenTriangleIndex;
        public GameColor color;
        public int Direction { get; }
        public bool thrownCubes = false;
        public List<int> availableTurns;
        public int eaten = 0;
        public Player(GameColor color)
        {
            this.color = color;
            Direction = color == GameColor.Black ? 1 : -1;
        }
    }
}