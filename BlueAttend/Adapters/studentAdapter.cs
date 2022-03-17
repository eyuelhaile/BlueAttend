using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BlueAttend.Model;

namespace BlueAttend.Adapters
{
    class studentAdapter : BaseAdapter
    {

        Context context;
        List<attendance> items;

        public studentAdapter(Context context, List<attendance> items)
        {
            this.context = context;
            this.items = items;
        }


        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view = convertView;
            if (view == null) // no view to re-use, create new
                //view = context.LayoutInflater.Inflate(Resource.Layout.CustomRow, null);
                view = LayoutInflater.From(context).Inflate(Resource.Layout.customrow, null, false);
            
            view.FindViewById<TextView>(Resource.Id.Text1).Text = item.name;
            view.FindViewById<TextView>(Resource.Id.Text2).Text = item.student_id + " "+ ((item.attended == 1) ? "Attended" : "Absent");
            if (item.attended == 1)
            {
                view.FindViewById<TextView>(Resource.Id.Text2).SetTextColor(Android.Graphics.Color.Green);       
            }
            else
            {
                view.FindViewById<TextView>(Resource.Id.Text2).SetTextColor(Android.Graphics.Color.Red);
            }
                //view.FindViewById<TextView>(Resource.Id.textStatus).Text = (item.attended == 1) ? "Attended" : "Absent";

                //var imageBitmap = GetImageBitmapFromUrl(item.ImageURI);
                view.FindViewById<ImageView>(Resource.Id.Image).SetImageResource(Resource.Drawable.notification_icon_background);
            return view;
        }

        //Fill in cound here, currently 0
        public override int Count
        {
            get
            {
                return items.Count;
            }
        }
        private Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;
            if (!(url == "null"))
                using (var webClient = new WebClient())
                {
                    var imageBytes = webClient.DownloadData(url);
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    }
                }

            return imageBitmap;
        }

    }

    class studentAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}