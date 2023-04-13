using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Backgammon_2_4_20.FirebaseHelpers;
using Backgammon_2_4_20.BoardViews;
using Backgammon_2_4_20.DataModels;
using Backgammon_2_4_20.Networking;
using System.Threading.Tasks;

namespace Backgammon_2_4_20.Activities
{
    [Activity(Label = "GameActivityMultiplayer", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape, Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
    public class GameActivityMultiplayer : GameActivity
    {
        public enum ConnectionRole
        {
            Host, Client
        }
        ConnectionRole connectionRole;
        GameNetworkEntity gameNetworkEntity;
        BoardViewMultiplayer boardViewMultiplayer;
        string opponentUsername;
        public GameColor MyColor { get { return boardViewMultiplayer.myPlayerColor; } set { boardViewMultiplayer.myPlayerColor = value; } }
        List<BoardState> boardStates = new List<BoardState>();
        public async void SendMove(Move move)
        {
            boardStates.Add(boardView.BoardState);
            try
            {
                await gameNetworkEntity.SendMessageAsync(move.Serialize());
            }
            catch (Exception e)
            {
                gameNetworkEntity.Close();
                Win(MyColor);
            }
        }
        protected override void InitializeBoardView()
        {
            Point point = new Point();
            WindowManager.DefaultDisplay.GetSize(point);
            boardView = new BoardViewMultiplayer(this, point.X, point.Y);
            boardViewMultiplayer = (BoardViewMultiplayer)boardView;
        }
        async Task<bool> InitializeAsHost()
        {
            gameNetworkEntity = new Host();
            Host host = ((Host)gameNetworkEntity);
            HostPublisher hostPublisher = new HostPublisher(FirebaseHandler.GetCurrentUsername(), host.LocalPort);
            hostPublisher.PublishHost();
            bool clientConnected = await host.WaitForClient();
            hostPublisher.Close();
            return clientConnected;
        }
        public async void ReceiveSerialization()
        {
            try
            {
                string message = await gameNetworkEntity.ReceiveMessageAsync();
                Move move = Move.Deserialize(message);
                bool isValid = boardViewMultiplayer.TryMoveByArgs(move);
                if (isValid)
                {
                    boardStates.Add(boardView.BoardState);
                }
                else
                {
                    throw new Exception("received message is invalid");
                }
            }
            catch
            {
                gameNetworkEntity.Close();
                Win(MyColor);
            }
        }
        public override void Win(GameColor winnerColor)
        {
            Intent intent = new Intent(this, typeof(GameOverActivityMultiplayer));
            intent.PutExtra("winnerColor", winnerColor.ToString());
            intent.PutExtra("boardStates", boardStates.ConvertAll(bs => bs.Serialize()).ToArray());
            intent.PutExtra("opponentUsername", opponentUsername);
            intent.PutExtra("opponentLeft", !gameNetworkEntity.Connected);
            intent.PutExtra("myColor", MyColor.ToString());
            StartActivityForResult(intent, 0);
        }
        protected override void OnDestroy()
        {
            gameNetworkEntity.Close();
            base.OnDestroy();
        }
        protected async override void OnResume()
        {
            base.OnResume();
            connectionRole = Intent.GetStringExtra("role") == ConnectionRole.Host.ToString() ? ConnectionRole.Host : ConnectionRole.Client;
            if (connectionRole == ConnectionRole.Host)
            {
                if (gameNetworkEntity == null || !gameNetworkEntity.Connected)
                {
                    if (!await InitializeAsHost())
                    {
                        Finish();
                        return;
                    }
                }
                MyColor = GameColorHandler.PickRandomColor();
                await gameNetworkEntity.SendMessageAsync(boardView.BoardState.Serialize() + "&" + GameColorHandler.OppositeColor(MyColor));
                opponentUsername = await gameNetworkEntity.ReceiveMessageAsync();
            }
            else
            {
                if (gameNetworkEntity == null || !gameNetworkEntity.Connected)
                {
                    IPAddress ip = IPAddress.Parse(Intent.GetStringExtra("ip"));
                    int port = Intent.GetIntExtra("port", -1);
                    opponentUsername = Intent.GetStringExtra("opponentUsername");
                    gameNetworkEntity = new Client();
                    if (!await ((Client)gameNetworkEntity).ConnectToHostSocketAsync(ip, port))
                    {
                        StartActivity(typeof(MainActivity));
                        Finish();
                        return;
                    }
                }
                string serialization = await gameNetworkEntity.ReceiveMessageAsync();
                await gameNetworkEntity.SendMessageAsync(FirebaseHandler.GetCurrentUsername() ?? "guest");
                string[] splitted = serialization.Split("&");
                boardView.BoardState = BoardState.Deserialize(splitted[0]);
                MyColor = GameColorHandler.GetColorFromString(splitted[1]);
            }
            boardStates = new List<BoardState>();
            boardStates.Add(boardView.BoardState);
            ToastCurrentPlayer();
            if (boardView.CurrentPlayer != MyColor)
            {
                ReceiveSerialization();
            }
            boardViewMultiplayer.isValid = true;
        }
        public override void ToastCurrentPlayer()
        {
            RunOnUiThread(() =>
            {
                Toast.MakeText(this, string.Format("its {0} turn", boardView.CurrentPlayer == MyColor ? "your" : opponentUsername + "'s"), ToastLength.Short).Show();
            });
        }
    }
}