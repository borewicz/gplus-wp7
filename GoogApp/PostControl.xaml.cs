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
using System.Collections;
using System.Threading.Tasks;
using GoogApp.Translations;

namespace GoogApp
{
    public partial class PostControl : UserControl
    {
        //Post post;
        public PostControl()
        {
            InitializeComponent();
            //commentsGrid.Visibility = System.Windows.Visibility.Collapsed;
            DataContext = this;
            //eventGrid.Visibility = System.Windows.Visibility.Collapsed;
            //post = m
            //MessageBox.Show(post.author);
            //MessageBox.Show(((App)Application.Current).RootFrame.CurrentSource.ToString());
            //System.Diagnostics.Debug.WriteLine(((App)Application.Current).RootFrame.CurrentSource.OriginalString);
            //if (((App)Application.Current).RootFrame.CurrentSource.OriginalString.Contains("Activity"))
            //{
            //    userOptionsGrid.Visibility = Visibility.Visible;
            //}
        }

        /*
        public System.Collections.IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(System.Collections.IEnumerable), typeof(PostControl), new PropertyMetadata("", ItemsSourceChanged));

        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            dataContext.DataContext = (System.Collections.IEnumerable)e.NewValue;
        }

        */
        public Visibility CountVisible
        {
            get { return (Visibility)GetValue(CountVisibilityProperty); }
            set { SetValue(CountVisibilityProperty, value); }
        }

        public static readonly DependencyProperty CountVisibilityProperty = DependencyProperty.Register("CountVisible", typeof(Visibility), typeof(PostControl), null);

        public async void PlusOneTap(object sender, System.Windows.Input.GestureEventArgs e)
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

        public void ShowPostTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Post post = (sender as ListBox).SelectedItem as Post;
            Post post = (sender as Button).Tag as Post;
            //(Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Activity.xaml?postID=" + post.postID + "&userID=" + post.userID, UriKind.Relative));
        }

        public void ProfileTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Profile.xaml?userID=" + (string)(sender as Grid).Tag, UriKind.Relative));
        }

        public void ShareTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/NewPost.xaml?sharedPostID=" + (string)(sender as Button).Tag, UriKind.Relative));
        }

        public void GoLinkTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string uri = (string)(sender as Grid).Tag;
            Global.webBrowser.Uri = new Uri(uri);
            Global.webBrowser.Show();
        }

        private async void postDeleteTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string postID = (string)(sender as Button).Tag;
            CustomMessageBox messageBox = new CustomMessageBox()
            {
                Caption = AppResources.warning,
                Message = AppResources.doYouReallyWant, //"Do you really want to delete this activity?",
                //    "Tap Edit to continue.",
                LeftButtonContent = AppResources.yes,
                RightButtonContent = AppResources.cancel
                //IsFullScreen = (bool)FullScreenCheckBox.IsChecked
            };

            var result = await ShowAsync(messageBox);
            switch (result)
            {
                case CustomMessageBoxResult.LeftButton:
                    int res = await Global.googLib.RemoveActivity(postID);
                    if (res != 1)
                         (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                    break;
                case CustomMessageBoxResult.RightButton:
                case CustomMessageBoxResult.None:
                    break;
                default:
                    break;
            }
        }

        private async void postEditTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Post post = (Post)(sender as Button).Tag;
            TextBox box = new TextBox()
            {
                Text = post.editableContent,
                Margin = new Thickness(0, 14, 0, -2)
            };

            TiltEffect.SetIsTiltEnabled(box, true);

            CustomMessageBox messageBox = new CustomMessageBox()
            {
                Caption = AppResources.editPost,
                //Message =
                //    "Tap Edit to continue.",
                Content = box,
                LeftButtonContent = "ok",
                RightButtonContent = AppResources.cancel
                //IsFullScreen = (bool)FullScreenCheckBox.IsChecked
            };

            var result = await ShowAsync(messageBox);
            switch (result)
            {
                case CustomMessageBoxResult.LeftButton:
                    string res = await Global.googLib.EditPost(box.Text, post.postID);
                    if (res != null)
                        //RefreshPost();
                        //MessageBox.Show("Super!");
                        contentTextBlock.Text = res;

                    break;
                case CustomMessageBoxResult.RightButton:
                case CustomMessageBoxResult.None:
                    break;
                default:
                    break;
            }
        }

        public static async Task<CustomMessageBoxResult> ShowAsync(CustomMessageBox box)
        {
            var taskCompletionSource = new TaskCompletionSource<CustomMessageBoxResult>();
            var result = CustomMessageBoxResult.None;

            //Only store the result here.
            box.Dismissed += (a, b) => result = b.Result;

            //Use this event to set the result.
            box.Unloaded += (_, __) => taskCompletionSource.TrySetResult(result);

            try { box.Show(); }
            catch (Exception exception) { taskCompletionSource.TrySetException(exception); }

            return await taskCompletionSource.Task;
        }

        private void Image_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Post post = (sender as Image).Tag as Post;
            if ((post.media.url != null) && (post.media.url.Contains("youtube.com")))
            {
                Global.webBrowser.Uri = new Uri(post.media.url);
                Global.webBrowser.Show();
            }
            else if (post.photos.Count > 0)
                (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/PhotoGallery.xaml?postID=" + post.postID + "&userID=" + post.userID, UriKind.Relative));
            else
                (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/Photo.xaml?url=" + post.photo, UriKind.Relative));



        }

        private void StackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/EventPage.xaml?eventID=" + (string)(sender as StackPanel).Tag, UriKind.Relative));
        }


    }
}
