using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace GoogApp
{
    public partial class Profile : PhoneApplicationPage
    {
        string userID;
        bool navigated = false;
        public Profile()
        {
            InitializeComponent();
            //SystemTray.SetProgressIndicator(this, Global.prog);
        }

        public static string StripHTML(string HTMLText)
        {
            Regex reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
            return reg.Replace(HTMLText, "");
        }

        protected override async void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (navigated == true)
                return;
            navigated = true;
            base.OnNavigatedTo(e);
            userID = string.Empty;
            //Global.prog.IsVisible = true;
            //Global.prog.IsIndeterminate = true;
            if (NavigationContext.QueryString.TryGetValue("userID", out userID))
            {
                var profile = await Global.googLib.GetProfile(userID, false);
                ContentPanel.DataContext = profile;
                taglineLabel.Text = StripHTML(profile.tagline);
                if (profile.intro == "")
                    introStack.Visibility = System.Windows.Visibility.Collapsed;
                if (profile.tagline == "" || profile.tagline == null)
                    taglineStack.Visibility = System.Windows.Visibility.Collapsed;
                if (profile.braggingRights == "" || profile.braggingRights == null)
                    bragStack.Visibility = System.Windows.Visibility.Collapsed;
                if (profile.education.Count == 0)
                    eduStack.Visibility = System.Windows.Visibility.Collapsed;
                if (profile.occupation == "" || profile.occupation == null)
                    occStack.Visibility = System.Windows.Visibility.Collapsed;
                if (profile.skills == "" || profile.skills == null)
                    skillsStack.Visibility = System.Windows.Visibility.Collapsed;
                if (profile.employment.Count == 0)
                    employStack.Visibility = System.Windows.Visibility.Collapsed;
                if (profile.home == "" || profile.home == null)
                    homeStack.Visibility = System.Windows.Visibility.Collapsed;
                if (profile.olderHomes.Count == 0)
                    prevStack.Visibility = System.Windows.Visibility.Collapsed;
                if (profile.gender == Gender.Other || profile.gender == Gender.NoSpecified)
                    genStack.Visibility = System.Windows.Visibility.Collapsed;
                if (profile.lookingFor.Count == 0)
                    lookStack.Visibility = System.Windows.Visibility.Collapsed;
                if (profile.relationship == Relationship.DontSay)
                    relationStack.Visibility = System.Windows.Visibility.Collapsed;
                if (profile.nicks.Count == 0)
                    nicksStack.Visibility = System.Windows.Visibility.Collapsed;
                if (profile.otherProfiles.Count == 0)
                    otherStack.Visibility = System.Windows.Visibility.Collapsed;
                if (profile.contributeTo.Count == 0)
                    conStack.Visibility = System.Windows.Visibility.Collapsed;
                if (profile.links.Count == 0)
                    linksStack.Visibility = System.Windows.Visibility.Collapsed;

                //if (user.backgroundUrl != null)
                //    bigImage.Source = new BitmapImage(new Uri(user.backgroundUrl, UriKind.RelativeOrAbsolute));
                //if (user.avatarUrl != null)
                //    avatarImage.Source = new BitmapImage(new Uri(user.avatarUrl, UriKind.RelativeOrAbsolute));
                //nameLabel.Text = user.name;
                //descLabel.Text = user.intro;
                //posts = await Global.googLib.GetActivities(null, userID, null);
                //community = await Global.googLib.GetCommunity(communityID);
                //this.PostsListBox.ItemsSource = posts.posts;
            }
            //Global.prog.IsVisible = false;
            //Global.prog.IsIndeterminate = false;
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AddToCircles.xaml?userID=" + userID, UriKind.Relative));
        }

        private void postsClick(object sender, System.EventArgs e)
        {
        	NavigationService.Navigate(new Uri("/Activities.xaml?userID=" + userID, UriKind.Relative));
        }

        private void Grid_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Global.webBrowser.Uri = new Uri((string)(sender as Grid).Tag);
            Global.webBrowser.Show();
        }
    }
}