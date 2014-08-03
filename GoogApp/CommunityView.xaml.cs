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
    public partial class CommunityView : PhoneApplicationPage
    {
        private Community community;

        public CommunityView()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e) 
        {
            base.OnNavigatedTo(e);
            string communityID = string.Empty;
            if (NavigationContext.QueryString.TryGetValue("communityID", out communityID))
            {
                community = await Global.googLib.GetCommunity(communityID);
                Community.Category category = new Community.Category();
                category.name = AppResources.all;
                category.id = null;
                community.categories.Insert(0, category);
                //posts = await Global.googLib.GetCommunityPosts (communityID, null, null);
                //this.PostsListBox.ItemsSource = posts.posts;
                this.CategoriesListBox.ItemsSource = community.categories;
            }
            /*
            this.PostsListBox.ItemsSource = (from post in Global.googLib.posts
                                             where post.userID == id
                                             select post);
             */
        }

        private void showCategoryTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Community.Category circle = (sender as ListBox).SelectedItem as Circle;
            Community.Category category = (sender as ListBox).SelectedItem as Community.Category;
            NavigationService.Navigate(new Uri("/Activities.xaml?communityID=" + community.id + "&categoryID=" + category.id, UriKind.Relative));
        }
    }
}