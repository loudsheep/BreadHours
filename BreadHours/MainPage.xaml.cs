using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BreadHours
{
    class Activity
    {
        public string desc;
        public float hours, pay;

        public Activity(string desc, float hours, float pay)
        {
            this.desc = desc;
            this.hours = hours;
            this.pay = pay;
        }

        public float TotalPay()
        {
            return hours * pay;
        }
    }

    public partial class MainPage : ContentPage
    {
        private Dictionary<string, float> breads = new Dictionary<string, float>() 
        {
            {"Wheat bread", 1.2f },
            {"Graham bread", 2f },
            {"No-gluten bread", 1.05f },
        };

        private List<Activity> activities = new List<Activity>();

        public MainPage()
        {
            InitializeComponent();
            for(int i=0; i<breads.Count; i++)
            {
                breadPicker.Items.Add(breads.ElementAt(i).Key);
            }
            breadPicker.SelectedIndex = 0;
        }

        private void addBtn_Clicked(object sender, EventArgs e)
        {
            if(descEntry.Text == "" || hourEntry.Text == "" || payEntry.Text == "")
            {
                DisplayAlert("Error", "Fill all fields", "OK");
                return;
            }

            float hours, pay;
            if(!float.TryParse(hourEntry.Text, out hours) || !float.TryParse(payEntry.Text, out pay))
            {
                DisplayAlert("Error", "Enter valid data", "OK");
                return;
            }

            activities.Add(new Activity(descEntry.Text, hours, pay));

            RedrawActivieties();
        }

        private float GetBreadCount(float pay)
        {
            float price = breads.ElementAt(breadPicker.SelectedIndex).Value;
            return (float)Math.Round(pay / price, 2);
        }


        private void RedrawActivieties()
        {
            listActivieties.Children.Clear();
            float sumH = 0, sumPay = 0;
            for(int i=0; i<activities.Count; i++)
            {
                sumH += activities[i].hours;
                sumPay += activities[i].TotalPay();

                StackLayout st = new StackLayout();
                st.Orientation = StackOrientation.Horizontal;
                st.HorizontalOptions = LayoutOptions.FillAndExpand;

                var tap = new TapGestureRecognizer();
                tap.Tapped += Tap_Tapped;
                st.GestureRecognizers.Add(tap);

                Label l1 = new Label();
                l1.Text = (i + 1) + ".";
                l1.VerticalOptions = LayoutOptions.Center;
                l1.HorizontalOptions = LayoutOptions.FillAndExpand;

                Label l2 = new Label();
                l2.Text = activities[i].desc;
                l2.VerticalOptions = LayoutOptions.Center;
                l2.HorizontalOptions = LayoutOptions.FillAndExpand;

                Label l3 = new Label();
                l3.Text = activities[i].hours + "h";
                l3.VerticalOptions = LayoutOptions.Center;
                l3.HorizontalOptions = LayoutOptions.FillAndExpand;

                Label l4 = new Label();
                l4.Text = activities[i].pay + " pay/h";
                l4.VerticalOptions = LayoutOptions.Center;
                l4.HorizontalOptions = LayoutOptions.FillAndExpand;

                st.Children.Add(l1);
                st.Children.Add(l2);
                st.Children.Add(l3);
                st.Children.Add(l4);

                listActivieties.Children.Add(st);
            }

            resultLabel.Text = "Hours: " + sumH + "h, Pay: " + sumPay + ", In bread: " + GetBreadCount(sumPay) + " (" + breads.ElementAt(breadPicker.SelectedIndex).Value + " each)";
        }

        private int GetIdTapped(StackLayout st)
        {
            for(int i= 0; i < listActivieties.Children.Count; i++)
            {
                if (st == listActivieties.Children[i])
                {
                    return i;
                }
            }
            return -1;
        }

        private async void Tap_Tapped(object sender, EventArgs e)
        {
            if(sender is StackLayout)
            {
                StackLayout st = (StackLayout)sender;
                if(GetIdTapped(st) != -1)
                {
                    bool answer = await DisplayAlert("Question?", "Are you sure to delete this item", "Yes", "No");

                    if(answer)
                    {
                        activities.RemoveAt(GetIdTapped(st));
                        RedrawActivieties();
                    }
                } 
            }
        }

        private void breadPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            RedrawActivieties();
        }
    }
}
