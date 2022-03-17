using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using BlueAttend.Adapters;
using BlueAttend.Model;

namespace BlueAttend
{
    public class studReport : Fragment
    {
        ListView lv;
        List<report> reports;
        TextView txtNameStudent;
        Context context;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.student_report, container, false);
            lv = view.FindViewById<ListView>(Resource.Id.lv2);
            txtNameStudent = view.FindViewById<TextView>(Resource.Id.txtNameStudent);
            
            reports = new List<report>();
            context = this.Activity;
            int user_id = DBManager.Instance.GetGlobalvariable().login_id;
            string username = DBManager.Instance.GetUser().Where(c => c.id == user_id).FirstOrDefault().username;

            string full_name = DBManager.Instance.GetStudent().Where(c => c.student_id == username).FirstOrDefault().full_name;
            txtNameStudent.Text = full_name;
            txtNameStudent.SetTextColor(Android.Graphics.Color.Blue);
           // var attended = DBManager.Instance.GetAttendance().Where(c => c.student_id.Equals(username) && c.attended == 1).ToList();
            var total = DBManager.Instance.GetAttendances().ToList().Where(c=>c.student_id == username).ToList();
            if(total.Count > 0)
            {
                foreach(var item in total)
                {
                 
                        int courseId = item.course_id;
                        string courseName = DBManager.Instance.GetCourses().Where(c=>c.id == courseId).FirstOrDefault().description;
                    var course_total = DBManager.Instance.GetAttendances().ToList().Where(c => c.student_id == username && c.course_id == courseId).ToList();
                    var attended= DBManager.Instance.GetAttendances().ToList().Where(c => c.student_id == username && c.attended == 1 && c.course_id == courseId).ToList();
                    ///var attended = DBManager.Instance.GetAttendance().Where(c => c.student_id.Equals(username) && c.attended == 1 && c.course_id == courseId).ToList();
                    var check = reports.Where(c => c.description == courseName).ToList();
                    if(check.Count > 0)
                    {
                        continue;
                    }
                        report report = new report()
                        {
                            description = courseName,
                            attended = attended.Count(),
                            total = course_total.Count()
                        };
                        reports.Add(report);
                    
                }
                lv.Adapter = new studreportAdapter(context, reports.ToList());
            }

            return view;
        }
    }
}