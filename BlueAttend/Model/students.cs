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
    public class students
    {
        [NotNull, PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string student_id { get; set; }
        public string full_name { get; set; }
        public int department_id { get; set; }
        public string ImageURI { get; set; }
        public int attended { get; set; }
    }
}