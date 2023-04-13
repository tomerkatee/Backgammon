using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using System.Threading;
using Android.Views;
using Android.Widget;

namespace Backgammon_2_4_20
{
    public class Rect
    {
        public float x, y;
        public float width;
        public float height;
        public Paint paint;
        public Rect(float x, float y, float width,float height, Color color)
        {

            this.height = height;
            this.width = width;
            this.x = x;
            this.y = y;
            paint = new Paint();
            paint.Color = color;
        }
        public virtual void Draw(Canvas canvas)
        {
            canvas.DrawRect(GetGraphicsRect(), paint);
        }
        public Android.Graphics.Rect GetGraphicsRect()
        {
            int l;
            int t;
            int r;
            int b;
            l = (int)Math.Round(x - width / 2);
            t = (int)Math.Round(y + height / 2);
            r = (int)Math.Round(x + width / 2);
            b = (int)Math.Round(y - height / 2);
            return new Android.Graphics.Rect(l, t, r, b);
        }
        public bool Touching(float x, float y)
        {
            if (x > this.x - width && x < this.x + width / 2)
            {
                if (y > this.y - height / 2 && y < this.y + height / 2)
                {
                    return true;
                }
            }
            return false;
        }
    }
}