using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Backgammon_2_4_20.BoardViews;

namespace Backgammon_2_4_20.DataModels
{
    public class BoardState
    {
        public static BoardState defaultBoardState = new BoardState(new List<int>() { -2, 0, 0, 0, 0, 5, 0, 3, 0, 0, 0, -5, 5, 0, 0, 0, -3, 0, -5, 0, 0, 0, 0, 2 }, 0, 0, (6, 6), GameColor.White);
        //public static BoardState defaultBoardState = new BoardState(new List<int>() { 1, 3, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -2, -2 }, 0, 0, (6, 6), GameColor.White);
        public List<int> trianglesCheckers;
        public int whiteEaten, blackEaten;
        public (int, int) cubesNumbers;
        public GameColor currentPlayerColor;
        public BoardState(List<int> trianglesCheckersCount, int whiteEaten, int blackEaten, (int, int) cubesNumbers, GameColor currentPlayerColor)
        {
            this.trianglesCheckers = trianglesCheckersCount;
            this.whiteEaten = whiteEaten;
            this.blackEaten = blackEaten;
            this.cubesNumbers = cubesNumbers;
            this.currentPlayerColor = currentPlayerColor;
        }
        public BoardState(List<BackgammonTriangle> triangles, int whiteEaten, int blackEaten, (int, int) cubesNumbers, GameColor currentPlayerColor) : 
            this(GetCounts(triangles), whiteEaten, blackEaten, cubesNumbers, currentPlayerColor) { }
        public BoardState() { }
        public string Serialize()
        {
            string trianglesCheckersString = string.Join(",", trianglesCheckers);
            string eatenString = whiteEaten + "," + blackEaten;
            string cubesNumbersString = cubesNumbers.Item1 + "," + cubesNumbers.Item2;
            return trianglesCheckersString + "!" + eatenString + "!" + cubesNumbersString + "!" + currentPlayerColor;
        }
        public static BoardState Deserialize(string serialization)
        {
            BoardState boardState = new BoardState();
            string[] splitted = serialization.Split("!");
            List<string> trianglesCheckersStrings = splitted[0].Split(",").ToList();
            List<int> trianglesCheckers = trianglesCheckersStrings.Select(s => int.Parse(s)).ToList();
            boardState.trianglesCheckers = trianglesCheckers;
            string[] eaten = splitted[1].Split(",");
            boardState.whiteEaten = int.Parse(eaten[0]);
            boardState.blackEaten = int.Parse(eaten[1]);
            string[] cubesNumbers = splitted[2].Split(",");
            boardState.cubesNumbers = (int.Parse(cubesNumbers[0]), int.Parse(cubesNumbers[1]));
            boardState.currentPlayerColor = GameColorHandler.GetColorFromString(splitted[3]);
            return boardState;         
        }

        public static List<int> GetCounts(List<BackgammonTriangle> triangles)
        {
            List<int> trianglesCheckers = new List<int>();
            foreach (BackgammonTriangle triangle in triangles)
            {
                trianglesCheckers.Add(triangle.Count * (int)triangle.CheckersColor);
            }
            return trianglesCheckers;
        }

    }
}