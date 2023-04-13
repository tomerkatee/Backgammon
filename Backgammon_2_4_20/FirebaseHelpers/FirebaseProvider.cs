using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Firebase.Firestore;
using Java.Util;
using Firebase.Auth;
using System.Threading.Tasks;
using Android.Widget;
using Android.Gms.Extensions;
using Firebase;

namespace Backgammon_2_4_20.FirebaseHelpers
{
    public static class FirebaseProvider
    {
        public static FirebaseFirestore Firestore { get; private set; }
        public static FirebaseAuth FirebaseAuth { get; private set; }
        public static void InitFirebase(Context context)
        {
            if (Firestore != null && FirebaseAuth != null)
            {
                return;
            }
            var options = new FirebaseOptions.Builder()
                .SetApiKey("AIzaSyCccHLIf7giJNVeEfFzp_KtjJxIMZU0tXA")
                .SetDatabaseUrl("https://backgammon-a643e.firebaseio.com")
                .SetProjectId("backgammon-a643e")
                .SetStorageBucket("backgammon-a643e.appspot.com")
                .SetApplicationId("backgammon-a643e")
                .Build();
            var app = FirebaseApp.InitializeApp(context, options);
            Firestore = Firestore ?? FirebaseFirestore.GetInstance(app);
            FirebaseAuth = FirebaseAuth ?? FirebaseAuth.GetInstance(app);
        }
    }
}