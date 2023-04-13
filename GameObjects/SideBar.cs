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

namespace Backgammon_2_4_20
{
    public class SideBar: Rect
    {
        Rect indicator;
        Color defaultColor;
        public SideBar(float x, float y, float width, float height, Color color) : base(x, y, width, height, color)
        {
            indicator = new Rect(x, y, width * 0.8f, height / 5, color);
            defaultColor = color;
        }
        public void SetIndicatorColor(Color color)
        {
            indicator.paint.Color = color;
        }
        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);
            indicator.Draw(canvas);
        }
        public void Mark()
        {
            paint.Color = Color.Green;
        }
        public void Unmark()
        {
            paint.Color = defaultColor;
        }
    }
}