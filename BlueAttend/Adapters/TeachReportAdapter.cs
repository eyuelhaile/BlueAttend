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

namespace BlueAttend.Adapters
{
    class TeachReportAdapter : BaseAdapter
    {

        Context context;
        List<TeacherReport> items;

        public TeachReportAdapter(Context context, List<TeacherReport> items)
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
                view = LayoutInflater.From(context).Inflate(Resource.Layout.teachers_report_view, null, false);

            view.FindViewById<TextView>(Resource.Id.Text1).Text = item.name;
            view.FindViewById<TextView>(Resource.Id.Text2).Text = item.student_id;
            view.FindViewById<TextView>(Resource.Id.Text3).Text = "Attended: " + item.attended + " Absent: " + item.absent;


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

    class TeachReportAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}