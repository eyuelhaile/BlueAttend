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
   public class attendance
    {
        [NotNull, PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public DateTimeOffset date_taken { get; set; }
        public int course_id { get; set; }
        public string student_id { get; set; }
        public string name { get; set; }
        public string uri { get; set; }
        public int attended { get; set; }
        public bool sync_status { get; set; }
    }
}