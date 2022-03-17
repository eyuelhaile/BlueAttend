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
using BlueAttend.Model;

namespace BlueAttend
{
    [Activity(Label = "Login")]
    public class Login : Activity
    {
        EditText txtUsername;
        EditText txtPassword;
        Button btnLogin;
        TextView txtError;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.login);
            txtUsername = FindViewById<EditText>(Resource.Id.userName);
            txtPassword = FindViewById<EditText>(Resource.Id.password);
            btnLogin = FindViewById<Button>(Resource.Id.login);
            txtError = FindViewById<TextView>(Resource.Id.txtError);
            btnLogin.Click += btnLogin_Click;



        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            int login = DBManager.Instance.login(username, password);
            if(login == 0)
            {
                txtError.Text = "Username or Password Incorrect";
                
            }
            else
            {
                var user = DBManager.Instance.GetUserById(username).FirstOrDefault();
                user.active = 1;
                DBManager.Instance.updateUser(user);
                var global = DBManager.Instance.GetGlobalvariable();
                global.login_id = user.id;
                DBManager.Instance.SaveGlobalVariables(global);
                StartActivity(typeof(MainActivity));        // Navigate to Main activity
                Finish();
                
            }
        }
    }
}