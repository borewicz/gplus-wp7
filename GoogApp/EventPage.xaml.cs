using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GoogApp.Translations;

namespace GoogApp
{
    public partial class EventPage : PhoneApplicationPage
    {
        Event events;
        List<string> count = new List<string>();
        List<string> poll = new List<string>();
        bool navigated = false;

        string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public EventPage()
        {
            InitializeComponent();
            for (int i = 0; i < 10; i++)
                count.Add("+" + i.ToString());
            poll.Add(UppercaseFirst(AppResources.yes));
            poll.Add(AppResources.no);
            poll.Add(AppResources.maybe);
            pollListPicker.ItemsSource = poll;
            countListPicker.ItemsSource = count;
        }

        protected override async void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (navigated == true)
                return;
            navigated = true;
            string eventID;
            if (NavigationContext.QueryString.TryGetValue("eventID", out eventID))
            {
                if (events == null)
                {
                    events = await Global.googLib.GetEvent(eventID);
                    DataContext = events;
                    if ((events.youGoing != Poll.Unspecified) && (events.youGoing != Poll.Invited))
                        Load();
                    else
                        buttonsPanel.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        private void Load()
        {
            switch (events.youGoing)
            {
                case Poll.Yes: pollListPicker.SelectedIndex = 0;
                    break;
                case Poll.No: pollListPicker.SelectedIndex = 1;
                    break;
                case Poll.Maybe: pollListPicker.SelectedIndex = 2;
                    break;
            }
            countListPicker.SelectedIndex = events.yourGuestsCount;
            pickersPanel.Visibility = System.Windows.Visibility.Visible;
            countListPicker.SelectionChanged += countListPicker_SelectionChanged;
            pollListPicker.SelectionChanged += pollListPicker_SelectionChanged;
        }

        private async void pollListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int result;
            switch (pollListPicker.SelectedIndex)
            {
                case 0: if (events.youGoing != Poll.Yes) result = await Global.googLib.ReportPresence(events.eventID, Poll.Yes, events.tokenID);
                    events.youGoing = Poll.Yes;
                    break;
                case 1: if (events.youGoing != Poll.No) result = await Global.googLib.ReportPresence(events.eventID, Poll.No, events.tokenID);
                    events.youGoing = Poll.No;
                    break;
                case 2: if (events.youGoing != Poll.Maybe) result = await Global.googLib.ReportPresence(events.eventID, Poll.Maybe, events.tokenID);
                    events.youGoing = Poll.Maybe;
                    break;
            }
            if (events.youGoing != Poll.No)
                countListPicker.Visibility = System.Windows.Visibility.Visible;
            else
                countListPicker.Visibility = System.Windows.Visibility.Collapsed;

        }

        private async void countListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int result;
            if (events.yourGuestsCount != countListPicker.SelectedIndex)
               result = await Global.googLib.ReportGuestsPresence(events.eventID, countListPicker.SelectedIndex);
        }

        private async void Button_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            int result = 1;
            int select = Convert.ToInt32((string)(sender as Button).Tag);
            switch (select)
            {
                case 0: result = await Global.googLib.ReportPresence(events.eventID, Poll.Yes, events.tokenID);
                    events.youGoing = Poll.Yes;
                    pollListPicker.SelectedIndex = 0;
                    break;
                case 1: result = await Global.googLib.ReportPresence(events.eventID, Poll.No, events.tokenID);
                    events.youGoing = Poll.No;
                    pollListPicker.SelectedIndex = 1;
                    countListPicker.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case 2: result = await Global.googLib.ReportPresence(events.eventID, Poll.Maybe, events.tokenID);
                    events.youGoing = Poll.Maybe;
                    pollListPicker.SelectedIndex = 2;
                    break;
            }
            if (result == 0)
            {
                pickersPanel.Visibility = System.Windows.Visibility.Visible;
                buttonsPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}