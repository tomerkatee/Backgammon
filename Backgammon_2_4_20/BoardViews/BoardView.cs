using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;
using Android.Nfc.Tech;
using Android.Support.V4.Content;
using System.Net.NetworkInformation;
using Backgammon_2_4_20.Activities;
using Backgammon_2_4_20.DataModels;
using Backgammon_2_4_20.GameObjects;

namespace Backgammon_2_4_20.BoardViews
{
    public class BoardView : SurfaceView
    {
        float screenWidth, screenHeight;
        List<BackgammonTriangle> triangles;
        Cubes cubes;
        protected SideBar sideBar;
        protected MidBar midBar;
        protected LogicalHandler logicalHandler;
        Paint paintBackground = new Paint();
        public GameColor CurrentPlayer { get { return logicalHandler.CurrentPlayerColor; } }
        public BoardView(Context context, float screenWidth, float screenHeight) : this(context, screenWidth, screenHeight, BoardState.defaultBoardState) { }
        public BoardView(Context context, float screenWidth, float screenHeight, BoardState initialBoardState) : base(context)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            Checker.radius = screenHeight / 25;
            InitializeTriangles();
            midBar = new MidBar(triangles[triangles.Count / 4 - 1].x + BackgammonTriangle.triangleWidth, screenHeight / 2, BackgammonTriangle.triangleWidth, screenHeight, Color.DarkGoldenrod);
            sideBar = new SideBar(BackgammonTriangle.triangleWidth / 2, screenHeight / 2, BackgammonTriangle.triangleWidth, screenHeight, Color.DarkGoldenrod);
            cubes = new Cubes(midBar.x, midBar.y, Color.Red, Color.White);
            logicalHandler = new LogicalHandler(this, triangles, cubes);
            logicalHandler.OnWin += (sender, e) => { Win(e.winnerColor); };
            logicalHandler.OnEndTurn += (sender, e) => { ((GameActivity)Context).ToastCurrentPlayer(); };
            BoardState = initialBoardState;
            SetWillNotDraw(false);
        }
        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            if (canvas != null)
            {
                DrawBackgammonBoard(canvas);
            }
        }
        public void InitializeTriangles()
        {
            triangles = new List<BackgammonTriangle>(24);
            Color color = Color.DarkGray;
            float triangleWidth = (float)(screenWidth / 14);
            float triangleHeight = screenHeight / (float)2.5;
            float ty = screenHeight;
            float tx = triangleWidth / 2 + triangleWidth;
            int direction = -1;
            int i = 0;
            void CreateAndUpdate()
            {
                triangles.Add(new BackgammonTriangle(tx, ty, triangleWidth, triangleHeight, direction, color));
                tx += triangleWidth * -direction;
                color = color == Color.Brown ? Color.DarkGray : Color.Brown;
            }
            for (; i < 6; i++)
            {
                CreateAndUpdate();
            }
            tx += triangleWidth;
            for (; i < 12; i++)
            {
                CreateAndUpdate();
            }
            ty = 0;
            tx -= triangleWidth;
            direction = 1;
            for (; i < 18; i++)
            {
                CreateAndUpdate();
            }
            tx -= triangleWidth;
            for (; i < 24; i++)
            {
                CreateAndUpdate();
            }
        }
        public BoardState BoardState
        {
            get
            {
                return new BoardState(triangles, midBar.CountWhite, midBar.CountBlack, cubes.CurrentNumbers, logicalHandler.CurrentPlayerColor);
            }
            set
            {
                SetBoardByBoardState(value);
            }
        }
        void SetBoardByBoardState(BoardState boardState)
        {
            cubes.SetNumbers(boardState.cubesNumbers);
            for (int i = 0; i < triangles.Count; i++)
            {
                //אם יש שוני או בצבע או במספר אז נחדש
                if((int)triangles[i].CheckersColor != Math.Sign(boardState.trianglesCheckers[i]) || triangles[i].Count != Math.Abs(boardState.trianglesCheckers[i]))
                {
                    int curr = boardState.trianglesCheckers[i];
                    triangles[i].Clear();
                    triangles[i].AddCheckers((GameColor)Math.Sign(curr), Math.Abs(curr));
                }
            }
            midBar.Clear();
            midBar.AddCheckers(GameColor.Black, boardState.blackEaten);
            midBar.AddCheckers(GameColor.White, boardState.whiteEaten);
            logicalHandler.CurrentPlayerColor = boardState.currentPlayerColor;
        }
        protected virtual void DrawBackgammonBoard(Canvas canvas)
        {
            canvas.DrawColor(Color.BurlyWood);
            Bitmap bitmap = MainActivity.BackgroundBitmapDrawable.Bitmap;
            if (bitmap != null)
            {
                canvas.DrawBitmap(Bitmap.CreateScaledBitmap(bitmap, (int)screenWidth, (int)screenHeight, false), 0, 0, paintBackground);
            }
            foreach (BackgammonTriangle triangle in triangles)
            {
                triangle.DrawTriangle(canvas);
                triangle.DrawCheckers(canvas);
            }
            cubes.DrawCubes(canvas);
            midBar.Draw(canvas);
            sideBar.SetIndicatorColor(CurrentPlayer == GameColor.Black ? Color.Black : Color.White);
            sideBar.Draw(canvas);
        }
        public void MarkSideBar()
        {
            sideBar.Mark();
        }
        public void ClearOptions()
        {
            foreach (BackgammonTriangle triangle in triangles)
            {
                triangle.Unmark();
            }
            sideBar.Unmark();          
        }
        public int TouchedTriangleIndex(float x, float y)
        {
            for (int i = 0; i < triangles.Count; i++)
            {
                float offset = BackgammonTriangle.triangleWidth / 2;
                if (x < triangles[i].x + offset && x > triangles[i].x - offset)
                {
                    if (triangles[i].direction == 1)
                    {
                        if (y < BackgammonTriangle.triangleHeight && y > triangles[i].y)
                        {
                            return i;
                        }
                    }
                    else
                    {
                        if (y < triangles[i].y && y > triangles[i].y - BackgammonTriangle.triangleHeight)
                        {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }
        public void RemoveMidBarCheckers(GameColor color, int quantity)
        {
            midBar.RemoveCheckers(color, quantity);
        }
        public void AddMidBarCheckers(GameColor color, int quantity)
        {
            midBar.AddCheckers(color, quantity);
        }
        public void Win(GameColor winnerColor)
        {
            ((GameActivity)Context).Win(winnerColor);
        }
        public virtual void AfterMove() 
        {
            this.Invalidate();
        }
        public override bool OnTouchEvent(MotionEvent e)
        {
            if (e.Action == MotionEventActions.Down)
            {
                if (cubes.TouchedCubes(e.GetX(), e.GetY()))
                {
                    logicalHandler.ManageCubesTouch();
                }
                else if (sideBar.Touching(e.GetX(), e.GetY()))
                {
                    logicalHandler.ManageSideBarTouch();
                }
                else
                {
                    int touchedTriangleIndex = TouchedTriangleIndex(e.GetX(), e.GetY());
                    if (touchedTriangleIndex != -1)
                    {
                        logicalHandler.ManageTriangleTouch(touchedTriangleIndex);
                    }
                    else
                    {
                        logicalHandler.ManageVoidTouch();
                    }
                }
                Invalidate();
            }
            return base.OnTouchEvent(e);
        }
    }
}
