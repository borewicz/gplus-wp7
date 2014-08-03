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
using System.Windows.Input;
using Microsoft.Xna.Framework.Media;
using System.Windows.Resources;
using System.IO.IsolatedStorage;
using System.IO;
using GoogApp.Translations;

namespace GoogApp
{
    public partial class Photo : PhoneApplicationPage
    {
        string url;

        public Photo()
        {
            InitializeComponent();
            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = AppResources.save;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (NavigationContext.QueryString.TryGetValue("url", out url))
            {
                photo.Source = new BitmapImage(new Uri(url, UriKind.RelativeOrAbsolute));
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

        private Point Center;
        private double InitialScale;

        private void GestureListener_PinchStarted(object sender, PinchStartedGestureEventArgs e)
        {
            // Store the initial rotation angle and scaling
            InitialScale = ImageTransformation.ScaleX;
            // Calculate the center for the zooming
            Point firstTouch = e.GetPosition(photo, 0);
            Point secondTouch = e.GetPosition(photo, 1);

            Center = new Point(firstTouch.X + (secondTouch.X - firstTouch.X) / 2.0, firstTouch.Y + (secondTouch.Y - firstTouch.Y) / 2.0);
        }

        private void OnPinchDelta(object sender, PinchGestureEventArgs e)
        {
            // If its less that the original  size or more than 4x then don’t apply
            if (InitialScale * e.DistanceRatio > 4 || (InitialScale != 1 && e.DistanceRatio == 1) || InitialScale * e.DistanceRatio < 1)
                return;

            // If its original size then center it back
            if (e.DistanceRatio <= 1.08)
            {
                ImageTransformation.CenterY = 0;
                ImageTransformation.CenterY = 0;
                ImageTransformation.TranslateX = 0;
                ImageTransformation.TranslateY = 0;
            }

            ImageTransformation.CenterX = Center.X;
            ImageTransformation.CenterY = Center.Y;

            // Update the rotation and scaling
            if (this.Orientation == PageOrientation.Landscape)
            {
                // When in landscape we need to zoom faster, if not it looks choppy
                ImageTransformation.ScaleX = InitialScale * (1 + (e.DistanceRatio - 1) * 2);
            }
            else
            {
                ImageTransformation.ScaleX = InitialScale * e.DistanceRatio;
            }
            ImageTransformation.ScaleY = ImageTransformation.ScaleX;
        }

        private void Image_DragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            // if is not touch enabled or the scale is different than 1 then don’t allow moving
            if (ImageTransformation.ScaleX <= 1.1)
                return;

            double centerX = ImageTransformation.CenterX;
            double centerY = ImageTransformation.CenterY;
            double translateX = ImageTransformation.TranslateX;
            double translateY = ImageTransformation.TranslateY;
            double scale = ImageTransformation.ScaleX;
            double width = photo.ActualWidth;
            double height = photo.ActualHeight;

            // Verify limits to not allow the image to get out of area
            if (centerX - scale * centerX + translateX + e.HorizontalChange < 0 && centerX + scale * (width - centerX) + translateX + e.HorizontalChange > width)
            {
                ImageTransformation.TranslateX += e.HorizontalChange;
            }

            if (centerY - scale * centerY + translateY + e.VerticalChange < 0 && centerY + scale * (height - centerY) + translateY + e.VerticalChange > height)
            {
                ImageTransformation.TranslateY += e.VerticalChange;
            }

            return;
        }

        private void ApplicationBarMenuItem_Click(object sender, EventArgs e)
        {
            var webClient = new WebClient();
            webClient.OpenReadCompleted += WebClientOpenReadCompleted;
            webClient.OpenReadAsync(new Uri(url, UriKind.Absolute));
        }

        void WebClientOpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            const string tempJpeg = "TempJPEG";
            var streamResourceInfo = new StreamResourceInfo(e.Result, null);
            var userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();
            if (userStoreForApplication.FileExists(tempJpeg))
                userStoreForApplication.DeleteFile(tempJpeg);          
            var isolatedStorageFileStream = userStoreForApplication.CreateFile(tempJpeg);
            var bitmapImage = new BitmapImage { CreateOptions = BitmapCreateOptions.None };
            bitmapImage.SetSource(streamResourceInfo.Stream);
            var writeableBitmap = new WriteableBitmap(bitmapImage);
            writeableBitmap.SaveJpeg(isolatedStorageFileStream, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight, 0, 85);
            isolatedStorageFileStream.Close();
            isolatedStorageFileStream = userStoreForApplication.OpenFile(tempJpeg, FileMode.Open, FileAccess.Read);
           // Save the image to the camera roll or saved pictures album.
            var mediaLibrary = new MediaLibrary();
            // Save the image to the saved pictures album.
            mediaLibrary.SavePicture(string.Format(Path.GetFileName(url), DateTime.Now), isolatedStorageFileStream);
            isolatedStorageFileStream.Close();

        }
    }
}