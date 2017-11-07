using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using FrogCroak.MyMethod;
using Android.Support.V4.App;
using static Android.Support.V4.View.ViewPager;

namespace FrogCroak
{
    public class IntroductionFragment : Fragment
    {
        private ViewPager vp_Introduction;
        private RadioGroup rg_Introduction;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.IntroductionFragment, container, false);
            vp_Introduction = (ViewPager)view.FindViewById(Resource.Id.vp_Introduction);
            rg_Introduction = (RadioGroup)view.FindViewById(Resource.Id.rg_Introduction);

            LayoutInflater mInflater = LayoutInflater.From(Activity.ApplicationContext);

            View v1 = mInflater.Inflate(Resource.Layout.Intro1, null);
            View v2 = mInflater.Inflate(Resource.Layout.Intro2, null);
            View v3 = mInflater.Inflate(Resource.Layout.Intro3, null);
            View v4 = mInflater.Inflate(Resource.Layout.Intro4, null);

            List<View> viewList = new List<View>();
            viewList.Add(v1);
            viewList.Add(v2);
            viewList.Add(v3);
            viewList.Add(v4);

            vp_Introduction.Adapter = new IntroPagerAdapter(viewList, Activity);
            vp_Introduction.CurrentItem = 0;
            vp_Introduction.AddOnPageChangeListener(new OnPageChangeListener(rg_Introduction));
            return view;
        }

        private class OnPageChangeListener : Java.Lang.Object,IOnPageChangeListener
        {

            RadioGroup rg_Introduction;

            public OnPageChangeListener(RadioGroup rg)
            {
                rg_Introduction = rg;
            }
            public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
            {
            }

            public void OnPageScrollStateChanged(int state)
            {
            }

            public void OnPageSelected(int position)
            {
                switch (position)
                {
                    case 0:
                        rg_Introduction.Check(Resource.Id.rb_page1);
                        break;
                    case 1:
                        rg_Introduction.Check(Resource.Id.rb_page2);
                        break;
                    case 2:
                        rg_Introduction.Check(Resource.Id.rb_page3);
                        break;
                    case 3:
                        rg_Introduction.Check(Resource.Id.rb_page4);
                        break;
                }
            }
        }
    }
}