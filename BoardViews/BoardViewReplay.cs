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
using Android.Graphics;
using Backgammon_2_4_20.Activities;
using Backgammon_2_4_20.DataModels;
using Backgammon_2_4_20.GameObjects;

namespace Backgammon_2_4_20.BoardViews
{
    public class BoardViewReplay : BoardView
    {
        BoardButton backButton, nextButton;
        public event EventHandler<EventArgs> OnFrameChanged;
        List<BoardState> boardStates;
        public bool isValid = false;
        int currentBoardStateNumber;
        public BoardViewReplay(Context context, float screenWidth, float screenHeight):base(context, screenWidth, screenHeight)
        {
            nextButton = new BoardButton(midBar.x + screenWidth / 3, screenHeight / 2, screenWidth / 13, screenWidth / 26, "next", Color.Black, Color.White);
            backButton = new BoardButton(midBar.x - screenWidth / 3, screenHeight / 2, screenWidth / 13, screenWidth / 26, "back", Color.Black, Color.White);
            OnFrameChanged += BoardViewReplay_onFrameChanged;
        }
        public void SetBoardStates(List<BoardState> moves)
        {
            this.boardStates = moves;
            currentBoardStateNumber = 0;
            BoardState = boardStates[currentBoardStateNumber];
            OnFrameChanged.Invoke(this, new EventArgs());
        }
        private void BoardViewReplay_onFrameChanged(object sender, EventArgs e)
        {
            backButton.Enabled = currentBoardStateNumber > 0;
            nextButton.Enabled = currentBoardStateNumber < boardStates.Count - 1;
            Invalidate();
        }

        protected override void DrawBackgammonBoard(Canvas canvas)
        {
            base.DrawBackgammonBoard(canvas);
            if (backButton.Enabled)
            {
                backButton.Draw(canvas);
            }
            if (nextButton.Enabled)
            {
                nextButton.Draw(canvas);
            }
        }
        public override bool OnTouchEvent(MotionEvent e)
        {
            if (isValid && e.Action == MotionEventActions.Down)
            {
                if (backButton.Enabled && backButton.Touching(e.GetX(), e.GetY()))
                {
                    BoardState = boardStates[--currentBoardStateNumber];
                    OnFrameChanged.Invoke(this, new EventArgs());
                }
                else if (nextButton.Enabled && nextButton.Touching(e.GetX(), e.GetY()))
                {
                    BoardState = boardStates[++currentBoardStateNumber];
                    OnFrameChanged.Invoke(this, new EventArgs());
                }
            }
            return true;
        }

    }
}