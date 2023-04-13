using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Backgammon_2_4_20.Activities;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Backgammon_2_4_20.DataModels;
using Backgammon_2_4_20.GameObjects;

namespace Backgammon_2_4_20.BoardViews
{
    public class BoardViewMultiplayer: BoardView
    {
        public GameColor myPlayerColor;
        public bool isValid = false;
        
        public BoardViewMultiplayer(Context context, float screenWidth, float screenHeight): base(context, screenWidth, screenHeight)
        {
            logicalHandler.OnMove += LogicalHandler_onMove;
        }
        public override void AfterMove()
        {
            base.AfterMove();
            if (logicalHandler.CurrentPlayerColor != myPlayerColor)
                ((GameActivityMultiplayer)Context).ReceiveSerialization();
        }
        public bool TryMoveByArgs(Move move)
        {
            return logicalHandler.TryMoveByArgs(move);
        }
        private void LogicalHandler_onMove(object sender, LogicalHandler.MoveEventArgs e)
        {
            if (e.playerColor == myPlayerColor)
            {
                ((GameActivityMultiplayer)Context).SendMove(e.move);
            }
        }
        public override bool OnTouchEvent(MotionEvent e)
        {
            if (myPlayerColor == logicalHandler.CurrentPlayerColor && isValid)
            {
                return base.OnTouchEvent(e);
            }
            return true;
        }

    }
}