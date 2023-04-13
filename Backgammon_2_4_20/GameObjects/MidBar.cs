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
using Backgammon_2_4_20.BoardViews;
using Backgammon_2_4_20.DataModels;

namespace Backgammon_2_4_20
{
    public class MidBar: Rect
    {
        List<Checker> blackCheckers;
        List<Checker> whiteCheckers;
        float initialBlackCheckersY;
        float initialWhiteCheckersY;
        Dictionary<GameColor, List<Checker>> listsDict;
        public MidBar(float x, float y, float width, float height, Color color) : base(x, y, width, height, color)
        {
            blackCheckers = new List<Checker>();
            whiteCheckers = new List<Checker>();
            listsDict = new Dictionary<GameColor, List<Checker>>() { { GameColor.Black, blackCheckers }, { GameColor.White, whiteCheckers } };
            initialBlackCheckersY = height / 3;
            initialWhiteCheckersY = height / 3 * 2;
        }
        public void AddCheckers(GameColor color, int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                listsDict[color].Add(new Checker(color));
            }
        }
        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);
            DrawCheckers(canvas);
        }
        public void DrawCheckers(Canvas canvas)
        {
            for (int i = 0; i < blackCheckers.Count; i++)
            {
                blackCheckers[i].DrawChecker(canvas, x, initialBlackCheckersY + Checker.radius * 2 * i);
            }
            for (int i = 0; i < whiteCheckers.Count; i++)
            {
                whiteCheckers[i].DrawChecker(canvas, x, initialWhiteCheckersY - Checker.radius * 2 * i);
            }
        }
        public void RemoveCheckers(GameColor color, int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                listsDict[color].RemoveAt(listsDict[color].Count - 1);
            }
        }
        public void Clear()
        {
            blackCheckers.Clear();
            whiteCheckers.Clear();
        }
        public int CountBlack
        {
            get
            {
                return blackCheckers.Count;
            }
        }
        public int CountWhite
        {
            get
            {
                return whiteCheckers.Count;
            }
        }
    }
}