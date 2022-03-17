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
using SQLite;

namespace BlueAttend.Model
{
    public class GlobalVariables
    {
        [NotNull, PrimaryKey, AutoIncrement]
        public int id { get; set; }
        [NotNull]
        public string BaseURL { get; set; }
        public DateTimeOffset? lastSync { get; set; }
        public int login_id { get; set; }
        public Guid guid { get; set; }
    }
}