using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
    class studreportAdapter : BaseAdapter
    {
        Context context;
        List<report> items;

        public studreportAdapter(Context context, List<report> items)
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
                view = LayoutInflater.From(context).Inflate(Resource.Layout.studReport_view, null, false);

            view.FindViewById<TextView>(Resource.Id.Text1).Text = item.description;
            view.FindViewById<TextView>(Resource.Id.Text2).Text = "Attended " + item.attended.ToString() + " From " + item.total.ToString() + " Attendances";

            
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
      

    }

    class studreportAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}