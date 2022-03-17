using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;


namespace BlueAttend.Model
{
    public class DBManager
    {
        private static readonly DBManager instance = new DBManager();
        SQLiteConnection dbConn;
        private const string DB_NAME = "Data.db3";
        private DBManager()
        {

        }
        public static DBManager Instance
        {
            get
            {
                return instance;

            }
        }
        public void CreateTable()
        {
            var path = System.Environment.GetFolderPath(System.Environment.
            SpecialFolder.Personal);
            dbConn = new SQLiteConnection(System.IO.Path.Combine(path, DB_NAME));
            //Create Tables
            dbConn.CreateTable<students>();
            dbConn.CreateTable<user>();
            dbConn.CreateTable<departments>();
            dbConn.CreateTable<course>();
            dbConn.CreateTable<GlobalVariables>();
            dbConn.CreateTable<attendance>();
            dbConn.CreateTable<ActiveClass>();

        }
        public void DeleteTable()
        {
            var path = System.Environment.GetFolderPath(System.Environment.
            SpecialFolder.Personal);
            dbConn = dbConn = new SQLiteConnection(System.IO.Path.Combine(path, DB_NAME));
            dbConn.DropTable<students>();
            dbConn.DropTable<attendance>();
        }
        public int SaveDepartment(departments dept)
        {
            int result = dbConn.InsertOrReplace(dept);
            Console.WriteLine("{0} record updated!", result);
            return result;
        }
        public int SaveCourse(course course)
        {
            int result = dbConn.InsertOrReplace(course);
            Console.WriteLine("{0} record updated!", result);
            return result;
        }
        public int SaveActiveCourse(ActiveClass course)
        {
            int result = dbConn.InsertOrReplace(course);
            Console.WriteLine("{0} record updated!", result);
            return result;
        }
        public int SaveStudent(students stud)
        {
            int result = dbConn.Insert(stud);
            Console.WriteLine("{0} record updated!", result);
            return result;
        }
        public int SaveUser(user stud)
        {
            int result = dbConn.Insert(stud);
            Console.WriteLine("{0} record updated!", result);
            return result;
        }
        public int updateUser(user stud)
        {
            int result = dbConn.InsertOrReplace(stud);
            Console.WriteLine("{0} record updated!", result);
            return result;
        }
        public int SaveAttendance(attendance attend)
        {
            int result = dbConn.Insert(attend);
            Console.WriteLine("{0} record updated!", result);
            return result;
        }
        public int updateAttendance(attendance attend)
        {
            int result = dbConn.InsertOrReplace(attend);
            Console.WriteLine("{0} record updated!", result);
            return result;
        }
        public int SaveGlobalVariables(GlobalVariables variables)
        {
            int result = dbConn.InsertOrReplace(variables);
            Console.WriteLine("{0} record updated!", result);
            return result;
        }

        public List<students> GetStudent()
        {
            var studentList = new List<students>();
            IEnumerable<students> table = dbConn.Table<students>();
            foreach (students stud in table)
            {
                studentList.Add(stud);
            }
            return studentList;
        }
        public int GetActiveClassId()
        {
            var studentList = new List<ActiveClass>();
            IEnumerable<ActiveClass> table = dbConn.Table<ActiveClass>();
            foreach (ActiveClass stud in table)
            {
                studentList.Add(stud);
            }
            return studentList.FirstOrDefault().course_id;
        }
        public ActiveClass GetActiveClass()
        {
            var studentList = new List<ActiveClass>();
            IEnumerable<ActiveClass> table = dbConn.Table<ActiveClass>();
            foreach (ActiveClass stud in table)
            {
                studentList.Add(stud);
            }
            return studentList.FirstOrDefault();
        }
        public List<user> GetUser()
        {
            var studentList = new List<user>();
            IEnumerable<user> table = dbConn.Table<user>();
            foreach (user stud in table)
            {
                studentList.Add(stud);
            }
            return studentList;
        }
        public user GetLoggedIdUser(int id)
        {
            user table = dbConn.Table<user>().Where(c=>c.id == id).FirstOrDefault();         
            return table;
        }
        public List<user> GetUserById(string student_id)
        {
            var studentList = new List<user>();
            IEnumerable<user> table = dbConn.Table<user>().Where(c=>c.username.Equals(student_id));
            foreach (user stud in table)
            {
                studentList.Add(stud);
            }
            return studentList;
        }
        public List<attendance> GetAttendanceById(string student_id)
        {
            var studentList = new List<attendance>();
            IEnumerable<attendance> table = dbConn.Table<attendance>().Where(c => c.student_id.Equals(student_id));
            foreach (attendance stud in table)
            {
                studentList.Add(stud);
            }
            return studentList;
        }
        public List<course> GetCourses()
        {
            var coursetList = new List<course>();
            IEnumerable<course> table = dbConn.Table<course>();
            foreach (course cou in table)
            {
                coursetList.Add(cou);
            }
            return coursetList;
        }
        public course GetCourseById(string course)
        {
            var courses = new course();
            courses = dbConn.Table<course>().Where(c => c.description == course).FirstOrDefault();

            return courses;
        }
        public List<attendance> GetAttendanceByDate(DateTimeOffset date_taken, int courseId)
        {
            var attendances = new List<attendance>();
            IEnumerable<attendance> table = dbConn.Table<attendance>().ToList().Where(c => c.course_id == courseId & DateTimeOffset.Compare(c.date_taken.Date, date_taken.Date) == 0);
            foreach (attendance lat in table)
            {
                attendances.Add(lat);
            }

            return attendances;
        }
        public List<attendance> GetAttendance()
        {
            var attendances = new List<attendance>();
            IEnumerable<attendance> table = dbConn.Table<attendance>().ToList().Where(c =>c.sync_status == false);
            foreach (attendance lat in table)
            {
                attendances.Add(lat);
            }

            return attendances;
        }
        public List<attendance> GetAttendanceReport()
        {
            var attendances = new List<attendance>();
            IEnumerable<attendance> table = dbConn.Table<attendance>().ToList();
            foreach (attendance lat in table)
            {
                attendances.Add(lat);
            }

            return attendances;
        }
        public List<attendance> GetAttendances()
        {
            var attendances = new List<attendance>();
            IEnumerable<attendance> table = dbConn.Table<attendance>().ToList();
            foreach (attendance lat in table)
            {
                attendances.Add(lat);
            }

            return attendances;
        }
        public GlobalVariables GetGlobalvariable()
        {
            GlobalVariables stud = new GlobalVariables();
            stud = dbConn.Table<GlobalVariables>().FirstOrDefault();
            return stud;
        }
        public attendance GetAttendanceLastDate()
        {
            var table = dbConn.Table<attendance>().OrderByDescending(c => c.id).Take(1).FirstOrDefault();
            return table;
        }
        public List<attendance> GetAttendedAfterUpdate()
        {
            var last_updated = DBManager.instance.GetGlobalvariable().lastSync;
            var attendedList = new List<attendance>();
            IEnumerable<attendance> table = dbConn.Table<attendance>().Where(c => c.sync_status == false);
            foreach (attendance detect in table)
            {
                attendedList.Add(detect);
            }
            return attendedList;
        }
        public int login(string username, string password)
        {
            
            var users = dbConn.Table<user>().Where(c => c.username.Equals(username) & c.password.Equals(password)).FirstOrDefault();
            if (users == null)
                return 0;
            else
                return 1;
        }

    }
}