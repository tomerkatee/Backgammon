using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Backgammon_2_4_20.BoardViews;
using Backgammon_2_4_20.DataModels;

namespace Backgammon_2_4_20
{
    public class Checker
    {
        public static float radius;
        Paint paint;
        public GameColor color;
        public Checker(GameColor color)
        {
            this.color = color;
            paint = new Paint();
            paint.Color = GameColorHandler.GetGraphicsColorByGameColor(color);
        }
        public void Mark()
        {
            Color color = Color.Green;
            paint.Color = color;
        }
        public void Unmark()
        {
            paint.Color = GameColorHandler.GetGraphicsColorByGameColor(color);
        }
        public void DrawChecker(Canvas canvas, float x, float y)
        {
            canvas.DrawCircle(x, y, radius, paint);
        }
    }
}