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
using GoogApp.Translations;

namespace GoogApp
{
    public class Enum<T>
    {
        public static IEnumerable<string> GetNames()
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException("Type '" + type.Name + "' is not an enum");

            return (
              from field in type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
              where field.IsLiteral
              select field.Name).ToList<string>();
        }
    }

    public class RepairedComboBox : ComboBox
    {
        public override string ToString()
        {
            return string.Empty;
        }
    }

    public partial class Search : PhoneApplicationPage
    {
        public Search()
        {
            InitializeComponent();
            ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).Text = AppResources.search;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //everyoneRadioButton.IsChecked = true;
            //bestRadioButton.IsChecked = true;
            //typePicker.ItemsSource = Enum<SearchType>.GetNames();
        }

        private void postsClick(object sender, System.EventArgs e)
        {
            if (postsRadioButton.IsChecked == true)
                Global.query.type = SearchType.Posts;
            else if (eventsRadioButton.IsChecked == true)
                Global.query.type = SearchType.Events;
            else if (usersRadioButton.IsChecked == true)
                Global.query.type = SearchType.PeoplePages;
            else if (communitiesRadioButton.IsChecked == true)
                Global.query.type = SearchType.Sparks;

            if (bestRadioButton.IsChecked == true)
                Global.query.category = SearchCategory.Best;
            else if (recentRadioButton.IsChecked == true)
                Global.query.category = SearchCategory.Recent;
            if (everyoneRadioButton.IsChecked == true)

                Global.query.privacy = SearchPrivacy.Everyone;
            else if (circlesRadioButton.IsChecked == true)
                Global.query.privacy = SearchPrivacy.Circles;
            else if (youRadioButton.IsChecked == true)
                Global.query.privacy = SearchPrivacy.You;
            //SearchType type = (SearchType)Enum.Parse(typeof(SearchType), (string)typePicker.SelectedItem, false);
            //Global.query.type = (SearchType)typePicker.SelectedItem;
            //Global.query.type = type;
            if ((Global.query.type == SearchType.PeoplePages) || (Global.query.type == SearchType.Sparks))
                NavigationService.Navigate(new Uri("/SearchResult.xaml?query=" + queryText.Text, UriKind.Relative));
            else
                NavigationService.Navigate(new Uri("/Activities.xaml?query=" + queryText.Text, UriKind.Relative));
        }
    }
}