using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using GoogApp.Translations;
using System.Threading.Tasks;
using System.IO;

namespace GoogApp
{
    public partial class LoginPage : PhoneApplicationPage
    {
        string username, password;
        bool done;

        //string username, password;
        public LoginPage()
        {
            InitializeComponent();
            //SystemTray.SetProgressIndicator(this, Global.prog);
        }

        private async void login(string username, string password)
        {
            Global.googLib = new GoogLib();
            //progressBar.Visibility = System.Windows.Visibility.Visible;
            LoginPanel.Visibility = System.Windows.Visibility.Collapsed;
            downloadPath.Visibility = System.Windows.Visibility.Visible;
            TokenPanel.Visibility = System.Windows.Visibility.Collapsed;
            int result = await Global.googLib.Connect(username, password, null);
            //progressBar.Visibility = System.Windows.Visibility.Collapsed;
            switch (result)
            {
                case 0: IsolatedStorageSettings.ApplicationSettings["username"] = username;
                    IsolatedStorageSettings.ApplicationSettings["password"] = password;
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                    break;
                case 2: MessageBox.Show(AppResources.wrongCredentials, AppResources.faggot, MessageBoxButton.OK);
                    LoginPanel.Visibility = System.Windows.Visibility.Visible;
                    downloadPath.Visibility = System.Windows.Visibility.Collapsed;
                    TokenPanel.Visibility = System.Windows.Visibility.Collapsed;
                    IsolatedStorageSettings.ApplicationSettings.Remove("username");
                    IsolatedStorageSettings.ApplicationSettings.Remove("password");
                    break;
                case 1: MessageBox.Show(AppResources.loginError, AppResources.faggot, MessageBoxButton.OK);
                    LoginPanel.Visibility = System.Windows.Visibility.Visible;
                    downloadPath.Visibility = System.Windows.Visibility.Collapsed;
                    TokenPanel.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case 3:
                    //IsolatedStorageSettings.ApplicationSettings["username"] = username;
                    //IsolatedStorageSettings.ApplicationSettings["password"] = password;
                    LoginPanel.Visibility = System.Windows.Visibility.Collapsed;
                    downloadPath.Visibility = System.Windows.Visibility.Collapsed;
                    TokenPanel.Visibility = System.Windows.Visibility.Visible;
                    break;
                case 4:
                    MessageBox.Show("Go to https://accounts.google.com/LoginVerification to verify your account and try again.", "Login verification required", MessageBoxButton.OK);
                    LoginPanel.Visibility = System.Windows.Visibility.Visible;
                    downloadPath.Visibility = System.Windows.Visibility.Collapsed;
                    TokenPanel.Visibility = System.Windows.Visibility.Collapsed;
                    break;
            }
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (done == false)
            {
                done = true;
                do
                    NavigationService.RemoveBackEntry();
                while (NavigationService.CanGoBack != false);
                IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>("username", out username);
                IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>("password", out password);
                if (TokenPanel.Visibility != Visibility.Visible)
                {
                    if ((username != null) && (password != null))
                    {
                        //IsolatedStorageSettings.ApplicationSettings["refresh_token"]
                        login(username, password);
                    }
                    else
                    {
                        LoginPanel.Visibility = System.Windows.Visibility.Visible;
                        downloadPath.Visibility = System.Windows.Visibility.Collapsed;
                        TokenPanel.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
            }

        }

        private void Button_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if ((usernameBox.Text != "") && (passwordBox.Password != ""))
                login(usernameBox.Text, passwordBox.Password);
            else
                MessageBox.Show(AppResources.fullCredentials, AppResources.faggot, MessageBoxButton.OK);
        }

        private async void tokenButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            LoginPanel.Visibility = System.Windows.Visibility.Collapsed;
            downloadPath.Visibility = System.Windows.Visibility.Visible;
            TokenPanel.Visibility = System.Windows.Visibility.Collapsed;
            int result = await Global.googLib.loginUsingToken(tokenBox.Text);
            //progressBar.Visibility = System.Windows.Visibility.Collapsed;
            switch (result)
            {
                case 0: IsolatedStorageSettings.ApplicationSettings["username"] = usernameBox.Text;
                    IsolatedStorageSettings.ApplicationSettings["password"] = passwordBox.Password;
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                    break;
                case 2: MessageBox.Show(AppResources.wrongCredentials, AppResources.faggot, MessageBoxButton.OK);
                    LoginPanel.Visibility = System.Windows.Visibility.Visible;
                    downloadPath.Visibility = System.Windows.Visibility.Collapsed;
                    TokenPanel.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case 1: MessageBox.Show(AppResources.loginError, AppResources.faggot, MessageBoxButton.OK);
                    LoginPanel.Visibility = System.Windows.Visibility.Visible;
                    downloadPath.Visibility = System.Windows.Visibility.Collapsed;
                    TokenPanel.Visibility = System.Windows.Visibility.Collapsed;
                    break;
            }
        }


        private void downloadPath_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!LowMemoryHelper.IsLowMemDevice)
                ThemeManager.ToDarkTheme();
            else ThemeManager.ToLightTheme();
            Application.Current.Host.Settings.EnableFrameRateCounter = true;
        }
    }
}