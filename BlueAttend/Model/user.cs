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
   public class user
    {
        [NotNull, PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string user_type { get; set; }
        public string student_id { get; set; }
        public int active { get; set; }
    }
}