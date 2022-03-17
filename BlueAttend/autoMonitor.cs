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
using Android.Util;
using AltBeaconOrg.BoundBeacon;
using AltBeaconOrg.BoundBeacon.Powersave;
using Android.Support.V4.App;
using Android.Media;
using Android.Bluetooth;
using BlueAttend.Model;

namespace BlueAttend
{
    [Service]
    public class autoMonitor : Service
    {
        static readonly string Tag = typeof(autoMonitor).Name;

        UtcTimestamper timestamper;
        bool isStarted;
        Handler handler;
        Action runnable;
        BackgroundPowerSaver saver;
        private IBinder binder;

        private MyBeaconClass _myBeaconClass;
        protected string TAG = "MonitoringActivity";
        private BeaconManager beaconManager;
        private Region region;
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;
        Context context;
        public autoMonitor()
        {
            
        }

        public override void OnCreate()
        {
            base.OnCreate();
            Log.Info(TAG, "OnCreate: the service is initializing.");         
            timestamper = new UtcTimestamper();
            handler = new Handler();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Info(TAG, "Service started");

            BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            if (!bluetoothAdapter.IsEnabled)
            {
                bluetoothAdapter.Enable();
            }

            context = ApplicationContext;

            //Getting Logged user Profile
            var profile = DBManager.Instance.GetUser().Where(c => c.active == 1).FirstOrDefault();
            ISharedPreferences sharedData = context.GetSharedPreferences("account", FileCreationMode.Private);
            string guid = Guid.NewGuid().ToString();
            if(profile.user_type == "Student")
            {
                var data = guid.Split("-");
                string student_id = profile.username.Replace("SGSR/", "");
                student_id = student_id.Replace("/", "") + "aaaaaa";
                data[4] = student_id;
                guid = data[0] + "-" + data[1] + "-" + data[2] + "-" + data[3] + "-" + data[4];
            }
            


            string user_id1 = guid.ToString();
            string user_id2 = "1000";// profile.username + "username";
            string user_id3 = "2000"; //profile.student_id + "test";
           

             
            //Beacon basic  Coniguration 
            Log.Info(TAG, "database passed---" + user_id1);
            var iBeaconParser = new BeaconParser();
            iBeaconParser.SetBeaconLayout("m:2-3=0215,i:4-19,i:20-21,i:22-23,p:24-24");
            BeaconTransmitter beaconTransmitter = new BeaconTransmitter(this, iBeaconParser);
            try
            {
                var beacon1 = new AltBeacon.Builder().SetId1(user_id1)
               .SetId2(user_id2).SetId3(user_id3).SetDataFields(new List<Java.Lang.Long> { new Java.Lang.Long(0x25L) }).SetTxPower(-59).Build();
                NotifyBeacon callback = new NotifyBeacon(beaconTransmitter);
                beaconTransmitter.StartAdvertising(beacon1, callback);
            }
            catch (Exception ex)
            {
                Log.Info(ex.StackTrace, ex.Message);
            }
           
            

            Log.Info(TAG, "Beacon Started");

            if (intent.Action.Equals(Constants.ACTION_START_SERVICE))
            {
                if (isStarted)
                {
                    Log.Info(TAG, "OnStartCommand: The service is already running.");
                }
                else
                {
                    Log.Info(TAG, "OnStartCommand: The service is starting.");
                    if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                        RegisterForegroundServiceO();
                    else { RegisterForegroundService(); }
                    //handler.PostDelayed(runnable, Constants.DELAY_BETWEEN_LOG_MESSAGES);
                    isStarted = true;
                    _myBeaconClass = new MyBeaconClass(this);
                    _myBeaconClass.Start();
                }
            }
            else if (intent.Action.Equals(Constants.ACTION_STOP_SERVICE))
            {
                Log.Info(TAG, "OnStartCommand: The service is stopping.");
                timestamper = null;
                StopForeground(true);
                StopSelf();
                isStarted = false;

            }
            else if (intent.Action.Equals(Constants.ACTION_RESTART_TIMER))
            {
                Log.Info(TAG, "OnStartCommand: Restarting the timer.");
               // timestamper.Restart();

            }
            
            //Battery Saving setting
            saver = new BackgroundPowerSaver(this);

            return StartCommandResult.Sticky;
        }

        

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
           // UnregisterReceiver(autosync);
          //  StartService(new Intent(this, typeof(autoMonitor)));
            Log.Debug(Tag, "Sensor Service has been terminated");
        }
        string GetFormattedTimestamp()
        {

            return timestamper?.GetFormattedTimestamp();
        }

