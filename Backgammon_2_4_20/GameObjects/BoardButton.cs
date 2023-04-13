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

namespace Backgammon_2_4_20
{
    public class BoardButton: Rect
    {
        string text;
        Paint paintText;
        public bool Enabled { get; set; } = true;
        public BoardButton(float x, float y, float width, float height, string text, Color backgroundColor, Color textColor): base(x,y,width,height,backgroundColor)
        {
            this.text = text;
            paintText = new Paint() { TextSize = 50, Color = textColor };
        }
        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);
            canvas.DrawText(text, x - width / 4, y + height / 4, paintText);
        }
    }
}