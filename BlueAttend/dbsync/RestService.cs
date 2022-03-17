using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net.NetworkInformation;
using System.Linq;
using BlueAttend.Model;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net;
using BlueAttend.dbsync;

namespace BlueAttend.dbsync
{
    public class RestService
    {
        private static RestService instance = new RestService();
        string user_id1 = "";
        string password = "";
        private RestService()
        {
            

        }
        public static RestService Instance
        {
            get
            {
                return instance;
            }
        }
        //Sync Updated record from server to the database
        
        public List<students> studentAsync()
        {
          
            List<students> Items = new List<students>();
            try
                {
                    HttpResponseMessage response = GlobalVariable.webApiClient.GetAsync("attend/getStudents").Result;
                    if (response.IsSuccessStatusCode && response != null)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        var studList = JsonConvert.DeserializeObject<List<students>>(content);
                        foreach (var item in studList)
                        {
                        DBManager.Instance.SaveStudent(item);
                           
                        }
                    }
  
                }
                catch (AggregateException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message + " " + ex.StackTrace);
                }
            
            
            return Items;
        }
        public List<user> userAsync()
        {

            List<user> Items = new List<user>();
            try
            {
                HttpResponseMessage response = GlobalVariable.webApiClient.GetAsync("attend/getUsers").Result;
                if (response.IsSuccessStatusCode && response != null)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var userList = JsonConvert.DeserializeObject<List<user>>(content);
                    foreach (var item in userList)
                    {
                        DBManager.Instance.SaveUser(item);

                    }
                }

            }
            catch (AggregateException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message + " " + ex.StackTrace);
            }


            return Items;
        }
        public List<attendance> AttendanceAsync()
        {
            List<attendance> Items = new List<attendance>();
            var last_updated = DBManager.Instance.GetGlobalvariable().lastSync;
            string syncDate;

            if (last_updated == null)
            {
                syncDate = DateTimeOffset.MinValue.AddYears(1).ToString();
             }
            else
            {
                syncDate = last_updated.ToString();
            }
            DateTimeOffset datee = Convert.ToDateTime(syncDate);
            syncDate = datee.Date.ToString("yyyyMMddHHmmss");


            try
                {
                    HttpResponseMessage response = GlobalVariable.webApiClient.GetAsync("attend/getAttendance?query=" + syncDate).Result;
                    if (response.IsSuccessStatusCode && response != null)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        var attendList = JsonConvert.DeserializeObject<List<attendance>>(content);
                       foreach (var item in attendList)
                       {
                            item.sync_status = true;
                            DBManager.Instance.SaveAttendance(item);

                       }
                }

                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message + " " + ex.StackTrace);
                }
            
            
            
             return Items;
        }

        public HttpResponseMessage Login(string username, string password, int schoolid)
        {
           
            HttpResponseMessage response;
            try
            {                              
                 response = GlobalVariable.webApiClient.GetAsync("api/Accounts/" + username + "/" + password + "/" + schoolid).Result;               
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return response;
        }
        public HttpResponseMessage ChangePassword(string username, string current, string New, int schoolid)
        {
            HttpResponseMessage response;
            try
            {
                response = GlobalVariable.webApiClient.GetAsync("api/Accounts/" + username + "/" + current + "/"+ New + "/" + schoolid).Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }
       

    }
}