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
    public class ReportFragment : Fragment
    {
        ListView lv;
        Spinner drpCourses;
        Button btnGenerate;
        TextView txtAbsent;
        TextView txtAttend;
        TextView txtTotal;
        Context context;
        List<attendance>  attendanceList;
        List<TeacherReport> reports;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.report, container, false);
            lv = view.FindViewById<ListView>(Resource.Id.lv3);
            drpCourses = view.FindViewById<Spinner>(Resource.Id.spnCourses);
            btnGenerate = view.FindViewById<Button>(Resource.Id.btnGenerate);

            txtAbsent = view.FindViewById<TextView>(Resource.Id.txtAbsent);
            txtAttend = view.FindViewById<TextView>(Resource.Id.txtAttend);
            txtTotal = view.FindViewById<TextView>(Resource.Id.txtTotal);
            context = this.Activity;

            var courses = DBManager.Instance.GetCourses().Select(c => c.description).ToArray();
            var arrayadapter = new ArrayAdapter(context, Android.Resource.Layout.SimpleSpinnerItem, courses);
            arrayadapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            drpCourses.Adapter = arrayadapter;

            //Filling the list view data from the sqlite database
            attendanceList = new List<attendance>();
            var selectedCourse = drpCourses.SelectedItem.ToString();
            int courseId = DBManager.Instance.GetCourseById(selectedCourse).id;
            var check = DBManager.Instance.GetAttendanceByDate(DateTimeOffset.Now, courseId);
            attendanceList = check;
            var studList = DBManager.Instance.GetStudent();
            reports = new List<TeacherReport>();
            foreach (var item in studList)
            {
                var attended = DBManager.Instance.GetAttendanceReport().Where(c => c.student_id == item.student_id && c.attended == 1 && c.course_id == courseId).ToList();
                TeacherReport rep = new TeacherReport()
                {
                    name = item.full_name,
                    student_id = item.student_id,
                    attended = attended.Count,
                    absent = 0
                };
                reports.Add(rep);
            }
            lv.Adapter = new TeachReportAdapter(context, reports.ToList());
            txtAbsent.Text = "Absent = " + attendanceList.Where(c => c.attended == 0).Count().ToString();
            txtAttend.Text = "Attended = " + attendanceList.Where(c => c.attended == 1).Count().ToString();
            txtTotal.Text = "Total = " + attendanceList.Count().ToString();

            //Generate button click will generate attendance list
            btnGenerate.Click += btnGenerate_Click;
            return view;
        }
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            //Filling the list view data from the sqlite database
            attendanceList = new List<attendance>();
            var selectedCourse = drpCourses.SelectedItem.ToString();
            int courseId = DBManager.Instance.GetCourseById(selectedCourse).id;
            var check = DBManager.Instance.GetAttendanceByDate(DateTimeOffset.Now, courseId);
            attendanceList = check;
            var studList = DBManager.Instance.GetStudent();
            reports = new List<TeacherReport>();
            foreach (var item in studList)
            {
                var attended = DBManager.Instance.GetAttendance().Where(c => c.student_id == item.student_id && c.attended == 1 && c.course_id == courseId).ToList();
                TeacherReport rep = new TeacherReport()
                {
                    name = item.full_name,
                    student_id = item.student_id,
                    attended = attended.Count,
                    absent = 0
                };
                reports.Add(rep);
            }
            lv.Adapter = new TeachReportAdapter(context, reports.ToList());
            txtAbsent.Text = "Absent = " + attendanceList.Where(c => c.attended == 0).Count().ToString();
            txtAttend.Text = "Attended = " + attendanceList.Where(c => c.attended == 1).Count().ToString();
            txtTotal.Text = "Total = " + attendanceList.Count().ToString();
        }
    }
}