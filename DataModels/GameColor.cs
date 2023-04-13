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
    public enum GameColor
    {
        Black = -1, White = 1, None = 0
    }
    public static class GameColorHandler
    {
        public static GameColor OppositeColor(GameColor color)
        {
            return color == GameColor.Black ? GameColor.White : GameColor.Black;
        }
        public static GameColor GetColorFromString(string color)
        {
            return (GameColor)Enum.Parse(typeof(GameColor), color);
        }
        public static GameColor PickRandomColor()
        {
            return DateTime.Now.Millisecond % 2 == 0 ? GameColor.Black : GameColor.White;
        }
        public static Color GetGraphicsColorByGameColor(GameColor color)
        {
            return color == GameColor.Black ? Color.Black : Color.White;
        }
    }

}