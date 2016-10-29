using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using RestRPC.Android.Plugins;
using RestRPC.Framework;
using System;
using System.Threading;

namespace RestRPC.Android
{
    [Activity(Label = "RestRPC Android Service", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        RrpcComponent component;
        EditText txtServer, txtName, txtUsername, txtPassword;
        TextView lblStatus, lblLog;
        Button btnConnect, btnSave, btnClearLog;
        TextViewWriter logWriter;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            txtServer = FindViewById<EditText>(Resource.Id.txtServer);
            txtName = FindViewById<EditText>(Resource.Id.txtName);
            txtUsername = FindViewById<EditText>(Resource.Id.txtUsername);
            txtPassword = FindViewById<EditText>(Resource.Id.txtPassword);
            lblStatus = FindViewById<TextView>(Resource.Id.lblConnStatus);
            lblLog = FindViewById<TextView>(Resource.Id.lblLog);
            btnConnect = FindViewById<Button>(Resource.Id.btnConnect);
            btnSave = FindViewById<Button>(Resource.Id.btnSave);
            btnClearLog = FindViewById<Button>(Resource.Id.btnClearLog);

            lblLog.MovementMethod = new ScrollingMovementMethod();
            logWriter = new TextViewWriter(lblLog, RunOnUiThread);

            // Load saved preferences
            txtServer.Text = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext).GetString("Server", "");
            txtName.Text = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext).GetString("Name", "");
            txtUsername.Text = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext).GetString("Username", "");
            txtPassword.Text = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext).GetString("Password", "");

            btnConnect.Click += btnConnect_OnClick;
            btnSave.Click += btnSave_OnClick;
            btnClearLog.Click += btnClearLog_OnClick;
        }

        private void btnConnect_OnClick(object sender, EventArgs e)
        {
            Uri serverUri = new Uri(FindViewById<EditText>(Resource.Id.txtServer).Text);
            string name = txtName.Text;
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            component = new RrpcComponent(name, serverUri, TimeSpan.FromMilliseconds(1000), username, password, logWriter, LogType.All);
            component.PluginManager.RegisterPlugin("wifi", new WifiUtils());
            component.Start();

            Thread updateThread = new Thread(Update_ThreadProc);
            updateThread.Start();

            // Disable connect button
            btnConnect.Enabled = false;
        }

        private void btnSave_OnClick(object sender, EventArgs e)
        {
            var editor = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext).Edit();
            editor.PutString("Server", txtServer.Text);
            editor.PutString("Name", txtName.Text);
            editor.PutString("Username", txtUsername.Text);
            editor.PutString("Password", txtPassword.Text);
            Toast.MakeText(ApplicationContext, "Settings saved", ToastLength.Long).Show();
            editor.Commit();
        }

        private void btnClearLog_OnClick(object sender, EventArgs e)
        {
            lblLog.Text = "";
        }

        private void Update_ThreadProc()
        {
            while (true)
            {
                // Run all the updates on the UI thread.
                // In a game, RRPC service must be updated on the game's main update thread
                RunOnUiThread(() => 
                {
                    component.Update();
                    UpdateStatusUI();
                });
                // TODO: Use a timer instead of thread sleep
                Thread.Sleep(100);
            }
        }

        private void UpdateStatusUI()
        {
            lblStatus.Text = component.ConnectionState.ToString();
        }
    }
}

