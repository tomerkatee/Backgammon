using System;
using System.Linq;
using System.Text;
using Android.Graphics;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using Backgammon_2_4_20.BoardViews;
using Backgammon_2_4_20.DataModels;

namespace Backgammon_2_4_20
{
    public class BackgammonTriangle
    {
        public float x { get; }
        public float y { get; }
        float initialCheckersY;
        public static float triangleWidth;
        public static float triangleHeight;
        Paint paint;
        public Color defaultColor { get; }
        public int direction { get; }
        List<Checker> checkers;
        public BackgammonTriangle(float x, float y, float width, float height, int direction, Color color)
        {           
            triangleWidth = width;
            triangleHeight = height;
            this.x = x;
            this.y = y;
            this.direction = direction;
            this.defaultColor = color;
            this.paint = new Paint() { Color = color };
            checkers = new List<Checker>();
            initialCheckersY = y + Checker.radius * direction;
        }
        public void DrawTriangle(Canvas canvas)
        {
            Path path = new Path();
            path.MoveTo(x, y);
            path.RLineTo(-triangleWidth / 2, 0);
            path.RLineTo(triangleWidth / 2, triangleHeight * direction);
            path.RLineTo(triangleWidth / 2, -triangleHeight * direction);
            path.RLineTo(-triangleWidth / 2, 0);
            canvas.DrawPath(path, paint);

        }
        public void Mark()
        {
            paint.Color = Color.Green;
        }
        public void Unmark()
        {
            paint.Color = defaultColor;
            checkers.ForEach(checker => checker.Unmark());
        }
        public void DrawCheckers(Canvas canvas)
        {
            for (int i = 0; i < checkers.Count; i++)
            {
                checkers[i].DrawChecker(canvas, x, initialCheckersY + Checker.radius * 2 * i * direction);
            }
        }
        public void AddCheckers(GameColor color, int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                checkers.Add(new Checker(color));
            }
        }
        public void RemoveCheckers(int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                checkers.RemoveAt(checkers.Count - 1);
            }
        }
        public void Clear()
        {
            checkers.Clear();
        }
        public void MarkFirstChecker()
        {
            checkers.Last().Mark();
        }
        public int Count
        {
            get
            {
                return checkers.Count;
            }
        }
        public bool IsClear
        {
            get
            {
                return checkers.Count == 0;
            }
        }
        public GameColor CheckersColor
        {
            get
            {
                if (IsClear)
                    return GameColor.None;
                return checkers.First().color;
            }
        }
    }
}