        void RegisterForegroundService()
        {
            var msg = Resources.GetString(Resource.String.notification_text);
            var notification = new Notification.Builder(this)
                .SetContentTitle(Resources.GetString(Resource.String.app_name))
                .SetContentText(msg)
                .SetSmallIcon(Resource.Drawable.ba)
                .SetContentIntent(BuildIntentToShowMainActivity())
                .SetOngoing(true)
                .Build();
            StartForeground(Constants.SERVICE_RUNNING_NOTIFICATION_ID, notification);
        }
        void RegisterForegroundServiceO()
        {
            var msg = Resources.GetString(Resource.String.notification_text);          

            String NOTIFICATION_CHANNEL_ID = "BlueAttend.BlueAttend";
            NotificationChannel chan = new NotificationChannel(NOTIFICATION_CHANNEL_ID, "BlueAttendNotify", NotificationImportance.High);

            NotificationManager manager = (NotificationManager)GetSystemService(Context.NotificationService);

            manager.CreateNotificationChannel(chan);

            NotificationCompat.Builder notificationBuilder = new NotificationCompat.Builder(this, NOTIFICATION_CHANNEL_ID);

            Notification notification = notificationBuilder.SetOngoing(true)
                .SetContentTitle(Resources.GetString(Resource.String.app_name))
                .SetContentText(msg)
                .SetSmallIcon(Resource.Drawable.ba)
                .SetContentIntent(BuildIntentToShowMainActivity())
                .SetOngoing(true)
                .Build();

            //const int Service_Running_Notification_ID = 936;
            StartForeground(Constants.SERVICE_RUNNING_NOTIFICATION_ID, notification);
        }

        /// <summary>
        /// Builds a PendingIntent that will display the main activity of the app. This is used when the 
        /// user taps on the notification; it will take them to the main activity of the app.
        /// </summary>
        /// <returns>The content intent.</returns>
        PendingIntent BuildIntentToShowMainActivity()
        {
            var notificationIntent = new Intent(this, typeof(MainActivity));
            notificationIntent.SetAction(Constants.ACTION_MAIN_ACTIVITY);
            notificationIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);
            notificationIntent.PutExtra(Constants.SERVICE_STARTED_KEY, true);

