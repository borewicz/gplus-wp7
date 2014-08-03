using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace GoogApp
{
    public class RepairedCheckBox : CheckBox
    {
        public override string ToString()
        {
            return string.Empty;
        }
    }

    public partial class AddToCircles : PhoneApplicationPage
    {
        public class CircleUser
        {
            public string name { get; set; }
            public string circleID { get; set; }
            public bool containUser { get; set; }
        }

        string userID;
        //List<Circle> circles;
        List<CircleUser> circleUser = new List<CircleUser>();
        public AddToCircles()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e) 
        {
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.TryGetValue("userID", out userID))
            {
                List<Circle> circles = await Global.googLib.GetCircles();
                //circlesList.ItemsSource = circles;
                List<CircleUser> circles2 = new List<CircleUser>();
                foreach (var c in circles)
                {
                    CircleUser circle = new CircleUser();
                    circle.containUser = c.users.Any(u => u.id == userID);
                    circle.name = c.name;
                    circle.circleID = c.id;
                    if (circle.circleID != "15")
                        circles2.Add(circle);
                }
                circlesList.ItemsSource = circles2;
            }         
        }

        private async void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //ListBoxItem checedItem = this.circlesList.ItemContainerGenerator.ContainerFromItem((sender as CheckBox).DataContext) as ListBoxItem;
            int result = await Global.googLib.AddPerson((string)(sender as CheckBox).Tag, userID);
            if (result == 0)
            {
                (sender as CheckBox).IsChecked = true;
            }
        }

        private async void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //ListBoxItem checedItem = this.circlesList.ItemContainerGenerator.ContainerFromItem((sender as CheckBox).DataContext) as ListBoxItem;
            int result = await Global.googLib.RemovePerson((string)(sender as CheckBox).Tag, userID);
            if (result == 0)
            {
                (sender as CheckBox).IsChecked = false;
            }
        }
    }
}