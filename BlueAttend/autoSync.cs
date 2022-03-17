using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Android.Support.V4.App;
using BlueAttend.Model;
using BlueAttend.dbsync;
using BlueAttend;

namespace BlueAttend
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    //[IntentFilter(new[] { Intent.ActionBootCompleted })]
    [IntentFilter(new string[] { "android.net.conn.CONNECTIVITY_CHANGE" })]
    [Android.Runtime.Preserve(AllMembers = true)]
    public class autoSync : BroadcastReceiver
    {
        const int NOTIFICATION_ID = 9000;
        int count = 0;
        Context context;
        Android.Support.V4.App.NotificationCompat.Builder builder;
        NotificationManager notificationManager;
        Task task1, task2, task3, task4, task5;
        NotificationChannel chan;
        NotificationManager manager;
        String NOTIFICATION_CHANNEL_ID;
        
        public override void OnReceive(Context context, Intent intent)
        {
            this.context = context;
            // Toast.MakeText(context, "Received intent!", ToastLength.Short).Show();
            if (intent.Action != null && intent.Action == "android.net.conn.CONNECTIVITY_CHANGE")
            {

                if (isOnline(context))
                {
                   // Intent i = new Intent(context, typeof(autoMonitor));
                   // i.AddFlags(ActivityFlags.NewTask);
                   // i.SetAction(Constants.ACTION_START_SERVICE);
                   // context.StartService(i);
                    count = 0;
                    //DBManager.Instance.CreateTable();
                    var profile = DBManager.Instance.GetAttendance().ToList();
                    if (profile.Count() > 0)
                    {
                        NOTIFICATION_CHANNEL_ID = "BlueAttend.BlueAttend";
                        if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                        {
                            chan = new NotificationChannel(NOTIFICATION_CHANNEL_ID, "BlueAttendNotify", NotificationImportance.High);

                            manager = (NotificationManager)context.GetSystemService(Context.NotificationService);

                            manager.CreateNotificationChannel(chan);
                        }                                             
                        
                        builder = new NotificationCompat.Builder(context, NOTIFICATION_CHANNEL_ID)
                                    .SetContentTitle("Sync Started")
                                    .SetContentText("Please do not disconnect intenet until it's completed")
                                    .SetSmallIcon(Android.Resource.Drawable.IcDialogAlert)
                                    .SetOngoing(false)
                                    .SetAutoCancel(true);

                        notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
                        notificationManager.Notify(222, builder.Build());
                        task1 = Task.Factory.StartNew(() => SyncPostAttendance());  //Post Detected to server
                        task2 = Task.Factory.StartNew(() => SyncAttendance());  //Get Updated Detected users
                        task3 = Task.Factory.StartNew(() => SyncStudents()); //Get Updated News from WHO



                    }
                    // }



                }

            }
        }
        public bool isOnline(Context context)
        {
            ConnectivityManager cm = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);
            NetworkInfo netInfo = cm.ActiveNetworkInfo;
            return netInfo != null && netInfo.IsConnected;
        }
        public static bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = new Ping();
            try
            {
                PingReply reply = pinger.Send(nameOrAddress, 4000);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            return pingable;
        }


        public void SyncPostAttendance()
        {

            ClientSync.Instance.postAttended();
            count += 5;
            checkCompleted(count);
        }
        public void SyncAttendance()
        {
            
            IEnumerable<attendance> data = RestService.Instance.AttendanceAsync().ToList();
            count += 5;
            checkCompleted(count);
        }
        PendingIntent BuildIntentToShowMainActivity()
        {
            var notificationIntent = new Intent(context, typeof(MainActivity));
            notificationIntent.PutExtra("menuFragment", "LatestFragment");

            var pendingIntent = PendingIntent.GetActivity(context, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }
        public void SyncStudents()
        {

            IEnumerable<students> data = RestService.Instance.studentAsync().ToList();
            if (data.Count() > 0)
            {
                foreach (var item in data)
                {
                    builder = new NotificationCompat.Builder(context, NOTIFICATION_CHANNEL_ID)
                                    .SetContentTitle("Notifications from Ethiopian MOH")
                                    .SetContentText("Subject: ")
                                    .SetSmallIcon(Android.Resource.Drawable.IcDialogAlert)
                                    .SetOngoing(false)
                                    .SetAutoCancel(true);

                    notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
                    notificationManager.Notify(40, builder.Build());
                }
            }
            count += 5;
            checkCompleted(count);
        }
        public void checkCompleted(int num)
        {
            if (num >= 15)
            {
                builder = new NotificationCompat.Builder(context, NOTIFICATION_CHANNEL_ID)
                                    .SetContentTitle("Sync Completed")
                                    .SetContentText("All data are Synced with the server")
                                    .SetSmallIcon(Android.Resource.Drawable.IcDialogAlert)
                                    .SetOngoing(false)
                                    .SetAutoCancel(true);

                notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
                notificationManager.Notify(222, builder.Build());

                //notificationManager.Cancel(222);
               
                Task.WhenAll(task1, task2, task3).Dispose();
                UnregisterFromRuntime();
            }
        }
    }
}