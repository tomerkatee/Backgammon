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
using Java.Util;
using Firebase.Auth;
using System.Threading.Tasks;
using Android.Gms.Extensions;
using Firebase.Firestore;
using Backgammon_2_4_20.DataModels;

namespace Backgammon_2_4_20.FirebaseHelpers
{
    public static class FirebaseHandler
    {
        static FirebaseFirestore Firestore { get { return FirebaseProvider.Firestore; } }
        static FirebaseAuth FirebaseAuth { get { return FirebaseProvider.FirebaseAuth; } }
        static CollectionReference Users { get { return Firestore.Collection("users"); } }
        public static string GetCurrentUsername()
        {
            return FirebaseAuth.CurrentUser?.DisplayName;
        }
        public static async Task<bool> CreateUser(string username, string email, string password)
        {
            try
            {
                await FirebaseAuth.CreateUserWithEmailAndPasswordAsync(email, password);
                await FirebaseAuth.CurrentUser.UpdateProfileAsync(new UserProfileChangeRequest.Builder().SetDisplayName(username).Build());
                HashMap map = new HashMap();
                map.Put("username", username);
                map.Put("email", email);
                map.Put("password", password);
                DocumentReference docRef = Users.Document();
                await docRef.Set(map);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static async Task<string> GetEmail(string username)
        {
            QuerySnapshot snapshot = (QuerySnapshot)await Users.WhereEqualTo("username", username).Get();
            if (!snapshot.IsEmpty)
            {
                DocumentSnapshot matchingDoc = snapshot.Documents[0];
                return matchingDoc.Get("email").ToString();
            }
            return "";
        }
        public static void Logout()
        {
            FirebaseAuth.SignOut();
        }
        public static async Task<bool> Login(string username, string password)
        {
            try
            {
                string email = await GetEmail(username);
                if (email != "")
                {
                    await FirebaseAuth.SignInWithEmailAndPassword(email, password);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        public static async Task<List<string>> GetCurrentUserFriends()
        {
            QuerySnapshot userSnapshot = (QuerySnapshot)await Users.WhereEqualTo("username", GetCurrentUsername()).Get();
            DocumentReference docRef = Users.Document(userSnapshot.Documents[0].Id);
            QuerySnapshot friendsSnapshot = (QuerySnapshot)await docRef.Collection("friends").Get();
            return friendsSnapshot.Documents.Select(f => f.GetString("username")).ToList();
        }
        public static async Task<string> AddFriend(string username)
        {
            if (!await UserExists(username))
            {
                return "user doesnt exist";
            }
            if ((await GetCurrentUserFriends()).Contains(username))
            {
                return string.Format("you are already a friend of {0}", username);
            }
            QuerySnapshot userSnapshot = (QuerySnapshot)await Users.WhereEqualTo("username", GetCurrentUsername()).Get();
            DocumentReference docRef = Users.Document(userSnapshot.Documents[0].Id);
            DocumentReference friendDocument = docRef.Collection("friends").Document();
            HashMap map = new HashMap();
            map.Put("username", username);
            await friendDocument.Set(map);
            return "friend added successfully";
        }
        public static async Task<bool> UserExists(string username)
        {
            QuerySnapshot qs = (QuerySnapshot)await Users.WhereEqualTo("username", username).Get();
            return !qs.IsEmpty;
        }
        public static async Task SaveMatchInHistory(string username, Match match)
        {
            QuerySnapshot qsUser = (QuerySnapshot)await Users.WhereEqualTo("username", username).Get();
            if (!qsUser.IsEmpty)
            {
                DocumentReference matchRef = Users.Document(qsUser.Documents[0].Id).Collection("matchesHistory").Document();
                match.id = matchRef.Id;
                HashMap matchMap = new HashMap();
                matchMap.Put("date", match.date.ToShortDateString());
                matchMap.Put("myColor", match.myColor.ToString());
                matchMap.Put("opponentUsername", match.opponentUsername);
                matchMap.Put("victory", match.victory);
                matchMap.Put("id", match.id);
                await matchRef.Set(matchMap);
                CollectionReference movesColRef = matchRef.Collection("boardStates");
                for (int i = 0; i < match.boardStates.Count; i++)
                {
                    DocumentReference moveDocRef = movesColRef.Document();
                    HashMap moveMap = new HashMap();
                    moveMap.Put("boardState", match.boardStates[i].Serialize());
                    moveMap.Put("number", i);
                    await moveDocRef.Set(moveMap);
                }
            }

        }
        public async static Task<List<BoardState>> FetchBoardStates(string username, string matchId)
        {
            QuerySnapshot userSnapshot = (QuerySnapshot)await Users.WhereEqualTo("username", username).Get();
            if (!userSnapshot.IsEmpty)
            {
                DocumentSnapshot matchingDoc = userSnapshot.Documents[0];
                CollectionReference matches = Users.Document(matchingDoc.Id).Collection("matchesHistory");
                QuerySnapshot movesSnapshot = (QuerySnapshot)await matches.Document(matchId).Collection("boardStates").OrderBy("number").Get();
                List<BoardState> boardStates = new List<BoardState>();
                IList<DocumentSnapshot> docs = movesSnapshot.Documents;
                foreach (DocumentSnapshot doc in docs)
                {
                    boardStates.Add(BoardState.Deserialize(doc.GetString("boardState")));
                }
                return boardStates;
            }
            return null;
        }
        public static async Task<CollectionReference> GetUserMatches(string username)
        {
            QuerySnapshot querySnapshot = (QuerySnapshot)await Users.WhereEqualTo("username", username).Get();
            if (!querySnapshot.IsEmpty)
            {
                DocumentReference docRef = Users.Document(querySnapshot.Documents[0].Id);
                return docRef.Collection("matchesHistory");
            }
            return null;
        }
    }
}