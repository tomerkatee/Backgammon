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
    public class Cube: Rect
    {
        public static float numberDotRadius;
        int numOptions;
        public int CurrentNumber { get; set; }
        Paint paintDots;
        public Cube(int numOptions, float x, float y, float width, float height,  Color backgroundColor, Color dotsColor):base(x, y, width, height, backgroundColor)
        {     
            this.numOptions = numOptions;
            numberDotRadius = width / 10;
            paintDots = new Paint() { Color = dotsColor, TextSize = width / 2 };
        }
        public int Throw(int seed)
        {
            Random r = new Random(seed);
            CurrentNumber = r.Next(1, numOptions + 1);
            return CurrentNumber;
        }
        public void DrawCube(Canvas canvas)
        {
            Draw(canvas);
            DrawNumber(canvas);
        }
        public void DrawNumber(Canvas canvas)
        {
            float offset = numberDotRadius * 2;
            float rightX = x + width / 2 - offset;
            float leftX = x - width / 2 + offset;
            float topY = y - width / 2 + offset;
            float bottomY = y + width / 2 - offset;
            switch (CurrentNumber)
            {
                case 1:
                    canvas.DrawCircle(this.x, this.y, numberDotRadius, paintDots);
                    break;
                case 2:
                    canvas.DrawCircle( leftX,bottomY, numberDotRadius, paintDots);
                    canvas.DrawCircle(rightX,topY, numberDotRadius, paintDots);
                    break;
                case 3:
                    canvas.DrawCircle(leftX, bottomY, numberDotRadius, paintDots);
                    canvas.DrawCircle(x, y, numberDotRadius, paintDots);
                    canvas.DrawCircle(rightX, topY, numberDotRadius, paintDots);
                    break;
                case 4:
                    canvas.DrawCircle(leftX, topY, numberDotRadius, paintDots);
                    canvas.DrawCircle(leftX, bottomY, numberDotRadius, paintDots);
                    canvas.DrawCircle(rightX, topY, numberDotRadius, paintDots);
                    canvas.DrawCircle(rightX, bottomY, numberDotRadius, paintDots);
                    break;
                case 5:
                    canvas.DrawCircle(leftX, topY, numberDotRadius, paintDots);
                    canvas.DrawCircle(leftX, bottomY, numberDotRadius, paintDots);
                    canvas.DrawCircle(x, y, numberDotRadius, paintDots);
                    canvas.DrawCircle(rightX, topY, numberDotRadius, paintDots);
                    canvas.DrawCircle(rightX, bottomY, numberDotRadius, paintDots);
                    break;
                case 6:
                    canvas.DrawCircle(leftX, topY, numberDotRadius, paintDots);
                    canvas.DrawCircle(leftX, y, numberDotRadius, paintDots);
                    canvas.DrawCircle(leftX, bottomY, numberDotRadius, paintDots);
                    canvas.DrawCircle(rightX, topY, numberDotRadius, paintDots);
                    canvas.DrawCircle(rightX, y, numberDotRadius, paintDots);
                    canvas.DrawCircle(rightX, bottomY, numberDotRadius, paintDots);
                    break;

            }
        }

    }
}