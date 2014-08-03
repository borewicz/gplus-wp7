using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Collections.ObjectModel;
using GoogApp.Translations;

namespace GoogApp
{
    public partial class Notifications : PhoneApplicationPage
    {
        //Notify notify;
        ObservableCollection<Notification> notifications;

        public Notifications()
        {
            InitializeComponent();
            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = AppResources.markAsRead;
        }

        protected override async void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            //if (notify == null)
            //if (notifications != null)
            //{
                //notify = await Global.googLib.ShowNotification();
                //NotifyListBox.ItemsSource = notify.notifications;
                notifications = await Global.googLib.GetUnreadNotifications();
                NotifyListBox.ItemsSource = notifications;
                //Global.googLib.
                /*
                for (int i = 0; i < notify.unreadCount; i++)
                {
                    ListBoxItem lbi1 = (ListBoxItem)NotifyListBox.ItemContainerGenerator.ContainerFromIndex(i);
                    lbi1.Background = new SolidColorBrush(Colors.Gray);
                }
                 */
            //}
            //notify.notifications[0].activities[0].
        }

        private async void showPostTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            /*
            //Notify.Notification notify = (sender as ListBox).SelectedItem as Notify.Notification;
            Notify.Activity activity = (sender as ListBox).SelectedItem as Notify.Activity;
            Notify.Notification notifies = (from n in notify.notifications
                                            where n.activities.Contains(activity)
                                            select n).FirstOrDefault();
            if (notifies != null)
                if (notifies.post != null)
                    NavigationService.Navigate(new Uri("/Activity.xaml?postID=" + notifies.post.postID + "&userID=" + activity.userID, UriKind.Relative));
                else if (notifies.communityID != null)
                    NavigationService.Navigate(new Uri("/CommunityView.xaml?communityID=" + notifies.communityID, UriKind.Relative));
                else
                    NavigationService.Navigate(new Uri("/Profile.xaml?userID=" + activity.userID, UriKind.Relative));
             */
            Notification notification = (sender as ListBox).SelectedItem as Notification;
            if (notification != null)
            {
                int i = await Global.googLib.SetReadState(notification.id);
                if (notification.postID != null)
                    //return;
                    NavigationService.Navigate(new Uri("/Activity.xaml?postID=" + notification.postID + "&userID=" + notification.userID, UriKind.Relative));
                else if (notification.communityID != null)
                    NavigationService.Navigate(new Uri("/CommunityView.xaml?communityID=" + notification.communityID, UriKind.Relative));
                else if (notification.userID != null)
                    NavigationService.Navigate(new Uri("/Profile.xaml?userID=" + notification.userID, UriKind.Relative));
                else if (notification.eventID != null)
                    NavigationService.Navigate(new Uri("/EventPage.xaml?eventID=" + notification.eventID, UriKind.Relative));
                else if (notification.url != null)
                {
                    Global.webBrowser.Uri = new Uri(notification.url);
                    Global.webBrowser.Show();
                }              
                notifications.Remove(notification);
            }
        }

        private async void ApplicationBarIconButton_Click_1(object sender, System.EventArgs e)
        {
            int i; 
            foreach (var n in notifications)
                i = await Global.googLib.SetReadState(n.id);
            notifications.Clear();
        }
    }
}