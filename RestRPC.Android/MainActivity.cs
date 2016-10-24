using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using RestRPC.Framework;
using Android.Preferences;

namespace RestRPC.Android
{
    [Activity(Label = "RestRPC.Android", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        RestRPCComponent component;
        EditText txtServer, txtName, txtUsername, txtPassword;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            txtServer = FindViewById<EditText>(Resource.Id.txtServer);
            txtName = FindViewById<EditText>(Resource.Id.txtName);
            txtUsername = FindViewById<EditText>(Resource.Id.txtUsername);
            txtPassword = FindViewById<EditText>(Resource.Id.txtPassword);

            // Load saved preferences
            txtServer.Text = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext).GetString("Server", "");
            txtName.Text = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext).GetString("Name", "");
            txtUsername.Text = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext).GetString("Username", "");
            txtPassword.Text = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext).GetString("Password", "");

            var btnConnect = FindViewById<Button>(Resource.Id.btnConnect);
            btnConnect.Click += btnConnect_OnClick;

            var btnSave = FindViewById<Button>(Resource.Id.btnSave);
            btnSave.Click += btnSave_OnClick;
        }

        private void btnConnect_OnClick(object sender, EventArgs e)
        {
            Uri serverUri = new Uri(FindViewById<EditText>(Resource.Id.txtServer).Text);
            string name = txtName.Text;
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            component = new RestRPCComponent(name, serverUri, TimeSpan.FromMilliseconds(100), username, password);
            component.PluginManager.RegisterPlugin("wifi", new RestRPC.Android.Plugins.WifiUtils());
            component.Start();

            Thread updateThread = new Thread(Update_ThreadProc);
            updateThread.Start();

            FindViewById<Button>(Resource.Id.btnConnect).Enabled = false;
        }

        private void btnSave_OnClick(object sender, EventArgs e)
        {
            var editor = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext).Edit();
            editor.PutString("Server", txtServer.Text);
            editor.PutString("Name", txtName.Text);
            editor.PutString("Username", txtUsername.Text);
            editor.PutString("Password", txtPassword.Text);
            Toast.MakeText(ApplicationContext, "saved", ToastLength.Long).Show();
            editor.Commit();
        }

        private void Update_ThreadProc()
        {
            while (true)
            {
                component.Update();
                // TODO: Use a timer instead of thread sleep
                Thread.Sleep(20);
            }
        }
    }
}

