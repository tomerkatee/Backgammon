using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content;
using Firebase.Firestore;
using Firebase;
using Java.Util;
using Android.Gms.Tasks;
using System.Net;
using Java.Lang;
using System.Net.Sockets;
using System;
using System.Threading;
using Firebase.Auth;
using Backgammon_2_4_20.FirebaseHelpers;
using Backgammon_2_4_20.Activities;
using Android.Views;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Graphics.Drawables;
using Backgammon_2_4_20.DownloadBackground;
using Android;
using Plugin.Media;
using System.IO;
using Android.Content.PM;

namespace Backgammon_2_4_20
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        static ISharedPreferences sp;
        DownloadReceiver downloadReceiver;
        Button buttonStartLan;
        Button buttonStart;
        Dialog dialogChangeBackground;
        Dialog dialogAddFriend;
        Button buttonHistory;
        LinearLayout linearLayout;
        TextView textViewUsername;
        public static string CurrentUsername { get { return FirebaseHandler.GetCurrentUsername(); } }
        public static bool LoggedIn { get { return CurrentUsername != null && CurrentUsername != ""; } }
        public static string BackgroundPath
        {
            get
            {
                return sp.GetString("backgroundPath", "");
            }
            set
            {
                ISharedPreferencesEditor editor = sp.Edit();
                editor.PutString("backgroundPath", value);
                editor.Apply();
            }
        }
        public static BitmapDrawable BackgroundBitmapDrawable
        {
            get
            {
                Bitmap bitmap = BitmapFactory.DecodeFile(BackgroundPath);
                return new BitmapDrawable(bitmap);
            }
        }
        readonly string[] permissionGroup =
        {
            Manifest.Permission.Camera,
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage
        };
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Initialize();            
        }
        void Initialize()
        {
            FirebaseProvider.InitFirebase(this);
            InitializeViewsAndEvents();
            downloadReceiver = new DownloadReceiver();
            downloadReceiver.OnImageReceived += (sender, e) => { BackgroundPath = e.path; UpdateLayoutBackground(); };
            sp = GetSharedPreferences("details", FileCreationMode.Private);
            UpdateLayoutBackground();
            RequestPermissions(permissionGroup, 0);
        }
        private void InitializeViewsAndEvents()
        {
            dialogChangeBackground = new Dialog(this);
            InitializeDialog(dialogChangeBackground, true, "Change Background", Resource.Layout.layoutChangeBackgroundMethod);
            dialogAddFriend = new Dialog(this);
            InitializeDialog(dialogAddFriend, true, "Add a friend", Resource.Layout.layoutAddFriend);
            linearLayout = (LinearLayout)FindViewById(Resource.Id.linearLayoutMain);
            buttonStart = (Button)FindViewById(Resource.Id.buttonStartLocal);
            buttonStartLan = (Button)FindViewById(Resource.Id.buttonStartLAN);
            buttonHistory = (Button)FindViewById(Resource.Id.buttonHistory);            
            textViewUsername = (TextView)FindViewById(Resource.Id.textViewUsernameMain);
            buttonStart.Click += (sender, args) => { StartActivity(typeof(GameActivity)); };
            buttonStartLan.Click += (sender, args) => { StartActivity(typeof(FindHostActivity)); };
            buttonHistory.Click += (sender, e) => { StartActivity(typeof(HistoryActivity)); };
            SetViewsVisibilities();

        }
        void InitializeDialog(Dialog dialog, bool cancelable, string title, int layoutResID)
        {
            dialog.SetCancelable(cancelable);
            dialog.SetTitle(title);
            dialog.SetContentView(layoutResID);
        }
        void OnChangeBackgroundClick()
        {
            dialogChangeBackground.SetContentView(Resource.Layout.layoutChangeBackgroundMethod);
            dialogChangeBackground.Show();
            dialogChangeBackground.FindViewById<Button>(Resource.Id.buttonChangeBackgroundMethodWeb).Click += (sender, e) =>
            {
                dialogChangeBackground.SetContentView(Resource.Layout.layoutDownloadFromWeb);
                dialogChangeBackground.FindViewById<Button>(Resource.Id.buttonChangeBackgroundApply).Click += (senderNested, eNested) =>
                {
                    string url = dialogChangeBackground.FindViewById<EditText>(Resource.Id.editTextBackgroundUrl).Text;
                    if (url != "")
                    {
                        Intent intent = new Intent(this, typeof(DownloadService));
                        intent.PutExtra("url", url);
                        StartService(intent);
                        dialogChangeBackground.Dismiss();
                    }
                };
            };
            dialogChangeBackground.FindViewById<Button>(Resource.Id.buttonChangeBackgroundMethodCamera).Click += (sender, e) => 
            {
                SetBackgroundByCameraPhoto();
                dialogChangeBackground.Dismiss();
            };
            dialogChangeBackground.FindViewById<Button>(Resource.Id.buttonChangeBackgroundMethodGallery).Click += (sender, e) => 
            { 
                SetBackgroundByGalleryPhoto();
                dialogChangeBackground.Dismiss();
            };
        }
        async void SetBackgroundByCameraPhoto()
        {
            await CrossMedia.Current.Initialize();
            var mediaFile = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions 
            { 
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Large,
                Name = "backgroundPic",
                Directory = "backgroundPics",   
                CompressionQuality = 20
            });
            if (mediaFile != null)
            {
                BackgroundPath = mediaFile.Path;
                UpdateLayoutBackground();
            }

        }
        async void SetBackgroundByGalleryPhoto()
        {
            var mediaFile = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Large,
                CompressionQuality = 20
            });
            if (mediaFile != null)
            {
                BackgroundPath = mediaFile.Path;
                UpdateLayoutBackground();
            }
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menuMain, menu);
            menu.FindItem(Resource.Id.action_change_background).SetShowAsAction(ShowAsAction.Always);
            menu?.FindItem(Resource.Id.action_login).SetVisible(!LoggedIn);
            menu?.FindItem(Resource.Id.action_register).SetVisible(!LoggedIn);
            menu?.FindItem(Resource.Id.action_logout).SetVisible(LoggedIn);
            menu?.FindItem(Resource.Id.action_add_friend).SetVisible(LoggedIn);
            return true;
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_change_background:
                    OnChangeBackgroundClick();
                    break;
                case Resource.Id.action_login:
                    StartActivity(typeof(LoginActivity));
                    break;
                case Resource.Id.action_logout:
                    FirebaseHandler.Logout();
                    InvalidateOptionsMenu();
                    SetViewsVisibilities();
                    textViewUsername.Text = string.Format("Hello {0}!", LoggedIn ? CurrentUsername : "guest");
                    break;
                case Resource.Id.action_register:
                    StartActivity(typeof(RegisterActivity));
                    break;
                case Resource.Id.action_add_friend:
                    OnAddFriendSelected();
                    break;
                default:
                    break;
            }
            return true;
        }
        protected override void OnResume()
        {
            base.OnResume();
            RegisterReceiver(downloadReceiver, new IntentFilter("download_finished"));
            textViewUsername.Text = string.Format("Hello {0}!", LoggedIn ? CurrentUsername : "guest");
            SetViewsVisibilities();
        }
        protected override void OnDestroy()
        {
            UnregisterReceiver(downloadReceiver);
            base.OnDestroy();
        }
        void SetViewsVisibilities()
        {
            buttonHistory.Visibility = LoggedIn ? ViewStates.Visible : ViewStates.Gone;
        }
        public void UpdateLayoutBackground()
        {
            linearLayout.Background = BackgroundBitmapDrawable;
        }
        void OnAddFriendSelected()
        {
            dialogAddFriend.Show();
            dialogAddFriend.FindViewById<Button>(Resource.Id.buttonAddFriend).Click += async (sender, e) =>
            {
                string username = dialogAddFriend.FindViewById<EditText>(Resource.Id.editTextAddFriend).Text;
                if (username != "")
                {
                    string result = await FirebaseHandler.AddFriend(username);
                    RunOnUiThread(() => { Toast.MakeText(this, result, ToastLength.Short).Show(); });
                }
                else
                {
                    RunOnUiThread(() => { Toast.MakeText(this, "please enter friend's name", ToastLength.Short).Show(); });
                }
            };
        }
    }
}