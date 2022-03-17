using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using BlueAttend.Model;
using BlueAttend.dbsync;

namespace BlueAttend.dbsync
{
    public class ClientSync
    {
      
        private static ClientSync instance = new ClientSync();
        int sid = 0;
        private ClientSync()
        {
           // sid = DBManager.Instance.GetAllLoginProfile().First().schoolID;
        }
        public static ClientSync Instance
        {
            get
            {
                return instance;
            }
        }

        //Sync Not Synced records to the Server
        public void postAttended()
        {
            try
            {
                List<attendance> attended = DBManager.Instance.GetAttendedAfterUpdate().ToList();
                if (attended.Count > 0)
                {
                    foreach (var item in attended)
                    {
                        var json = JsonConvert.SerializeObject(item);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        HttpResponseMessage response = GlobalVariable.webApiClient.PostAsync("attend/crudAttendance", content).Result;

                        var content1 = response.Content.ReadAsStringAsync().Result;
                        item.sync_status = true;
                        DBManager.Instance.updateAttendance(item);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message + " " + ex.StackTrace);
            }
        }
      
    }
}