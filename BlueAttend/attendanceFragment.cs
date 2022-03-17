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
    public class attendanceFragment : Fragment
    {
        ListView lv;
        Spinner drpCourses;
        Button btnGenerate;
        TextView txtAbsent;
        TextView txtAttend;
        TextView txtTotal;
        List<students> student;
        List<course> cours;
        Context context;
        ProgressDialog dialog;
        List<attendance> attendanceList;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            
            View view = inflater.Inflate(Resource.Layout.attendance, container, false);
            lv = view.FindViewById<ListView>(Resource.Id.lv1);
            drpCourses = view.FindViewById<Spinner>(Resource.Id.spinner1);
            btnGenerate = view.FindViewById<Button>(Resource.Id.button1);

            txtAbsent = view.FindViewById<TextView>(Resource.Id.txtAbsent);
            txtAttend = view.FindViewById<TextView>(Resource.Id.txtAttend);
            txtTotal = view.FindViewById<TextView>(Resource.Id.txtTotal);
            context = this.Activity;
            //Filling the drop down spinner with courses from the sqlite database
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
            lv.Adapter = new studentAdapter(context, attendanceList.OrderByDescending(c=>c.attended).ToList());
            txtAbsent.Text = "Absent = " + attendanceList.Where(c => c.attended == 0).Count().ToString();
            txtAttend.Text = "Attended = " + attendanceList.Where(c => c.attended == 1).Count().ToString();
            txtTotal.Text = "Total = " + attendanceList.Count().ToString();

            //Generate button click will generate attendance list
            btnGenerate.Click += btnGenerate_Click;

            //Drop down course Select
            drpCourses.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) =>
            {
                string course = e.Position.ToString();
                string selected = drpCourses.SelectedItem.ToString();
                int course_id = DBManager.Instance.GetCourseById(selected).id;
                var check2 = DBManager.Instance.GetAttendanceByDate(DateTimeOffset.Now, course_id);
                attendanceList = check2;
                lv.Adapter = new studentAdapter(context, attendanceList.ToList());
                txtAbsent.Text = "Absent = " + attendanceList.Where(c => c.attended == 0).Count().ToString();
                txtAttend.Text = "Attended = " + attendanceList.Where(c => c.attended == 1).Count().ToString();
                txtTotal.Text = "Total = " + attendanceList.Count().ToString();

                var active = DBManager.Instance.GetActiveClass();
                active.course_id = course_id;
                DBManager.Instance.SaveActiveCourse(active);

            };

            return view;
        }
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            dialog = new ProgressDialog(context);
            dialog.SetMessage("Generating Attendance List...");
            dialog.Indeterminate = true;
            dialog.SetCanceledOnTouchOutside(false);
            dialog.Show();
            generateAttendance();
        }
        public void generateAttendance()
        {
            var selectedCourse = drpCourses.SelectedItem.ToString();
            int courseId = DBManager.Instance.GetCourseById(selectedCourse).id;
            var check = DBManager.Instance.GetAttendanceByDate(DateTimeOffset.Now, courseId);
            if(check.Count == 0)
            {
                var students = DBManager.Instance.GetStudent().Where(c => c.department_id == 1).ToList();
                foreach (var item in students)
                {
                    attendance attend = new attendance()
                    {
                        date_taken = DateTimeOffset.Now,
                        course_id = courseId,
                        student_id = item.student_id,
                        name = item.full_name,
                        uri = item.ImageURI,
                        attended = 0,
                        sync_status = false
                    };
                    DBManager.Instance.SaveAttendance(attend);
                }
                attendanceList = DBManager.Instance.GetAttendanceByDate(DateTimeOffset.Now, courseId);
                lv.Adapter = new studentAdapter(context, attendanceList.OrderByDescending(c => c.attended).ToList());
                txtAbsent.Text = "Absent = " +  attendanceList.Where(c => c.attended == 0).Count().ToString();
                txtAttend.Text = "Attended = " + attendanceList.Where(c => c.attended == 1).Count().ToString();
                txtTotal.Text = "Total = " + attendanceList.Count().ToString();
            }
            else
            {
                attendanceList = check;
                lv.Adapter = new studentAdapter(context, attendanceList.OrderByDescending(c => c.attended).ToList());
                txtAbsent.Text = "Absent = " + attendanceList.Where(c => c.attended == 0).Count().ToString();
                txtAttend.Text = "Attended = " + attendanceList.Where(c => c.attended == 1).Count().ToString();
                txtAbsent.Text = "Total = " + attendanceList.Count().ToString();
            }
            dialog.Dismiss();
            
        }

        
    }
}