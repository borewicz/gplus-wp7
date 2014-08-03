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
using Microsoft.Phone.Shell;

namespace GoogApp
{
    public partial class ActivitiesPage : PhoneApplicationPage
    {
        Posts posts;
        string circleID, communityID, categoryID, userID, query;
        bool navigated = false;
        public ActivitiesPage()
        {
            InitializeComponent();
            //SystemTray.SetProgressIndicator(this, Global.prog);
        }

        protected override async void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (navigated == true)
                return;
            navigated = true;
            //Global.prog.IsVisible = true;
            //Global.prog.IsIndeterminate = true;
            base.OnNavigatedTo(e);
            if ((Application.Current as App).IsTrial)
                adStackPanel.Visibility = System.Windows.Visibility.Visible;
            else
                adStackPanel.Visibility = System.Windows.Visibility.Collapsed;
            if (posts == null)
            {
                //string circleID, userID;
                if (NavigationContext.QueryString.TryGetValue("circleID", out circleID))
                {
                    posts = await Global.googLib.GetActivities(circleID, null, null);
                    this.PostsListBox.ItemsSource = posts.posts;

                }
                else if (NavigationContext.QueryString.TryGetValue("communityID", out communityID))
                {
                    categoryID = NavigationContext.QueryString["categoryID"].ToString();
                    if (categoryID == "") categoryID = null;
                    //    posts = await Global.googLib.GetCommunityPosts(communityID, null, null);
                    //else
                    posts = await Global.googLib.GetCommunityPosts(communityID, categoryID, null);
                    this.PostsListBox.ItemsSource = posts.posts;

                }
                else if (NavigationContext.QueryString.TryGetValue("userID", out userID))
                {
                    posts = await Global.googLib.GetActivities(null, userID, null);
                    this.PostsListBox.ItemsSource = posts.posts;

                }
                else if (NavigationContext.QueryString.TryGetValue("query", out query))
                {
                    posts = await Global.googLib.QueryPost(query, Global.query, null, null);
                    this.PostsListBox.ItemsSource = posts.posts;

                }
                //Global.prog.IsVisible = false;
                //Global.prog.IsIndeterminate = false;
            }
            /*
            this.PostsListBox.ItemsSource = (from post in Global.googLib.posts
                                             where post.userID == id
                                             select post);
             */
            loadingProgressBar.IsVisible = false;
        }

        private async void ShowMoreTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Posts newPosts;
            if (circleID != null)
                //Post post = (sender as ListBox).SelectedItem as Post;
                newPosts = await Global.googLib.GetActivities(circleID, null, posts.pageToken);
            else if (userID != null)
                newPosts = await Global.googLib.GetActivities(null, userID, posts.pageToken);
            else if (query != null)
                newPosts = await Global.googLib.QueryPost(query, Global.query, posts.pageToken, posts.longPageToken);
            else
                newPosts = await Global.googLib.GetCommunityPosts(communityID, categoryID, posts.pageToken);
            //MessageBox.Show((StreamListBox.ItemsSource as ObservableCollection<Post>).Count.ToString());
            foreach (var p in newPosts.posts)
                posts.posts.Add(p);
            posts.pageToken = newPosts.pageToken;
        }
    }
}