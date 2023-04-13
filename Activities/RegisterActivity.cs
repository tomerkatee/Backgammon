using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Backgammon_2_4_20.FirebaseHelpers;

namespace Backgammon_2_4_20.Activities
{
    [Activity(Label = "RegisterActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class RegisterActivity : Activity
    {
        Button buttonSubmit;
        EditText editTextPassword;
        EditText editTextEmail;
        EditText editTextUsername;
        EditText editTextConfirmPassword;
        LinearLayout linearLayout;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Initialize();
        }
        private void Initialize()
        {
            SetContentView(Resource.Layout.layoutRegister);
            editTextPassword = (EditText)FindViewById(Resource.Id.editTextPasswordRegister);
            editTextConfirmPassword = (EditText)FindViewById(Resource.Id.editTextConfirmPasswordRegister);
            editTextEmail = (EditText)FindViewById(Resource.Id.editTextEmailRegister);
            editTextUsername = (EditText)FindViewById(Resource.Id.editTextUsernameRegister);
            buttonSubmit = (Button)FindViewById(Resource.Id.buttonSubmitRegister);
            buttonSubmit.Click += ButtonSubmit_Click;
            linearLayout = (LinearLayout)FindViewById(Resource.Id.linearLayoutRegister);
            linearLayout.Background = MainActivity.BackgroundBitmapDrawable;
        }

        private async void ButtonSubmit_Click(object sender, EventArgs e)
        {
            if (editTextPassword.Text == "" || editTextUsername.Text == "" || editTextEmail.Text == "")
            {
                Toast.MakeText(this, "please fill missing information", ToastLength.Long);
                return;
            }
            if (editTextUsername.Text == "guest")
            {
                Toast.MakeText(this, "invalid username", ToastLength.Long);
                return;
            }
            if (editTextConfirmPassword.Text != editTextPassword.Text)
            {
                Toast.MakeText(this, "passwords dont match", ToastLength.Long);
                return;
            }
            bool success = await FirebaseHandler.CreateUser(editTextUsername.Text, editTextEmail.Text, editTextPassword.Text);
            Toast.MakeText(this, string.Format("register {0}", success ? "succeeded" : "failed"), ToastLength.Short).Show();
            if (success)
            {
                await FirebaseHandler.Login(editTextEmail.Text, editTextPassword.Text);
                StartActivity(typeof(MainActivity));
            }
        }
    }
}