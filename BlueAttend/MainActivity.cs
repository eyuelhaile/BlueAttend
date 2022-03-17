using AltBeaconOrg.BoundBeacon;
using Android;
using Android.App;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using BlueAttend.Model;
using System;

namespace BlueAttend
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        TextView textMessage;
        Sync sync;
        Button stopServiceButton;
        Button startServiceButton;
        Intent startServiceIntent;
        Intent stopServiceIntent;
        bool isStarted = false;
        const int REQUEST_LOCATION = 0;
        LocationManager locationManager;
        bool GpsStatus;
        readonly string[] PermissionsLocation =
        {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };
        public string TAG
        {
            get;
            private set;
        }
        public void startBackgroundService()
        {
            Intent intent = new Intent(this, typeof(autoMonitor));
            this.StartService(intent);
        }
        public void stopBackgroundService()
        {
            Intent intent = new Intent(this, typeof(autoMonitor));
            this.StopService(intent);
        }
        // [Export]
        public bool CheckPermissionGranted(string permission)
        {
            if (ActivityCompat.CheckSelfPermission(this, permission) != Permission.Granted)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void RequestLocationPermission()
        {
            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.AccessFineLocation))
            {
                ActivityCompat.RequestPermissions(this, PermissionsLocation, REQUEST_LOCATION);
            }
            else
            {
                ActivityCompat.RequestPermissions(this, PermissionsLocation, REQUEST_LOCATION);
            }
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Lollipop)
            {
                Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                Android.App.AlertDialog alert = dialog.Create();
                alert.SetTitle("Save");
                alert.SetMessage("Minimum android version required to run this application is Android 5, please upgrade the Android version");
                alert.SetButton("OK", (c, ev) =>
                {
                    var activity = (Activity)this;
                    activity.FinishAffinity();
                });
                alert.Show();
            }
            if (!(CheckPermissionGranted(Manifest.Permission.AccessCoarseLocation) && CheckPermissionGranted(Manifest.Permission.AccessFineLocation)))
            {
                RequestLocationPermission();
            }
            CheckGpsStatus();
            BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            if (!bluetoothAdapter.IsEnabled)
            {
                Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
                builder.SetTitle("Message");
                builder.SetMessage("Bluetooth Required for the application to run click Yes to turn Bluetooth On");
                builder.SetCancelable(false);
                builder.SetPositiveButton("Yes", delegate
                {
                    bluetoothAdapter.Enable();
                    //Starting Foreground Service Code
                    OnNewIntent(this.Intent);
                    if (savedInstanceState != null)
                    {
                        isStarted = savedInstanceState.GetBoolean(Constants.SERVICE_STARTED_KEY, false);
                    }

                    startServiceIntent = new Intent(this, typeof(autoMonitor));
                    startServiceIntent.SetAction(Constants.ACTION_START_SERVICE);

                    stopServiceIntent = new Intent(this, typeof(autoMonitor));
                    stopServiceIntent.SetAction(Constants.ACTION_STOP_SERVICE);
                    StartServiceButton();


                    //End of Forground Service start code
                });
                builder.SetNegativeButton("No", delegate
                {
                    this.FinishAffinity();
                });
                builder.Show();

            }
            else
            {
                //Starting Foreground Service Code
                OnNewIntent(this.Intent);
                if (savedInstanceState != null)
                {
                    isStarted = savedInstanceState.GetBoolean(Constants.SERVICE_STARTED_KEY, false);
                }

                startServiceIntent = new Intent(this, typeof(autoMonitor));
                startServiceIntent.SetAction(Constants.ACTION_START_SERVICE);

                stopServiceIntent = new Intent(this, typeof(autoMonitor));
                stopServiceIntent.SetAction(Constants.ACTION_STOP_SERVICE);
                StartServiceButton();


                //End of Forground Service start code
            }

            SetContentView(Resource.Layout.activity_main);
            //textMessage = FindViewById<TextView>(Resource.Id.message);
           
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnNavigationItemSelectedListener(this);


            int loginid = DBManager.Instance.GetGlobalvariable().login_id;
            var user_type = DBManager.Instance.GetLoggedIdUser(loginid).user_type;
            
            if(user_type == "Staff")
            {
                var ft2 = FragmentManager.BeginTransaction();
                ft2.AddToBackStack(null);
                ft2.Replace(Resource.Id.container, new attendanceFragment());
                ft2.Commit();
            }
            else
            {
                var ft2 = FragmentManager.BeginTransaction();
                ft2.AddToBackStack(null);
                ft2.Replace(Resource.Id.container, new studReport());
                ft2.Commit();
            }

            // textMessage.Text = "welcome";
        }
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            Android.Support.V4.App.Fragment fragment = null;
            int loginid = DBManager.Instance.GetGlobalvariable().login_id;
            var user_type = DBManager.Instance.GetLoggedIdUser(loginid).user_type;
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:
                    if (user_type == "Staff")
                    {
                        var ft2 = FragmentManager.BeginTransaction();
                        ft2.AddToBackStack(null);
                        ft2.Replace(Resource.Id.container, new attendanceFragment());
                        ft2.Commit();
                    }
                    else
                    {
                        var ft2 = FragmentManager.BeginTransaction();
                        ft2.AddToBackStack(null);
                        ft2.Replace(Resource.Id.container, new studReport());
                        ft2.Commit();
                    }
                    break;
                case Resource.Id.navigation_dashboard:
                    if (user_type == "Staff")
                    {
                        var ft = FragmentManager.BeginTransaction();
                        ft.AddToBackStack(null);
                        ft.Replace(Resource.Id.container, new ReportFragment());
                        ft.Commit();
                    }
                    break;
                case Resource.Id.navigation_notifications:
                    break;
            }
            return false;
        }
        protected override void OnResume()
        {
            //SupportActionBar.SetTitle (Resource.String.app_nameAmh);
            base.OnResume();
           // RegisterReceiver(sync, new IntentFilter("android.net.conn.CONNECTIVITY_CHANGE"));
        }
        protected override void OnDestroy()
        {
            //Log.Info(TAG, "Activity is being destroyed; stop the service.");

            //StopService(startServiceIntent);
            base.OnDestroy();
           // UnregisterReceiver(sync);
        }
        protected override void OnNewIntent(Intent intent)
        {
            if (intent == null)
            {
                return;
            }

            var bundle = intent.Extras;
            if (bundle != null)
            {
                if (bundle.ContainsKey(Constants.SERVICE_STARTED_KEY))
                {
                    isStarted = true;
                }
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutBoolean(Constants.SERVICE_STARTED_KEY, isStarted);
            base.OnSaveInstanceState(outState);
        }
        void StopServiceButton()
        {

            Log.Info(TAG, "User requested that the service be stopped.");
            StopService(stopServiceIntent);
            isStarted = false;

        }

        void StartServiceButton()
        {

            StartService(startServiceIntent);
            Log.Info(TAG, "User requested that the service be started.");

            isStarted = true;

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            //   PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public void CheckGpsStatus()
        {

            locationManager = (LocationManager)this.GetSystemService(Context.LocationService);

            GpsStatus = locationManager.IsProviderEnabled(LocationManager.GpsProvider);

            if (GpsStatus == true)
            {

            }
            else
            {
                Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
                builder.SetTitle("Message");
                builder.SetMessage("To Continue, turn on device location, which the application need to calculate Distance");
                builder.SetCancelable(false);
                builder.SetPositiveButton("OK", delegate
                {
                    Intent intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
                    intent.AddFlags(ActivityFlags.NewTask);
                    intent.AddFlags(ActivityFlags.MultipleTask);
                    Android.App.Application.Context.StartActivity(intent);


                    //End of Forground Service start code
                });
                builder.SetNegativeButton("No Thanks", delegate {
                    // this.FinishAffinity();
                });
                builder.Show();



            }

        }

    }
    public class NotifyBeacon : AdvertiseCallback
    {
        private BeaconTransmitter mBeaconTransmittor;
        public NotifyBeacon(BeaconTransmitter mBeaconTransmittor)
        {
            this.mBeaconTransmittor = mBeaconTransmittor;
            Log.Info("MonitoringActivity", "Notify Beacons");
        }
        public override void OnStartFailure([GeneratedEnum] AdvertiseFailure errorCode)
        {
            base.OnStartFailure(errorCode);
            Log.Info("MonitoringActivity", "Not Started Beacons");
        }
        public override void OnStartSuccess(AdvertiseSettings settingsInEffect)
        {
            base.OnStartSuccess(settingsInEffect);
            Log.Info("MonitoringActivity", "Started Successfully Beacons");
        }

    }
}

