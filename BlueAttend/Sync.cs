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

namespace BlueAttend
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new string[] { "android.net.conn.CONNECTIVITY_CHANGE" })]
    [Android.Runtime.Preserve(AllMembers = true)]
    public class Sync : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Toast.MakeText(context, "Login Successfull", ToastLength.Short).Show();
        }
    }
}