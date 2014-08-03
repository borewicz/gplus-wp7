using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Threading.Tasks;
using Microsoft.Phone.Shell;
using GoogApp.Translations;
using System.Collections;
using System.Linq;
using System.Windows.Input;

namespace GoogApp
{
    public partial class NewPost : PhoneApplicationPage
    {
        private CameraCaptureTask ctask = new CameraCaptureTask();
        private PhotoChooserTask ptask = new PhotoChooserTask();
        JArray media = null;
        string sharedPostId = null;
        List<Community> communities;
        ObservableCollection<AclItem> aclItems = new ObservableCollection<AclItem>();
        //Community community;
        bool navigated = false;
        public NewPost()
        {
            InitializeComponent();
            toggle.Checked += new EventHandler<RoutedEventArgs>(toggle_Checked);
            toggle.Unchecked += new EventHandler<RoutedEventArgs>(toggle_Unchecked);
            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = AppResources.git;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).Text = AppResources.photo;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).Text = AppResources.camera;
            ((ApplicationBarIconButton)ApplicationBar.Buttons[3]).Text = AppResources.link;
            listPicker.SummaryForSelectedItemsDelegate = (IList items) =>
            {
                if (items == null || items.Count == 0) return string.Empty;
                return String.Join(", ", ((IEnumerable<object>)items)
                    .Select(item => (item as AclItem).name));
            };
        }

        private void toggle_Unchecked(object sender, RoutedEventArgs e)
        {
            listPicker.ItemsSource = aclItems;
            listPicker.SelectionMode = System.Windows.Controls.SelectionMode.Multiple; 
            ///toggle.Content = "ToggleSwitch is off";
            categoriesListPicker.Visibility = System.Windows.Visibility.Collapsed;
            //toggle.SwitchForeground = new SolidColorBrush(Colors.Red);
        }

        private async void toggle_Checked(object sender, RoutedEventArgs e)
        {
            //toggle.Content = "ToggleSwitch is on";
            //toggle.SwitchForeground = new SolidColorBrush(Colors.Black);
            //var communities = await Global.googLib.GetYourCommunities();
            categoriesListPicker.Visibility = System.Windows.Visibility.Visible;
            listPicker.ItemsSource = communities;
            categoriesListPicker.ItemsSource = (await Global.googLib.GetCommunity(communities[0].id)).categories;
            listPicker.SelectionMode = System.Windows.Controls.SelectionMode.Single;
        }

        protected override async void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (navigated == true)
                return;
            navigated = true;
            NavigationContext.QueryString.TryGetValue("sharedPostID", out sharedPostId);
            if (sharedPostId != null)
            {
                titleBlock.Text = AppResources.reshare;
                ((ApplicationBarIconButton)ApplicationBar.Buttons[1]).IsEnabled = false;
                ((ApplicationBarIconButton)ApplicationBar.Buttons[2]).IsEnabled = false;
                ((ApplicationBarIconButton)ApplicationBar.Buttons[3]).IsEnabled = false;
            }

            if (listPicker.ItemsSource == null)
            {
                aclItems = await Global.googLib.GetAclItems();
                communities = Global.googLib.communities;
                if (communities.Count < 1)
                    toggle.IsEnabled = false;
                listPicker.ItemsSource = aclItems;
                //if (NavigationContext.QueryString.TryGetValue("communityID", out communityID))
                //{
                //    Community community = await Global.googLib.GetCommunity(communityID);
                //    listPicker.ItemsSource = community.categories;
                //    listPicker.SelectionMode = SelectionMode.Single;
                //}   
                //else
            }
            ctask.Completed += new EventHandler<PhotoResult>(PhotoTask_Completed);
            ptask.Completed += new EventHandler<PhotoResult>(PhotoTask_Completed);
            //SystemTray.SetProgressIndicator(this, Global.prog); 
        }

        private async void gitClick(object sender, EventArgs e)
        {
            loadingProgressBar.IsVisible = true;
            loadingProgressBar.Text = AppResources.posting;
            string id;
            List<AclItem> items = new List<AclItem>();
            if (listPicker.SelectedItems != null)
                //if (community == null)
                    foreach (var item in listPicker.SelectedItems)
                        items.Add((item as AclItem));
            //if (sharedPostId != null)
            if (toggle.IsChecked == true)
            {
                string communityID = (listPicker.SelectedItem as Community).id;
                string communityCategory = (categoriesListPicker.SelectedItem as Community.Category).id;
                Tuple<string, string> tuple = new Tuple<string, string>(communityID, communityCategory);
                //id = await Global.googLib
                id = await Global.googLib.NewPost(contentTextBox.Text, items, sharedPostId, media, tuple);
            }
            else
                id = await Global.googLib.NewPost(contentTextBox.Text, items, sharedPostId, media, null);
                if (id != null)
                    NavigationService.Navigate(new Uri("/Activity.xaml?posted=true&postID=" + id + "&userID=" + Global.googLib.info.userID, UriKind.Relative));
            loadingProgressBar.IsVisible = false;
            //else
            //    id = await Global.googLib.NewPost(contentTextBox.Text, items, null);
            //NavigationService.Navigate("/Actitity.xaml?postID=" + id ...
            //NavigationService.Remove(...
        }

        private async void PhotoTask_Completed(object sender, PhotoResult e)
        {
            loadingProgressBar.IsVisible = true;
            loadingProgressBar.Text = AppResources.uploading;
            if (e.TaskResult == TaskResult.OK)
            {
                //Global.prog.IsVisible = true;
                //Global.prog.IsIndeterminate = true;
                media = await Global.googLib.UploadPhoto(e.ChosenPhoto, e.OriginalFileName);
                //media = await Global.googLib.GetLinkDetails("http://www.google.pl");
                //PostUtility.uploadPhoto((listPicker.SelectedItem as string), e.ChosenPhoto);
                //Code to display the photo on the page in an image control named myImage.
                //myImage.Source = bmp;
                //previewGrid.DataContext = media;
                //ost.Media tmpMedia = new Post.Media();
                tmbImage.Source = new BitmapImage(new Uri("https:" + media[6]["27639957"][6].ToString(), UriKind.Absolute));
                titleLabel.Text = null;
                sourceLabel.Text = null;
                clearButton.Visibility = System.Windows.Visibility.Visible;
                //Global.prog.IsVisible = false;
                //Global.prog.IsIndeterminate = false;
                //previewGrid.DataContext = tmpMedia;
            }
            loadingProgressBar.IsVisible = false;
        }

        private void photoClick(object sender, System.EventArgs e)
        {
            ptask.Show();
            //Global.googLib.UploadPhoto(
        }

        private void cameraClick(object sender, System.EventArgs e)
        {
            ctask.Show();
        }

        //private async void linkClick(object sender, System.EventArgs e)
        //{
        //    media = await Global.googLib.GetLinkDetails("http://www.ubuntu-pomoc.org/ubuntu-13-10-instalacja-serwera-wyswietlania-mir/");
        //    tmbImage.Source = new BitmapImage(new Uri("https:" + media[6]["39748951"][5][0].ToString(), UriKind.Absolute));
        //    titleLabel.Text = media[6]["39748951"][2].ToString();
        //    sourceLabel.Text = media[6]["39748951"][0].ToString();
        //}

        private async void listPicker_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (toggle.IsChecked == true)
            {
                categoriesListPicker.ItemsSource = (await Global.googLib.GetCommunity(communities[listPicker.SelectedIndex].id)).categories;
            }
        }

        private async void linkClick(object sender, System.EventArgs e)
        {
            InputScope scope = new InputScope();
            InputScopeName name = new InputScopeName();
            name.NameValue = InputScopeNameValue.Url;
            scope.Names.Add(name);
            TextBox box = new TextBox()
            {
                //Margin = new Thickness(0, 14, 0, -2)
                InputScope = scope,
                Text = "http://"
            };

            TiltEffect.SetIsTiltEnabled(box, true);

            CustomMessageBox messageBox = new CustomMessageBox()
            {
                Caption = "Link",
                Message =
                    "Tap OK to continue.",
                Content = box,
                LeftButtonContent = "ok",
                RightButtonContent = "cancel",
                //IsFullScreen = (bool)FullScreenCheckBox.IsChecked
            };

            var result = await ShowAsync(messageBox);
            loadingProgressBar.IsVisible = true;
            loadingProgressBar.Text = AppResources.loading;
            switch (result)
            {
                case CustomMessageBoxResult.LeftButton:
                    //Global.prog.IsVisible = true;
                    //Global.prog.IsIndeterminate = true;
                    media = await Global.googLib.GetLinkDetails(box.Text);
                    if (media != null)
                    {
                        try
                        {
                            System.Diagnostics.Debug.WriteLine(media[7].First.First[5][0].ToString());
                            tmbImage.Source = new BitmapImage(new Uri("https:" + media[7].First.First[5][0].ToString(), UriKind.Absolute));
                        }
                        catch { }
                        titleLabel.Text = media[7].First.First[2].ToString();
                        sourceLabel.Text = media[7].First.First[0].ToString();
                        clearButton.Visibility = System.Windows.Visibility.Visible;
                        //Global.prog.IsVisible = false;
                        //Global.prog.IsIndeterminate = false;
                    }
                    break;
                case CustomMessageBoxResult.RightButton:
                case CustomMessageBoxResult.None:
                    break;
                default:
                    break;
            }
            loadingProgressBar.IsVisible = false;
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

        private void Button_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            media = null;
            titleLabel.Text = null;
            sourceLabel.Text = null;
            tmbImage.Source = null;
            (sender as Button).Visibility = System.Windows.Visibility.Collapsed;
        }


    }
}