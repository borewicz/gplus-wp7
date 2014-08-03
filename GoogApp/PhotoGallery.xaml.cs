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
    public partial class PhotoGallery : PhoneApplicationPage
    {
        Post post;
        public PhotoGallery()
        {
            InitializeComponent();
            //RefreshPost();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            RefreshPost();
        }

        private async void RefreshPost()
        {
            string postID, userID;
            if (NavigationContext.QueryString.TryGetValue("postID", out postID) && NavigationContext.QueryString.TryGetValue("userID", out userID))
            {
                post = await Global.googLib.GetActivity(userID, postID);
                photosListBox.ItemsSource = post.photos;
                /*
                plusButton.Content = post.plusCount;
                reshareButton.Content = post.forwardCount;
                contentLabel.Text = post.content;
                authorLabel.Text = post.author;
                timeLabel.Text = post.time;
                avatarImage.Source = new BitmapImage(new Uri(post.avatar, UriKind.RelativeOrAbsolute));
                avatarImage.Tag = post.userID;
                
                if (post.media.thumbUrl != null)
                    thumbImage.Source = new BitmapImage(new Uri(post.media.thumbUrl, UriKind.RelativeOrAbsolute));
                titleLabel.Text = post.media.title;
                sourceLabel.Text = post.media.source;
                 */
                //this.PostsListBox.ItemsSource = Global.googLib.posts;
            }
        }

        private void StackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Photo.xaml?url=" + (string)(sender as StackPanel).Tag, UriKind.Relative));
        }
    }
}