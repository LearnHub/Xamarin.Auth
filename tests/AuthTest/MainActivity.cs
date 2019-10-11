using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Xamarin.Auth;

namespace AuthTest
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private const string TAG = "AuthTest.MainActivity";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            Log.Info(TAG, $"OnCreate() called...");

            Button btnShib = FindViewById<Button>(Resource.Id.btnShib);
            btnShib.Click += BtnShib_Click;

            Button btnMicrosoft = FindViewById<Button>(Resource.Id.btnMicrosoft);
            btnMicrosoft.Click += BtnMicrosoft_Click;
        }

        public static readonly string URL_SHIB_AUTH = "https://shib.learnpad.com/Shibboleth.sso/Login";
        public static readonly string URL_SHIB_REDIRECT = "https://api.learnpad.com/login/success/";

        public static readonly string OAUTH_MS_URL = "https://login.microsoftonline.com/common/oauth2/V2.0/authorize";
        public static readonly string OAUTH_MS_REDIRECT_URL = "https://connect.learnpad.com/login/";
        public static readonly string OAUTH_MS_CLIENT_ID = "1a7bcec1-5c20-42e2-8f7b-bf086abfe911";
        public static readonly string OAUTH_MS_SCOPE = "openid email https://graph.microsoft.com/user.read";

        private async void BtnShib_Click(object sender, EventArgs e) {
            var authenticator = new WebRedirectAuthenticator(new Uri(URL_SHIB_AUTH), new Uri(URL_SHIB_REDIRECT));
            authenticator.BrowsingCompleted += authenticator_BrowsingCompleted;
            authenticator.Completed += authenticator_Completed;
            authenticator.Error += authenticator_Error;

            Account account = null;

            // If authorization succeeds or is canceled, Completed event will be fired

            var tcs = new TaskCompletionSource<Account>();
            EventHandler<AuthenticatorCompletedEventArgs> onCompleteHandler = (o, ea) => tcs.SetResult(ea.IsAuthenticated ? ea.Account : null);

            authenticator.Completed += onCompleteHandler;
            try {
                // Raise UI dialog
                var oUI = authenticator.GetUI(Application.Context);
                var intent = oUI as Intent;
                intent.SetFlags(ActivityFlags.NewTask);
                Application.Context.StartActivity(intent);
                // Wait for dialog completion event handler to fire
                account = await tcs.Task.ConfigureAwait(false);
            } catch (Exception ex) {
                Log.Error(TAG, $"Auth exception ->{ex.Message}");
            } finally {
                authenticator.Completed -= onCompleteHandler;
            }
        }

        private async void BtnMicrosoft_Click(object sender, EventArgs e) {
            var authenticator = new OAuth2Authenticator(
                OAUTH_MS_CLIENT_ID,
                OAUTH_MS_SCOPE,
                new Uri(OAUTH_MS_URL),
                new Uri(OAUTH_MS_REDIRECT_URL)) {
                AllowCancel = true,
            };
            authenticator.BrowsingCompleted += authenticator_BrowsingCompleted;
            authenticator.Completed += authenticator_Completed;
            authenticator.Error += authenticator_Error;

            Account account = null;

            // If authorization succeeds or is canceled, Completed event will be fired

            var tcs = new TaskCompletionSource<Account>();
            EventHandler<AuthenticatorCompletedEventArgs> onCompleteHandler = (o, ea) => tcs.SetResult(ea.IsAuthenticated ? ea.Account : null);

            authenticator.Completed += onCompleteHandler;
            try {
                // Raise UI dialog
                var oUI = authenticator.GetUI(Application.Context);
                var intent = oUI as Intent;
                intent.SetFlags(ActivityFlags.NewTask);
                Application.Context.StartActivity(intent);
                // Wait for dialog completion event handler to fire
                account = await tcs.Task.ConfigureAwait(false);
            } catch (Exception ex) {
                Log.Error(TAG, $"Auth exception ->{ex.Message}");
            } finally {
                authenticator.Completed -= onCompleteHandler;
            }
        }

        private void authenticator_Error(object sender, AuthenticatorErrorEventArgs e) {
            Log.Info(TAG, $"Authenticator Error : {e.Message}");
        }

        private void authenticator_Completed(object sender, AuthenticatorCompletedEventArgs e) {
            if (e.IsAuthenticated) {
                Log.Info(TAG, $"Authenticator Completed: AUTH SUCCESS - {e.Account.Username}");
            } else {
                Log.Info(TAG, $"Authenticator Completed: AUTH FAILED");
            }
        }

        private void authenticator_BrowsingCompleted(object sender, EventArgs e) {
            Log.Info(TAG, $"Authenticator Browsing Completed");
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
	}
}

