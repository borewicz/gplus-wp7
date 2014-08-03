using GoogApp.Translations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace GoogApp
{
    public class LocalizedStrings
    {
        public LocalizedStrings() { }

        private static AppResources _localizedResources = new AppResources();

        public AppResources LocalizedResources
        {
            get { return _localizedResources; }
        }
    }

    public class NotificationBodyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string content = String.Empty;
            NotifyType type = (NotifyType)value;
            //var activities = (List<Notify.Activity>)value;
            //foreach (var activity in activities)
            //{
                switch (type)
                {
                    case NotifyType.AddedYouToCircle:
                        content = AppResources.addedYouToCircles;
                        break;
                    case NotifyType.Comment:
                        content = AppResources.commented;
                        break;
                    case NotifyType.MentionedYou:
                        content = AppResources.metionedYou;
                        break;
                    case NotifyType.NewCommunityPost:
                        content = AppResources.postedInCommunity;
                        break;
                    case NotifyType.NewPost:
                        content = AppResources.posted;
                        break;
                    case NotifyType.PlusForYou:
                        content = AppResources.gavePlusOne;
                        break;
                    case NotifyType.PlusForYourPost:
                        content = AppResources.plusForYourPost;
                        break;
                    case NotifyType.CommentedYourPost:
                        content = AppResources.commentedYourPost;
                        break;
                    default:
                        content = AppResources.didSomething;
                        break;
                }
            //}
            //var dateTime = (DateTime)value;
            //var difference = DateTime.UtcNow - dateTime.ToUniversalTime();
            //return thresholds.First(t => difference.TotalSeconds < t.Key).Value(difference);
            return content;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class NotificationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
                 var activities = (List<Notify.Activity>)value;
            //List<ConvertedNotify> converted = new List<ConvertedNotify>();
            List<Notify.Activity> converted = new List<Notify.Activity>();
            foreach (var a in activities)
            {
                if (a.activityType == NotifyType.AddedYouToCircle)
                {
                    if (converted.Any(item => item.activityType == NotifyType.AddedYouToCircle) == false)
                        converted.Add(a);
                }
                if (a.activityType == NotifyType.MentionedYou)
                {
                    if (converted.Any(item => item.activityType == NotifyType.MentionedYou) == false)
                        converted.Add(a);
                }
                if (a.activityType == NotifyType.Comment)
                {
                    if (converted.Any(item => item.activityType == NotifyType.Comment) == false)
                        converted.Add(a);
                }
                if (a.activityType == NotifyType.NewPost)
                {
                    if (converted.Any(item => item.activityType == NotifyType.NewPost) == false)
                        converted.Add(a);
                }
                if (a.activityType == NotifyType.NewCommunityPost)
                {
                    if (converted.Any(item => item.activityType == NotifyType.NewCommunityPost) == false)
                        converted.Add(a);
                }
                if (a.activityType == NotifyType.PlusForYou)
                {
                    if (converted.Any(item => item.activityType == NotifyType.PlusForYou) == false)
                        converted.Add(a);
                }
            }
            return converted;          
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            var isVisible = (bool)value;
            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visiblity = (Visibility)value;
            return visiblity == Visibility.Visible;
        }
    }

    public class UserOptionsVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string userID = (string)value;
            if (userID == Global.googLib.info.userID)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class SharedVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string shared = (string)value;
            if (shared != null)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class ButtonBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isPressed = (bool)value;
            return isPressed ? new SolidColorBrush(Colors.Red) : new SolidColorBrush((App.Current.Resources["PhoneForegroundBrush"] as SolidColorBrush).Color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class TicksConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime centuryBegin = new DateTime (1970, 1, 1);
            long elapsedTicks = (long)value;
            long nowTicks = 10000 * elapsedTicks + centuryBegin.Ticks;
            DateTime ticks = new DateTime(nowTicks, DateTimeKind.Utc);
            return ticks.ToLocalTime().ToShortDateString() + " " + ticks.ToLocalTime().ToShortTimeString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class YearsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            User.HistoryElement history = (User.HistoryElement)value;
            if (history.current == true)
                return history.start + " - " + "now";
            else
                return history.start + " - " + history.end;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class GenderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string content = string.Empty;
            Gender gender = (Gender)value;
            switch (gender)
            {
                case Gender.Female:
                    content = AppResources.female;
                    break;
                case Gender.Male:
                    content = AppResources.male;
                    break;
                case Gender.NoSpecified:
                    content = AppResources.noSpecified;
                    break;
                case Gender.Other:
                    content = AppResources.otherGender;
                    break;
            }
            return content;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class RelationshipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string content = string.Empty;
            Relationship ship = (Relationship)value;
            switch (ship)
            {
                case Relationship.CivilUnion:
                    content = AppResources.civilUnion;
                    break;
                case Relationship.Complicated:
                    content = AppResources.complicated;
                    break;
                case Relationship.DomesticPartnership:
                    content = AppResources.domesticPartnership;
                    break;
                case Relationship.DontSay:
                    content = AppResources.dontSay;
                    break;
                case Relationship.Engaged:
                    content = AppResources.engaged;
                    break;
                case Relationship.InRelationship:
                    content = AppResources.inRelationship;
                    break;
                case Relationship.Married:
                    content = AppResources.married;
                    break;
                case Relationship.OpenRelationship:
                    content = AppResources.openRelationship;
                    break;
                case Relationship.Single:
                    content = AppResources.single;
                    break;
                case Relationship.Widowed:
                    content = AppResources.widowed;
                    break;
            }
            return content;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class LookingForConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string content = string.Empty;
            LookingFor lookingFor = (LookingFor)value;
            switch (lookingFor)
            {
                case LookingFor.Dating:
                    content = AppResources.dating;
                    break;
                case LookingFor.Friends:
                    content = AppResources.friends;
                    break;
                case LookingFor.Networking:
                    content = AppResources.networking;
                    break;
                case LookingFor.Relationship:
                    content = AppResources.aRelationship;
                    break;
            }
            return content;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class RoleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string content = string.Empty;
            string role = (string)value;
            switch (role)
            {
                case "contributor-to" : content = AppResources.currentContributeTo;
                                    break;
                case "past-contributor-to" : content = AppResources.pastContributeTo;
                    break;
            }
            return content;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class PostTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string type = (string)value;
            if (type == "Events")
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
