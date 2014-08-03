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
    public partial class SearchResult : PhoneApplicationPage
    {
        SearchUsers users;
        SearchCommunities communities;
        string query;
        bool navigated = false;

        public SearchResult()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (navigated == true)
                return;
            navigated = true;
            RefreshPost();
        }

        private async void RefreshPost()
        {
            if (NavigationContext.QueryString.TryGetValue("query", out query))
            {
                if (Global.query.type == SearchType.PeoplePages)
                {
                    users = await Global.googLib.QueryUser(query, null);
                    ResultListBox.ItemsSource = users.users;
                }
                else if (Global.query.type == SearchType.Sparks)
                {
                    communities = await Global.googLib.QueryCommunity(query, null);
                    ResultListBox.ItemsSource = communities.communities;
                }
            }
        }

        private void ResultListBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (Global.query.type == SearchType.PeoplePages)
            {
                //Post post = (sender as Button).Tag as Post;
                UserInfo info = (sender as Grid).Tag as UserInfo;
                NavigationService.Navigate(new Uri("/Profile.xaml?userID=" + info.userID, UriKind.Relative));

            }
            else if (Global.query.type == SearchType.Sparks)
            {
                Community community = (sender as Grid).Tag as Community;
                NavigationService.Navigate(new Uri("/CommunityView.xaml?communityID=" + community.id, UriKind.Relative));
            }
        }

        private async void ShowMoreTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (Global.query.type == SearchType.PeoplePages)
            {
                var newUsers = await Global.googLib.QueryUser(query, users.pageToken);
                foreach (var n in newUsers.users)
                    users.users.Add(n);
                users.pageToken = newUsers.pageToken;
            }
            else if (Global.query.type == SearchType.Sparks)
            {
                var newCommunities = await Global.googLib.QueryCommunity(query, communities.pageToken);
                foreach (var n in newCommunities.communities)
                    communities.communities.Add(n);
                communities.pageToken = newCommunities.pageToken;
            }
        }
    }
}