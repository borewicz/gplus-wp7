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
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using Microsoft.Phone.Shell;
using GoogApp.Translations;

namespace GoogApp
{
    public partial class Activity : PhoneApplicationPage
    {
        Post post;
        bool navigated = false;
        public Activity()
        {
            InitializeComponent();
            //SystemTray.SetProgressIndicator(this, Global.prog);
        }

        private async void RefreshPost()
        {
            //Global.prog.IsVisible = true;
            //Global.prog.IsIndeterminate = true;
            string postID, userID, posted;
            if ((Application.Current as App).IsTrial)
                adStackPanel.Visibility = System.Windows.Visibility.Visible;
            else
                adStackPanel.Visibility = System.Windows.Visibility.Collapsed;
            if (NavigationContext.QueryString.TryGetValue("posted", out posted))
                if (posted == "true")
                    NavigationService.RemoveBackEntry();
            if (NavigationContext.QueryString.TryGetValue("postID", out postID) && NavigationContext.QueryString.TryGetValue("userID", out userID))
            {
                post = await Global.googLib.GetActivity(userID, postID);
                stackPanel.DataContext = post;
                commentsBox.ItemsSource = post.comments;
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
            //Global.prog.IsVisible = false;
            //Global.prog.IsIndeterminate = false;
            loadingProgressBar.IsVisible = false;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (navigated == false)
            {
                RefreshPost();
                navigated = true;
            }
        }

        private async void Button_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            int result = await Global.googLib.AddComment(commentTextBox.Text, post.postID);
            if (result != 1)
            {
                RefreshPost();
                commentTextBox.Text = "";
            }
        }

        private async void PlusOneCommentTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string count = (string)(sender as Button).Content;
            SolidColorBrush brush = (sender as Button).Foreground as SolidColorBrush;
            if (brush != null)
            {
                if (brush.Color == Colors.Red)
                {
                    int result = await Global.googLib.PlusOne((string)(sender as Button).Tag, false, "comment");
                    if (result == 0)
                    {
                        brush.Color = (App.Current.Resources["PhoneForegroundBrush"] as SolidColorBrush).Color;
                        (sender as Button).Content = "+" + (Convert.ToInt32(count.Remove(0, 1)) - 1).ToString();
                    }
                }
                else
                {
                    int result = await Global.googLib.PlusOne((string)(sender as Button).Tag, true, "comment");
                    if (result == 0)
                    {
                        brush.Color = Colors.Red;
                        (sender as Button).Content = "+" + (Convert.ToInt32(count.Remove(0, 1)) + 1).ToString();
                    }
                }
            }
            (sender as Button).Foreground = brush;
        }

        private async void MenuItem_Click_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //string commentID = (commentsBox.SelectedItem as Post.Comment).commentID;
            string index = (string)(sender as Button).Tag;
            CustomMessageBox messageBox = new CustomMessageBox()
            {
                Message = AppResources.doYouReallyWant,
                Caption = AppResources.warning,
                LeftButtonContent = AppResources.yes,
                RightButtonContent = AppResources.cancel
                //IsFullScreen = (bool)FullScreenCheckBox.IsChecked
            };

            var result = await ShowAsync(messageBox);
            switch (result)
            {
                case CustomMessageBoxResult.LeftButton:
                    int res = await Global.googLib.RemoveComment(index);
                    if (res != 1)
                    {
                        Post.Comment comment = (from p in post.comments
                                                where p.commentID == index
                                                select p).FirstOrDefault();
                        post.comments.Remove(comment);
                        commentsBox.ItemsSource = null;
                        commentsBox.ItemsSource = post.comments;
                    }
                    break;
                case CustomMessageBoxResult.RightButton:
                case CustomMessageBoxResult.None:
                    break;
                default:
                    break;
            }
        }

        private void replyTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            commentTextBox.Text += "@" + (string)(sender as Button).Tag + " ";
        }


        private async void MenuItem_Click_2(object sender, System.Windows.Input.GestureEventArgs e)
        {
            InputScope scope = new InputScope();
            InputScopeName name = new InputScopeName();
            name.NameValue = InputScopeNameValue.Text;
            scope.Names.Add(name);
            string commentID = (string)(sender as Button).Tag;
            TextBox box = new TextBox()
            {
                Text = (from c in post.comments
                        where c.commentID == commentID
                        select c).First().content,
                Margin = new Thickness(0, 14, 0, -2),
                InputScope = scope
            };

            TiltEffect.SetIsTiltEnabled(box, true);

            CustomMessageBox messageBox = new CustomMessageBox()
            {
                Caption = AppResources.editComment,
                Content = box,
                LeftButtonContent = "ok",
                RightButtonContent = AppResources.cancel
                //IsFullScreen = (bool)FullScreenCheckBox.IsChecked
            };

            var result = await ShowAsync(messageBox);
            switch (result)
            {
                case CustomMessageBoxResult.LeftButton:
                    int res = await Global.googLib.EditComment(commentID, box.Text);
                    if (res != 1)
                        RefreshPost();
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
    }
}