using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace GoogApp
{
    public static class PostSupport
    {
        public static async void PlusOneTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string count = (string)(sender as Button).Content;
            //Post post = (from p in (StreamListBox.Items as List<Post>)
            //             where p.postID == (string)(sender as Button).Tag
            //             select p).FirstOrDefault();
            //int result = await Global.googLib.PlusOne(post.postID, !(post.youPlused));
            //if (result == 0)
            //{
            //    SolidColorBrush brush = (sender as Button).Background as SolidColorBrush;
            //    if (brush != null) {
            //        if (brush.Color == Colors.Red)
            //            brush.Color = Colors.Gray;
            //        else brush.Color = Colors.Red;
            //    }
            //    (sender as Button).Background = brush;
            //}
            SolidColorBrush brush = (sender as Button).Foreground as SolidColorBrush;
            if (brush != null)
            {
                if (brush.Color == Colors.Red)
                {
                    int result = await Global.googLib.PlusOne((string)(sender as Button).Tag, false, "buzz");
                    if (result == 0)
                    {
                        //brush.Color = Colors.White;
                        brush.Color = (App.Current.Resources["PhoneForegroundBrush"] as SolidColorBrush).Color;
                        (sender as Button).Content = "+" + (Convert.ToInt32(count.Remove(0, 1)) - 1).ToString();
                        //(sender as Button).Content = (int)(sender as Button).Content - 1;
                    }
                }
                else
                {
                    int result = await Global.googLib.PlusOne((string)(sender as Button).Tag, true, "buzz");
                    if (result == 0)
                    {
                        brush.Color = Colors.Red;
                        //(sender as Button).Content = (int)(sender as Button).Content + 1;
                        (sender as Button).Content = "+" + (Convert.ToInt32(count.Remove(0, 1)) + 1).ToString();
                    }
                }
            }
            (sender as Button).Foreground = brush;
            //(sender as Button).Content = "+2";
            //(sender as ListBoxItem).Content = "XD";
            //MessageBox.Show((string)(sender as Button).Tag);
        }

        public static void ShowPostTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Post post = (sender as ListBox).SelectedItem as Post;
            Post post = (sender as Button).Tag as Post;
            //(Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Activity.xaml?postID=" + post.postID + "&userID=" + post.userID, UriKind.Relative));
        }

        public static void ProfileTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Profile.xaml?userID=" + (string)(sender as Grid).Tag, UriKind.Relative));
        }

        public static void ShareTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/NewPost.xaml?sharedPostID=" + (string)(sender as Button).Tag, UriKind.Relative));
        }

        public static void GoLinkTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string uri = (string)(sender as Grid).Tag;
            Global.webBrowser.Uri = new Uri(uri);
            Global.webBrowser.Show();
        }
    }
}