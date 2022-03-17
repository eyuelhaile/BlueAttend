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

namespace BlueAttend.Model
{
   public  class TeacherReport
    {
        public string name { get; set; }
        public string student_id { get; set; }
        public int attended { get; set; }
        public int absent { get; set; }
    }
}