using System;

using Android.Views;
using Android.Widget;
using BlueAttend.Model;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

namespace BlueAttend.dbsync
{
   public static class GlobalVariable
   {
        public static HttpClient webApiClient = new HttpClient();
        public static readonly int SYNC_STATUS_OK = 1;
        public static readonly int SYNC_STATUS_FAILED = 0;
        static GlobalVariable()
        {
            string uri = DBManager.Instance.GetGlobalvariable().BaseURL;
            webApiClient.BaseAddress = new Uri(uri);
            //webApiClient.Timeout = TimeSpan.FromSeconds(3);
            webApiClient.DefaultRequestHeaders.Clear();
            webApiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

    }
}