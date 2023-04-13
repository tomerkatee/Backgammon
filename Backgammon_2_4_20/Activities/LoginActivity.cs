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
using Firebase.Auth;
using Firebase.Firestore;
using Firebase;
using System.Threading.Tasks;
using Backgammon_2_4_20.FirebaseHelpers;

namespace Backgammon_2_4_20.Activities
{
    [Activity(Label = "LoginActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class LoginActivity : Activity
    {
        Button buttonSubmit;
        EditText editTextPassword;
        EditText editTextUsername;
        LinearLayout linearLayout;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.layoutLogin);            
            InitializeViews();            
        }
        public void InitializeViews()
        {
            editTextUsername = (EditText)FindViewById(Resource.Id.editTextUsernameLogin);
            editTextPassword = (EditText)FindViewById(Resource.Id.editTextPasswordLogin);
            buttonSubmit = (Button)FindViewById(Resource.Id.buttonSubmitLogin);
            buttonSubmit.Click += ButtonSubmit_Click;
            linearLayout = (LinearLayout)FindViewById(Resource.Id.linearLayoutLogin);
            linearLayout.Background = MainActivity.BackgroundBitmapDrawable;
        }

        async void ButtonSubmit_Click(object sender, EventArgs e)
        {
            if (editTextPassword.Text == "" || editTextUsername.Text == "")
            {
                Toast.MakeText(this, "please fill missing information", ToastLength.Long);
                return;
            }
            bool success = await FirebaseHandler.Login(editTextUsername.Text, editTextPassword.Text);
            Toast.MakeText(this, string.Format("login {0}", success ? "succeeded" : "failed"), ToastLength.Short).Show();
            if (success)
            {
                StartActivity(typeof(MainActivity));
            }
        }
    }
}