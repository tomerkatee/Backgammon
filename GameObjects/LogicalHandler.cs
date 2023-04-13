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
using Backgammon_2_4_20.DataModels;
using Backgammon_2_4_20.BoardViews;
using static Backgammon_2_4_20.BoardViews.BoardView;

namespace Backgammon_2_4_20.GameObjects
{
    public class LogicalHandler
    {
        const int NONE = -3;
        Player blackPlayer;
        Player whitePlayer;
        Player currentPlayer;
        public GameColor CurrentPlayerColor
        {
            get { return currentPlayer.color; }
            set { currentPlayer = value == GameColor.White ? whitePlayer : blackPlayer; }
        }
        BoardView boardView;
        List<BackgammonTriangle> triangles;
        Cubes cubes;
        public event EventHandler<MoveEventArgs> OnMove;
        public event EventHandler<EndTurnEventArgs> OnEndTurn;
        public event EventHandler<WinEventArgs> OnWin;
        public class WinEventArgs: EventArgs
        {
            public GameColor winnerColor;
        }
        public class MoveEventArgs : EventArgs
        {
            public Move move;
            public GameColor playerColor;
            public bool movedManually;
            public MoveEventArgs(Move.MoveType type, (int, int) move, GameColor playerColor, bool movedManually)
            {
                this.move = new Move(type, move);
                this.playerColor = playerColor;
                this.movedManually = movedManually;
            }
        }
        public class EndTurnEventArgs
        {
            public GameColor currentPlayerColor;
        }
        public LogicalHandler(BoardView boardView, List<BackgammonTriangle> triangles, Cubes cubes)
        {
            this.boardView = boardView;
            this.triangles = triangles;
            this.cubes = cubes;
            blackPlayer = new Player(GameColor.Black);
            blackPlayer.chosenTriangleIndex = NONE;
            whitePlayer = new Player(GameColor.White);
            whitePlayer.chosenTriangleIndex = NONE;
            currentPlayer = whitePlayer;
            OnMove += LogicalHandler_onMove;
        }
        //**************************************************************************
        void LogicalHandler_onMove(object sender, MoveEventArgs e)
        {
            if (IsWinning())
            {
                OnWin.Invoke(this, new WinEventArgs() { winnerColor = currentPlayer.color });
                return;
            }
            if (ReadyForTakeOut())
            {
                FixMovesByMax(MaxForTakeOut() + 1);
            }
            if (NoAvailableMoves())
            {
                EndTurnAndAdvance();
            }
            else if(e.movedManually) 
                switch (e.move.moveType)
                {
                    case DataModels.Move.MoveType.CubesThrow:
                        if (currentPlayer.eaten > 0)
                        {
                            currentPlayer.chosenTriangleIndex = currentPlayer == blackPlayer ? -1 : triangles.Count;
                            MarkPossibleMoves(currentPlayer.chosenTriangleIndex);
                        }
                        else MarkPlayableCheckers();
                        break;
                    case DataModels.Move.MoveType.CheckersMove:
                        if (!PlayableTriangle(currentPlayer.chosenTriangleIndex))
                        {
                            Cancel();
                            MarkPlayableCheckers();
                        }
                        else
                        {
                            ChooseTriangle(currentPlayer.chosenTriangleIndex);
                        }
                        break;
                }
            boardView.AfterMove();
        }
        //**************************************************************************
        public void Move(int currentIndex, int targetIndex, List<List<int>> possibleMoves, bool movedManually)
        {
            int distance = Math.Abs(currentIndex - targetIndex);
            List<int> matchingMoves = FindMatchingMoves(distance, possibleMoves);
            int tempIndex = currentIndex;
            foreach (int move in matchingMoves)
            {
                currentPlayer.availableTurns.Remove(move);
                MoveChecker(tempIndex, tempIndex + move * currentPlayer.Direction);
                tempIndex += move * currentPlayer.Direction;
            }
            OnMove.Invoke(this, new MoveEventArgs(DataModels.Move.MoveType.CheckersMove, (currentIndex, targetIndex), currentPlayer.color, movedManually));
        }
        public static List<int> FindMatchingMoves(int distance, List<List<int>> moves)//מכל המהלכים הזמינים, מצא את הכי מתאים
        {
            List<List<int>> filteredMoves = moves.FindAll(l => l.Sum() == distance);
            filteredMoves = filteredMoves.OrderBy(l => l.Count).ToList();
            List<int> matchingMovesUnordered = filteredMoves[0];
            filteredMoves = filteredMoves.FindAll(l => EqualMoves(l, matchingMovesUnordered));
            for (int i = 0; i < matchingMovesUnordered.Count; i++)
            {
                filteredMoves = filteredMoves.FindAll(l => l[i] == filteredMoves.Max(a => a[i]));
            }
            return filteredMoves[0];

            bool EqualMoves(List<int> l1, List<int> l2)
            {
                foreach (int n in l1)
                {
                    if (!l2.Contains(n))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public void MoveChecker(int currentIndex, int targetIndex)
        {
            if (IsSideBarIndex(currentIndex))
            {
                boardView.RemoveMidBarCheckers(currentPlayer.color, 1);
                currentPlayer.eaten--;
            }
            else
            {
                triangles[currentIndex].RemoveCheckers(1);
            }
            if (!IsSideBarIndex(targetIndex))
            {
                if (Edible(targetIndex))
                {
                    Eat(targetIndex);
                }
                triangles[targetIndex].AddCheckers(currentPlayer.color, 1);
            }
        }
        bool Edible(int index)
        {
            return triangles[index].Count == 1 && triangles[index].CheckersColor != currentPlayer.color;
        }
        void Eat(int index)
        {
            triangles[index].Clear();
            Player otherPlayer = currentPlayer == blackPlayer ? whitePlayer : blackPlayer;
            otherPlayer.eaten++;
            boardView.AddMidBarCheckers(otherPlayer.color, 1);
        }
        //*******************************************************************
        public bool AvailableSpotForMovement(int index)
        {
            if ((currentPlayer.Direction == -1 && index < -1) || (currentPlayer.Direction == 1 && index > triangles.Count))
            {
                return false;
            }
            if (IsSideBarIndex(index))
            {
                return ReadyForTakeOut();
            }
            BackgammonTriangle triangle = triangles[index];
            if (triangle.IsClear)
            {
                return true;
            }
            if (triangle.CheckersColor == currentPlayer.color)
            {
                return true;
            }
            if (triangle.Count == 1)
            {
                return true;
            }
            return false;
        }
        List<List<int>> GetPossibleMoves(List<int> availableTurns, int startIndex)
        {
            List<List<int>> possibleMoves = GetPossibleMoves(availableTurns, new List<int>(), startIndex, currentPlayer.Direction);
            if (currentPlayer.eaten > 1)
            {
                return possibleMoves.FindAll(l => l.Count == 1);
            }
            return possibleMoves;
        }
        List<List<int>> GetPossibleMoves(List<int> availableTurns, List<int> path, int currIndex, int direction)
        {
            List<List<int>> moves = new List<List<int>>();
            for (int i = 0; i < availableTurns.Count; i++)
            {
                int nextIndex = currIndex + availableTurns[i] * direction;
                if (AvailableSpotForMovement(nextIndex))
                {
                    List<int> availableTurnsCopy = new List<int>(availableTurns);
                    List<int> pathCopy = new List<int>(path);
                    pathCopy.Add(availableTurnsCopy[i]);
                    moves.Add(pathCopy);
                    availableTurnsCopy.RemoveAt(i);
                    moves.AddRange(GetPossibleMoves(availableTurnsCopy, pathCopy, nextIndex, direction));
                }
            }
            return moves;
        }
        //****************************************************************
        bool PlayableTriangle(int triangleIndex)
        {
            if (IsSideBarIndex(triangleIndex))
            {
                return currentPlayer.eaten > 0 && GetPossibleMoves(currentPlayer.availableTurns, GetSideBarIndexByColor()).Count != 0;
            }
            if (triangles[triangleIndex].CheckersColor != currentPlayer.color)
            {
                return false;
            }
            return GetPossibleMoves(currentPlayer.availableTurns, triangleIndex).Count != 0;
        }
        List<int> PlayableTrianglesIndexes()
        {
            return Enumerable.Range(0, triangles.Count).ToList().FindAll(i => PlayableTriangle(i));
        }
        void MarkPlayableCheckers()
        {
            PlayableTrianglesIndexes().ForEach(i => triangles[i].MarkFirstChecker());
        }
        //************************************************************
        public void ManageCubesTouch()
        {
            if (!currentPlayer.thrownCubes)
            {
                cubes.ThrowCubes();
                currentPlayer.thrownCubes = true;
                currentPlayer.availableTurns = cubes.GetMovesAvailable();
                OnMove.Invoke(this, new MoveEventArgs(DataModels.Move.MoveType.CubesThrow, cubes.CurrentNumbers, currentPlayer.color, true));
            }
        }
        void FixMovesByMax(int max)
        {
            for (int i = 0; i < currentPlayer.availableTurns.Count; i++)
            {
                if (currentPlayer.availableTurns[i] > max)
                {
                    currentPlayer.availableTurns[i] = max;
                }
            }
        }
        int MaxForTakeOut()
        {
            int startIndex = currentPlayer == whitePlayer ? 0 : triangles.Count - 1;
            int max = 0;
            for(int i = 0; i < 6; i++)
            {
                int currIndex = startIndex - i * currentPlayer.Direction;
                if (triangles[currIndex].CheckersColor == CurrentPlayerColor)
                    max = i;
            }
            return max;
        }
        bool ReadyForTakeOut()
        {   // התחלת הסריקה מתבצעת מתחילת השטח שהוא לא הבסיס, בהתאם לצבע השחקן
            int startIndex = currentPlayer == whitePlayer ? triangles.Count / 4 : 0;
            for (int i = 0; i < triangles.Count * 3 / 4; i++)
            {
                if(triangles[startIndex + i].CheckersColor == CurrentPlayerColor)
                    return false;
            }
            return true;
        }
        bool IsWinning()
        {
            foreach (BackgammonTriangle triangle in triangles)
            {
                if(triangle.CheckersColor == CurrentPlayerColor)
                    return false;
            }
            return true;
        }
        void EndTurnAndAdvance()
        {
            Cancel();
            currentPlayer.thrownCubes = false;
            currentPlayer.availableTurns.Clear();
            currentPlayer = currentPlayer == blackPlayer ? whitePlayer : blackPlayer;
            OnEndTurn?.Invoke(this, new EndTurnEventArgs() { currentPlayerColor = currentPlayer.color });
        }
        bool NoAvailableMoves()
        {
            if (currentPlayer.eaten > 0)
            {
                return !PlayableTriangle(GetSideBarIndexByColor());
            }
            return currentPlayer.availableTurns.Count == 0 || PlayableTrianglesIndexes().Count == 0;
        }
        //*********************************************************
        void MarkPossibleMoves(int startIndex)
        {
            List<List<int>> possibleMoves = GetPossibleMoves(currentPlayer.availableTurns, startIndex);
            foreach (List<int> list in possibleMoves)
            {
                int curr = startIndex;
                foreach (int n in list)
                {
                    curr += n * currentPlayer.Direction;
                    if (curr == GetSideBarIndexByColor())
                    {
                        boardView.MarkSideBar();
                    }
                    else
                    {
                        if (curr == -1)
                            boardView.MarkSideBar();
                        else
                            triangles[curr].Mark();                         
                    }
                }
            }
        }
        //********************************************************************************
        void ChooseTriangle(int index)
        {
            boardView.ClearOptions();
            currentPlayer.chosenTriangleIndex = index;
            if (!IsSideBarIndex(index))
            {
                triangles[index].MarkFirstChecker();
            }
            MarkPossibleMoves(currentPlayer.chosenTriangleIndex);
        }
        void Cancel()
        {
            boardView.ClearOptions();
            currentPlayer.chosenTriangleIndex = NONE;
        }
        //******************************************
        bool CanMoveToTriangle(int currentIndex, int targetIndex)
        {
            return GetPossibleMoves(currentPlayer.availableTurns, currentIndex).Any(list => list.Sum() * currentPlayer.Direction == targetIndex - currentIndex);
        }
        public void ManageTriangleTouch(int touchedIndex)
        {
            if (currentPlayer.thrownCubes)
            {
                if (currentPlayer.chosenTriangleIndex != NONE) //בחרנו כבר במשולש לשחק איתו
                {
                    if (CanMoveToTriangle(currentPlayer.chosenTriangleIndex, touchedIndex)) //אם נוכל לזוז במשולש הנגוע, נזוז אליו
                    {
                        Move(currentPlayer.chosenTriangleIndex, touchedIndex, GetPossibleMoves(currentPlayer.availableTurns, currentPlayer.chosenTriangleIndex), true);
                    }
                    // אם יש אכולים, לא ניתן לבטל בחירה וחייבים להוציא את האכולים קודם
                    else if (currentPlayer.eaten == 0)//אם לא, מדובר בביטול של בחירה, או בבחירה באחר
                    {
                        bool canceled = touchedIndex == currentPlayer.chosenTriangleIndex;
                        Cancel();
                        if (canceled || !PlayableTriangle(touchedIndex)) //ביטול על ידי לחיצה במשולש הנבחר
                        {
                            MarkPlayableCheckers();
                        }
                        else // נבחר במשולש אחר אם הוא שחיק
                        {
                            ChooseTriangle(touchedIndex);
                        }
                    }
                }
                else if (PlayableTriangle(touchedIndex)) // נבחר במשולש אם הוא שחיק
                {
                    ChooseTriangle(touchedIndex);
                }
            }
        }
        public void ManageSideBarTouch()
        {
            if (currentPlayer.chosenTriangleIndex != NONE)
            {
                if (CanMoveToTriangle(currentPlayer.chosenTriangleIndex, GetSideBarIndexByColor()))
                {
                    Move(currentPlayer.chosenTriangleIndex, GetSideBarIndexByColor(), GetPossibleMoves(currentPlayer.availableTurns, currentPlayer.chosenTriangleIndex), true);
                }
            }
        }
        public void ManageVoidTouch()
        {
            if (currentPlayer.chosenTriangleIndex != NONE && currentPlayer.eaten == 0)
            {
                Cancel();
                MarkPlayableCheckers();
            }
        }
        //***********************************************************
        bool IsSideBarIndex(int index)
        {
            return index == -1 || index == triangles.Count;
        }
        int GetSideBarIndexByColor()
        {
            if (currentPlayer.eaten > 0)
            {
                return currentPlayer == blackPlayer ? -1 : triangles.Count;
            }
            return currentPlayer == whitePlayer ? -1 : triangles.Count;
        }
        //************************************************************
        public bool TryMoveByArgs(Move move)
        {
            switch (move.moveType)
            {
                case DataModels.Move.MoveType.CubesThrow:
                    cubes.SetNumbers(move.move);
                    currentPlayer.thrownCubes = true;
                    currentPlayer.availableTurns = cubes.GetMovesAvailable();
                    OnMove.Invoke(this, new MoveEventArgs(DataModels.Move.MoveType.CubesThrow, cubes.CurrentNumbers, currentPlayer.color, false));
                    return true;
                case DataModels.Move.MoveType.CheckersMove:
                    if (CanMoveToTriangle(move.move.Item1, move.move.Item2))
                    {
                        Move(move.move.Item1, move.move.Item2, GetPossibleMoves(currentPlayer.availableTurns, move.move.Item1), false);
                        return true;
                    }
                    break;
            }
            return false;
        }
    }
}