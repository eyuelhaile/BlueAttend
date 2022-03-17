using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BlueAttend.dbsync;
using BlueAttend.Model;

namespace BlueAttend
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class SplashScreen : Activity
    {
        ProgressBar PB1;
        TextView lblDateTime;
        TextView versionName;
        int pr = 0;
        object _lock = new object();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.splashScreen);
            PB1 = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            lblDateTime = FindViewById<TextView>(Resource.Id.lblDateTime);
            versionName = FindViewById<TextView>(Resource.Id.lblVersionName);
            PB1.Max = 100;
            PB1.Progress = pr;

            PackageManager manager = this.PackageManager;
            PackageInfo info = manager.GetPackageInfo(this.PackageName, 0);
            string version = info.VersionName;
            versionName.Text = "Version: " + version;
            createTables();
            
            if (isOnline())
            {
                Thread tr1 = new Thread(new ThreadStart(SyncPostAttendance));
                Thread tr2 = new Thread(new ThreadStart(SyncAttendance));
                Thread tr3 = new Thread(new ThreadStart(SyncStudents));
                Thread tr4 = new Thread(new ThreadStart(SyncUsers));
                tr1.Start();
                tr2.Start();
                tr3.Start();
                tr4.Start();
            }
            else
            {
                RunOnUiThread(() =>
                {
                    PB1.Progress += 20;
                    PB1.Progress += 20;
                    PB1.Progress += 20;
                    pr += PB1.Progress;
                    CheckProgress(PB1.Progress);
                });
            }
            
        }
        public bool isOnline()
        {
            ConnectivityManager cm = (ConnectivityManager)this.GetSystemService(Context.ConnectivityService);
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
            RunOnUiThread(() =>
            {
                PB1.Progress += 20;
                pr += PB1.Progress;
                CheckProgress(PB1.Progress);
            });

        }
        public void SyncAttendance()
        {

            IEnumerable<attendance> data = RestService.Instance.AttendanceAsync().ToList();
            RunOnUiThread(() =>
            {
                PB1.Progress += 20;
                pr += PB1.Progress;
                CheckProgress(PB1.Progress);
            });
        }
        public void SyncStudents()
        {
            var studs = DBManager.Instance.GetStudent();
            if(studs.Count == 0)
            {
                IEnumerable<students> data = RestService.Instance.studentAsync().ToList();
            }
           
            RunOnUiThread(() =>
            {
                PB1.Progress += 20;
                pr += PB1.Progress;
                CheckProgress(PB1.Progress);
            });
        }
        public void SyncUsers()
        {
            var studs = DBManager.Instance.GetUser();
            if (studs.Count == 0)
            {
                IEnumerable<user> data = RestService.Instance.userAsync().ToList();
            }

            RunOnUiThread(() =>
            {
                PB1.Progress += 20;
                pr += PB1.Progress;
                CheckProgress(PB1.Progress);
            });
        }
        public void CheckProgress(int progress)
        {
            lock (_lock)
            {
                if (progress >= 80)
                {
                    var global = DBManager.Instance.GetGlobalvariable();
                    global.lastSync = DateTimeOffset.Now;
                    DBManager.Instance.SaveGlobalVariables(global);
                    //int Role = account.Role;  
                    var logedin_id = DBManager.Instance.GetGlobalvariable().login_id;
                    if(logedin_id == 0)
                    {
                        StartActivity(typeof(Login));        // Navigate to Main activity
                    }
                    else
                    {
                        StartActivity(typeof(MainActivity));        // Navigate to Main activity
                    }
                    
                    Finish();
                }
            }
        }
        public void createTables()
        {
            // DBManager.Instance.DeleteTable();

            DBManager.Instance.CreateTable();
            var check = DBManager.Instance.GetGlobalvariable();
            if (check == null)
            {
                GlobalVariables global = new GlobalVariables();
                global.id = 1;
                global.BaseURL = "http://192.168.137.76:81/blueattend/index.php/";
                global.lastSync = DateTimeOffset.MinValue;
                global.login_id = 0;
                DBManager.Instance.SaveGlobalVariables(global);
            }


            course course = new course()
            {
                id = 1,
                description = "Mobile Computing"
            };
            course course2 = new course()
            {
                id = 2,
                description = "Network Design"
            };
            ActiveClass active = new ActiveClass()
            {
                id = 1,
                course_id = 1,
                active = 1
            };
            DBManager.Instance.SaveActiveCourse(active);
            DBManager.Instance.SaveCourse(course);
            DBManager.Instance.SaveCourse(course2);

        }
    }
}