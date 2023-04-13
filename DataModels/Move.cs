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

namespace Backgammon_2_4_20.DataModels
{
    public class Move
    {
        public enum MoveType { CubesThrow, CheckersMove }
        public MoveType moveType;
        public (int, int) move;
        public Move(MoveType type, (int, int) move)
        {
            this.move = move;
            this.moveType = type;
        }
        public string Serialize()
        {
            return moveType.ToString() + "!" + move.Item1 + "," + move.Item2;
        }
        public static Move Deserialize(string serializedString)
        {
            string[] splitted = serializedString.Split("!");
            MoveType moveType = (MoveType)Enum.Parse(typeof(MoveType), splitted[0]);
            string[] moveAsArray = splitted[1].Split(",");
            (int, int) move = (int.Parse(moveAsArray[0]), int.Parse(moveAsArray[1]));
            return new Move(moveType, move);
        }
    }
}