            var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }

        /// <summary>
        /// Builds a Notification.Action that will instruct the service to restart the timer.
        /// </summary>
        /// <returns>The restart timer action.</returns>
        Notification.Action BuildRestartTimerAction()
        {
            var restartTimerIntent = new Intent(this, GetType());
            restartTimerIntent.SetAction(Constants.ACTION_RESTART_TIMER);
            var restartTimerPendingIntent = PendingIntent.GetService(this, 0, restartTimerIntent, 0);

            var builder = new Notification.Action.Builder(Resource.Drawable.ic_action_refresh,
                                              GetText(Resource.String.restart_timer),
                                              restartTimerPendingIntent);

            return builder.Build();
        }

        /// <summary>
        /// Builds the Notification.Action that will allow the user to stop the service via the
        /// notification in the status bar
        /// </summary>
        /// <returns>The stop service action.</returns>
        Notification.Action BuildStopServiceAction()
        {
            var stopServiceIntent = new Intent(this, GetType());
            stopServiceIntent.SetAction(Constants.ACTION_STOP_SERVICE);
            var stopServicePendingIntent = PendingIntent.GetService(this, 0, stopServiceIntent, 0);

            var builder = new Notification.Action.Builder(Android.Resource.Drawable.IcMediaPause,
                                                          GetText(Resource.String.stop_service),
                                                          stopServicePendingIntent);
            return builder.Build();

        }
    }

    public class MyBeaconClass : Java.Lang.Object, IRangeNotifier, IBeaconConsumer
    {
        private Context _context;
        private Region _region;
        private BeaconManager _beaconManager;

        NotificationCompat.Builder builder;
        NotificationManager notificationManager;
        Android.Net.Uri alarmUri;
        AudioAttributes alarmAttributes;

        NotificationChannel chan;
        NotificationManager manager;
        String NOTIFICATION_CHANNEL_ID;

        public MyBeaconClass(Context context)
        {
            _context = context;
        }

        public Context ApplicationContext
        {
            get
            {
                return _context.ApplicationContext;
            }
        }

       

        public void Start()
        {
            alarmUri = Android.Net.Uri.Parse("android.resource://" + Application.Context.PackageName + "/raw/soundfile");
            alarmAttributes = new AudioAttributes.Builder()
                              .SetContentType(AudioContentType.Sonification)
                              .SetUsage(AudioUsageKind.Notification).Build();
            _beaconManager = BeaconManager.GetInstanceForApplication(this.ApplicationContext);
            var iBeaconParser = new BeaconParser();
            iBeaconParser.SetBeaconLayout("m:2-3=0215,i:4-19,i:20-21,i:22-23,p:24-24");
            _beaconManager.BeaconParsers.Add(iBeaconParser);
            _beaconManager.Bind(this);
            // _beaconManager = BeaconManager.GetInstanceForApplication(_context.ApplicationContext);
            // _beaconManager.Bind(this);

            NOTIFICATION_CHANNEL_ID = "BlueAttend.BlueAttend";
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                chan = new NotificationChannel(NOTIFICATION_CHANNEL_ID, "BlueAttendNotify", NotificationImportance.High);

                manager = (NotificationManager)_context.GetSystemService(Context.NotificationService);

                manager.CreateNotificationChannel(chan);
            }
            
        }

        public void OnBeaconServiceConnect()
        {
            _beaconManager.SetForegroundBetweenScanPeriod(5000);
            _beaconManager.SetRangeNotifier(this);
            _region = new AltBeaconOrg.BoundBeacon.Region("Region", null, null, null);
            _beaconManager.StartRangingBeaconsInRegion(_region);
            

        }

        public void DidRangeBeaconsInRegion(ICollection<Beacon> beacons, Region region)
        {
            int i = 0;
            try
            {
                foreach (var item in beacons)
                {
                    var ids = item.Id1.ToString().Split("-");
                    string pre = "SGSR";
                    string id = ids[4].Substring(0, 4);
                    string batch = ids[4].Substring(4, 2);
                    string student_id = pre + "/" + id + "/" + batch;
                    int courseId = DBManager.Instance.GetActiveClassId();
                    var check = DBManager.Instance.GetAttendanceById(student_id).Where(c=>c.course_id == courseId && c.attended == 0).ToList();
                    //Checking if the student is in range 
                    if (item.Distance < 2 && check.Count > 0)
                    {
                        //Notifying the teacher during attendance using beep sound
                        alarmUri = Android.Net.Uri.Parse("android.resource://" + Application.Context.PackageName + "/raw/beep2");
                        alarmAttributes = new AudioAttributes.Builder()
                                          .SetContentType(AudioContentType.Sonification)
                                          .SetUsage(AudioUsageKind.Notification).Build();

                        builder = new NotificationCompat.Builder(_context, NOTIFICATION_CHANNEL_ID)
                                        .SetContentTitle("Message")
                                        .SetContentText(check.FirstOrDefault().name + " Attended Successfully")
                                        .SetSmallIcon(Resource.Drawable.ba)
                                        .SetOngoing(false)
                                        .SetSound(alarmUri)
                                        .SetVibrate(new long[] { 1000, 1000 })
                                        .SetAutoCancel(true);
                        notificationManager = (NotificationManager)_context.GetSystemService(Context.NotificationService);
                        notificationManager.Notify(1, builder.Build());
                        //Saving the Attendance
                        attendance attend = check.FirstOrDefault();
                        attend.attended = 1;
                        DBManager.Instance.updateAttendance(attend);
                    }
                    else
                    {
                    }                          
                }
            }
            catch (Exception e)
            {
                Log.Info("MonitoringActivity", e.Message.ToString());
            }
            
        }

        public bool BindService(Intent intent, IServiceConnection serviceConnection, [GeneratedEnum] Bind flags)
        {
            return _context.BindService(intent, serviceConnection, flags);
        }

        public void UnbindService(IServiceConnection serviceConnection)
        {
            _context.UnbindService(serviceConnection);
        }

        public void Dispose()
        {

        }
    }
}