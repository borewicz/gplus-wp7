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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using GoogApp.Translations;
using Microsoft.Phone.Marketplace;
using Microsoft.Phone.Info;

namespace GoogApp
{
    public static class Global
    {
        public static GoogLib googLib;
        public static WebBrowserTask webBrowser;
        public static SearchQuery query;
        //public static ProgressIndicator prog;
    }

    public static class LowMemoryHelper
    {
        public static bool IsLowMemDevice { get; set; }

        static LowMemoryHelper()
        {
            try
            {
                Int64 result = (Int64)DeviceExtendedProperties.GetValue("ApplicationWorkingSetLimit");
                if (result < 94371840L)
                    IsLowMemDevice = true;
                else
                    IsLowMemDevice = false;
            }
            catch (ArgumentOutOfRangeException)
            {
                // Windows Phone OS update not installed, which indicates a 512-MB device. 
                IsLowMemDevice = false;
            }
        }
    }

    public partial class App : Application
    {
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        private static LicenseInformation _licenseInfo = new LicenseInformation();
        private static bool _isTrial = true;
        public bool IsTrial
        {
            get
            {
                return _isTrial;
            }
        }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();
            if (!LowMemoryHelper.IsLowMemDevice)
                ThemeManager.ToLightTheme();
            Global.webBrowser = new WebBrowserTask();
            //Global.prog = new ProgressIndicator();
            ////Global.prog.IsIndeterminate = true;
            Global.query = new SearchQuery();
            Global.query.type = SearchType.Everything;
            Global.query.category = SearchCategory.Recent;
            Global.query.privacy = SearchPrivacy.Everyone;

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters
                //Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
        }

        /// <summary>
        /// Check the current license information for this application
        /// </summary>
        private void CheckLicense()
        {
            // When debugging, we want to simulate a trial mode experience. The following conditional allows us to set the _isTrial 
            // property to simulate trial mode being on or off. 
#if DEBUG
            string message = "This sample demonstrates the implementation of a trial mode in an application." +
                               "Press 'OK' to simulate trial mode. Press 'Cancel' to run the application in normal mode.";
            if (MessageBox.Show(message, "Debug Trial",
                 MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                _isTrial = true;
            }
            else
            {
                _isTrial = false;
            }
#else
            _isTrial = _licenseInfo.IsTrial();
#endif
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            CheckLicense();
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            //// Ensure that application state is restored appropriately
            //if (!App.ViewModel.IsDataLoaded)
            //{
            //    App.ViewModel.LoadData();
            //}
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            CheckLicense();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            // Ensure that required application state is persisted here.
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is QuitException)
                return;

            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }

            string newText = null;
            //List<String> newLines = new List<string>();
            List<string> lines = e.ExceptionObject.StackTrace.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Contains("GoogApp"))
                    newText += lines[i] + "\n";
            }
            //lines.ForEach(str => newText += str + Environment.NewLine);
            newText = newText.Replace(" ", "").Replace("at","").Replace("\t", "");
            MessageBox.Show(AppResources.unexpected + "\n\n" + newText, AppResources.nooo, MessageBoxButton.OK);

            //MessageBox.Show(e.ExceptionObject.ToString(), "Unexpected error", MessageBoxButton.OK);

            //var errorWin = new ErrorWindow(e.ExceptionObject, "An unexpected error has occurred. Please click Send to report the error details.") { Title = "Unexpected Error" };
            /*
            var errorWin = MessageBox.Show("An unexpected error has occurred. Tap OK to send report to developer OR cancel to return to application. Sorry.", "Nooooooo!", MessageBoxButton.OKCancel);
            if (errorWin == MessageBoxResult.OK)
            {
                EmailComposeTask emailComposeTask = new EmailComposeTask();
                emailComposeTask.Subject = "[gPlus error report]";
                emailComposeTask.Body = "===ERROR REPORT BEGIN====" + e.ExceptionObject.ToString() + "===ERROR REPORT END===";
                emailComposeTask.To = " gtfo.productions@outlook.com";

                emailComposeTask.Show();
            }
             */
            //((PhoneApplicationFrame) RootVisual).Source = new Uri("/Views/ErrorWindow.xaml", UriKind.Relative);

            e.Handled = true;
        }

        private class QuitException : Exception { }

        public static void Quit()
        {
            throw new QuitException();
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}