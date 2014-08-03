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
using Microsoft.Phone.Tasks;

namespace GoogApp
{
    public partial class About : PhoneApplicationPage
    {
        public About()
        {
            InitializeComponent();
        }

        //GTFO 101767966522661730111
        //kredens 113577538655080453733
        //pawlowski 107224090973269463370
        //czech 115073485373959621378
        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Profile.xaml?userID=101767966522661730111", UriKind.Relative));
        }

        private void TextBlock_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Profile.xaml?userID=113577538655080453733", UriKind.Relative));
        }

        private void TextBlock_Tap_2(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Profile.xaml?userID=107224090973269463370", UriKind.Relative));
        }

        private void TextBlock_Tap_3(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Profile.xaml?userID=115073485373959621378", UriKind.Relative));
        }
    }
}