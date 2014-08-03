using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Windows.Threading;
using GoogApp.Translations;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace GoogApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        Posts posts;
        DispatcherTimer dt = new DispatcherTimer();
        MarketplaceDetailTask _marketPlaceDetailTask = new MarketplaceDetailTask();
        bool navigated = false;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = AppResources.newPost;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).Text = AppResources.search;
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).Text = AppResources.refresh;
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[1]).Text = AppResources.yourProfile;
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[2]).Text = AppResources.googleBrowser;
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[3]).Text = AppResources.about;
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[4]).Text = AppResources.reportBug;
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[5]).Text = AppResources.signOut;


            if ((Application.Current as App).IsTrial)
            {
                ApplicationBarIconButton button1 = new ApplicationBarIconButton();
                button1.IconUri = new Uri("/Assets/AppBar/appbar.marketplace.png", UriKind.Relative);
                button1.Text = AppResources.buy;
                ApplicationBar.Buttons.Add(button1);
                button1.Click += new EventHandler(button1_Click);
            }
            
            dt.Interval = TimeSpan.FromSeconds(30);
            //SystemTray.SetProgressIndicator(this, Global.prog); 
                       
            // Set the data context of the listbox control to the sample data
            //this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Load data for the ViewModel Items
        //private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        //{

        //}

        private void button1_Click(object sender, EventArgs e)
        {
            _marketPlaceDetailTask.Show();
        }

        protected override async void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            dt.Start();
            dt.Tick += async delegate
            {
                notificationButton.Content = await Global.googLib.GetUnreadNotificationsCount();
                if ((int)notificationButton.Content > 0)
                    notificationButton.Background = new SolidColorBrush(Colors.Red);
                else
                    notificationButton.Background = new SolidColorBrush(Colors.Gray);
            };
            if (navigated == true)
                return;
            navigated = true;
            //Global.prog.IsVisible = true;
            //Global.prog.IsIndeterminate = true;
            base.OnNavigatedTo(e);
            do
                NavigationService.RemoveBackEntry();
            while (NavigationService.CanGoBack != false);
            if (posts == null)
            {
                posts = await Global.googLib.GetActivities(null, null, null);
                StreamListBox.ItemsSource = posts.posts;
            }

            CommunitiesListBox.ItemsSource = Global.googLib.communities;
            CirclesListBox.ItemsSource = Global.googLib.circles;
            int count = await Global.googLib.GetUnreadNotificationsCount();
            notificationButton.Content = count;
            if (count > 0)
                notificationButton.Background = new SolidColorBrush(Colors.Red);
            else
                notificationButton.Background = new SolidColorBrush(Colors.Gray);
            //if (Global.googLib.IsConnected() == true)
            //Global.googLib._getCookie("kredens23@outlook.com", "Dupa1234");
            //else
            //{

            loadingProgressBar.IsVisible = false;
            //}
            //else
            //    MessageBox.Show("Error");
            //Global.prog.IsVisible = false;
            //Global.prog.IsIndeterminate = false;
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {          
            //NavigationService.Navigate(new Uri("/Activities.xaml?userID=113577538655080453733", UriKind.Relative));
            NavigationService.Navigate(new Uri("/Profile.xaml?userID=111399666922347055972", UriKind.Relative));
        }

        private void ApplicationBarMenuItem_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void signOutClick(object sender, EventArgs e)
        {
            dt.Stop();
            IsolatedStorageSettings.ApplicationSettings.Remove("username");
            IsolatedStorageSettings.ApplicationSettings.Remove("password");
            NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
            do
                NavigationService.RemoveBackEntry();
            while (NavigationService.CanGoBack != false);
            //Global.googLib = new GoogLib();
        }

        private void reportBugClick(object sender, EventArgs e)
        {
            dt.Stop();
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.To = "gtfo.productions.ltd@gmail.com";
            emailComposeTask.Show();
        }

        private void ApplicationBarMenuItem_Click_2(object sender, EventArgs e)
        {
            dt.Stop();
            NavigationService.Navigate(new Uri("/Search.xaml", UriKind.Relative));
        }

        private void yourProfileClick(object sender, EventArgs e)
        {
            dt.Stop();
            NavigationService.Navigate(new Uri("/Profile.xaml?userID=" + Global.googLib.info.userID, UriKind.Relative));
        }

        private void browserClick(object sender, EventArgs e)
        {
            dt.Stop();
            Global.webBrowser.Uri = new Uri("http://plus.google.com");
            Global.webBrowser.Show();
        }

        private async void refreshClick(object sender, EventArgs e)
        {
            loadingProgressBar.IsVisible = true;
            //Global.prog.IsVisible = true;
            //Global.prog.IsIndeterminate = true;
            posts = await Global.googLib.GetActivities(null, null, null);
            StreamListBox.ItemsSource = posts.posts;
            //Global.prog.IsVisible = false;
            //Global.prog.IsIndeterminate = false;
            loadingProgressBar.IsVisible = false;
        }

        private void newPostClick(object sender, EventArgs e)
        {
            dt.Stop();
            NavigationService.Navigate(new Uri("/NewPost.xaml", UriKind.Relative));
        }

        private void notificationClick(object sender, EventArgs e)
        {
            dt.Stop();
            NavigationService.Navigate(new Uri("/Notifications.xaml", UriKind.Relative));
        }

        private void showCommunityTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            dt.Stop();
            Community community = (sender as ListBox).SelectedItem as Community;
            NavigationService.Navigate(new Uri("/CommunityView.xaml?communityID=" + community.id, UriKind.Relative));
        }

        private void showCircleTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            dt.Stop();
            Circle circle = (sender as ListBox).SelectedItem as Circle;
            NavigationService.Navigate(new Uri("/Activities.xaml?circleID=" + circle.id, UriKind.Relative));
        }

        private async void ShowMoreTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Post post = (sender as ListBox).SelectedItem as Post;
            var newPosts = await Global.googLib.GetActivities(null, null, posts.pageToken);
            //MessageBox.Show((StreamListBox.ItemsSource as ObservableCollection<Post>).Count.ToString());
            foreach (var p in newPosts.posts)
                posts.posts.Add(p);
            posts.pageToken = newPosts.pageToken;
        }

    }
}