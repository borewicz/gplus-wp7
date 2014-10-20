/*
 * GoogLib - unofficial G+ API for Mono/.NET
 * Copyright (C) 2013 Marek Kurniawka
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Phone.Controls;
using System.Windows.Controls;
using GoogApp.Translations;
using System.IO.IsolatedStorage;
using HtmlAgilityPack; 

namespace GoogApp
{
	#region Enums
	public enum AclType
	{
		Public = 1, 
		ExtendedCircles = 2, 
		YourCircles = 3, 
		SpecifiedCircle = 4,
		SpecifiedPerson = 5
	}

	public enum AbuseReason
	{
		Spam = 1,
		Nudity = 2,
		Hate = 3,
		Fake = 8
	}

    public enum Poll
    {
        Maybe = 6,
        Yes = 1,
        No = 2,
        Unspecified = 0,
        Invited = 5
    }
	
	public enum ActivityType
	{
		Reshare = 3,
		PlusOne = 2,
		Comment = 1
	}

    public enum NotifyType
    {
        NewCommunityPost = 49,
        Comment = 3,
        NewPost = 64,
        PlusForYou = 21,
        AddedYouToCircle = 6,
        MentionedYou = 15,
        PlusForYourPost = 20,
        CommentedYourPost = 1
    }

	public enum SearchType
	{
		Everything = 1,
		PeoplePages = 2,
		Posts = 3,
		Sparks = 4, //?
		Hangouts = 5,
		Communities = 6,
        Events = 7
	}

	public enum SearchPrivacy
	{
		Everyone = 1,
		Circles = 2,
		You = 5
	}

	public enum SearchCategory
	{
		Best = 1,
		Recent = 2
	}
	
	public enum LookingFor
	{
		Networking = 5,
		Relationship = 4,
		Dating = 3,
		Friends = 2
	}
	
	public enum Gender
	{
        NoSpecified = 0,
		Male = 1,
		Female = 2,
		Other = 3
	}
	
	public enum Relationship
	{
		DontSay = 0,
		Single = 2,
		InRelationship = 3,
		Engaged = 4,
		Married = 5,
		Complicated = 6,
		OpenRelationship = 7,
		Widowed = 8,
		DomesticPartnership = 9,
		CivilUnion = 10
	}
	
	public enum IM
	{
		NetMeeting = 10,
		AIM = 2,
		GTalk = 7,
		ICQ = 8,
		Jabber = 9,
		MSN = 3,
		QQ = 6,
		Skype = 5,
		Yahoo = 4
	}
	
	#endregion

	#region Element classes
	public class AclItem
	{
		public string id;
        public string name { get; set; }
		public AclType type;

		public AclItem (string id, string name, AclType type)
		{
			this.id = id;
			this.name = name;
			this.type = type;
		}
	}

	public class User 
	{
		public struct HistoryElement { 
            public string name { get; set; }
            public string title { get; set; }
            public string start { get; set; }
            public string end { get; set; }
            public string description { get; set; }
            public bool current { get; set; }
        }
        public struct Link {
            public string link { get; set; }
            public string faviconUrl { get; set; }
            public string title { get; set; }
            public string role { get; set; }
        }
		public struct Follower { public string id, name, avatar; }
		public class Contact { 
			public List<string> telephone = new List<string>();
			public List<string> cellphone = new List<string>();
			public List<string> email = new List<string>();
			public List<string> fax = new List<string>();
			public List<string> pager = new List<string>();
			public List<string> address = new List<string>();
			public Dictionary<string, IM> chat = new Dictionary<string, IM>(); } 
		//basic
		public string userId, profileUrl;
        public string name { get; set; }
        public string avatarUrl { get; set; }
        public string backgroundUrl { get; set; }

		//history
		public string tagline { get; set; }
        public string intro { get; set; }
        public string braggingRights { get; set; } //so much attention

		//education
        private List<HistoryElement> _education = new List<HistoryElement>();
        public List<HistoryElement> education { get { return _education; } }

		//some heavy stuff
        public string birthday { get; set; }
        public Gender gender { get; set; }
        public Relationship relationship { get; set; }
		private List<LookingFor> _lookingFor = new List<LookingFor>();
        public List<LookingFor> lookingFor { get { return _lookingFor; } }
		private List<string> _nicks = new List<string>();
        public List<string> nicks { get { return _nicks; } }

		//fapping
		public string occupation { get; set; }
        public string skills { get; set; }
        private List<HistoryElement> _employment = new List<HistoryElement>();
        public List<HistoryElement> employment { get { return _employment; } }

        public string home { get; set; }
        public string mapUrl { get; set; }
		private List<string> _olderHomes = new List<string>();
        public List<string> olderHomes { get { return _olderHomes; } }

		//contact
		public Contact workContact = new Contact();
		public Contact homeContact = new Contact();

		//a lot of spam links
		private List<Link> _otherProfiles = new List<Link>();    
		private List<Link> _contributeTo = new List<Link>();
		private List<Link> _links = new List<Link>();
        public List<Link> otherProfiles { get { return _otherProfiles; } }
        public List<Link> contributeTo { get { return _contributeTo; } }
        public List<Link> links { get { return _links; } }
		
		//followers xD
		public List<Follower> incoming = new List<Follower>();
		public List<Follower> visible = new List<Follower>();
	}

    public class Event
    {
        public class User { public string userID { get; set; } public string name { get; set; } public string avatar { get; set; } public int guests { get; set; } }
        private List<User> _going = new List<User>();
        private List<User> _maybe = new List<User>();
        private List<User> _noResponse = new List<User>();
        private List<User> _notGoing = new List<User>();
        private List<User> _unknown = new List<User>(); //7
        public List<User> going { get { return _going; } }
        public List<User> maybe { get { return _maybe; } }
        public List<User> noResponse { get { return _noResponse; } }
        public List<User> notGoing { get { return _notGoing; } }
        public List<User> unknown { get { return _unknown; } }
        public int goingCount { get; set; }
        public int maybeCount { get; set; }
        public int noResponseCount { get; set; }
        public int notGoingCount { get; set; }
        public int unknownCount { get; set; }
        public Poll youGoing { get; set; }
        public int yourGuestsCount { get; set; }
        public string eventID { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string location { get; set; }
        public string link { get; set; }
        public string tokenID { get; set; }
        public string image { get; set; }
        public string userID { get; set; }
        public string user { get; set; }
        public string avatar { get; set; }
    }
	
	public class Post
	{
        public class Photo {
            public string desc { get; set; }
            public string url { get; set; }
            public string albumUrl { get; set; }
            public string image { get; set; }
            public string thumbImage { get; set; }
        }
		public class Comment { 
            public string name { get; set; }
            public string content { get; set; } 
            public string time { get; set; }
            public string avatar { get; set; }
            public string commentID { get; set; }
            public string userID { get; set; }
            public int plusCount { get; set; }
            public bool youPlused { get; set; }
        }

		public class Activity { public string name, userID, avatar; public List<ActivityType> type = new List<ActivityType>(); }
        public class Shared {
            public string postID, userID;
            public string name { get; set; }
            public string avatar { get; set; }
            public string content { get; set; }
        }
        public class Community { public string id, category, categoryID; public string name { get; set; } }
		public class Media { 
            public string url { get; set; }
            public string title { get; set; }
            public string source { get; set; }
            public string address { get; set; }
            public string type {get; set; }
            public string thumbUrl { get; set; }
            public string photo { get; set; }
        }
        public string photo { get; set; } //temp

        private Media _media = new Media();
        private Shared _shared = new Shared();
        private Community _community = new Community();
        public Shared shared { get { return _shared; } }
        public Community community { get { return _community; } }
        public Media media { get { return _media; } }
        private List<Comment> _comments = new List<Comment>();
        public List<Comment> comments { get { return _comments; } set { this._comments = value; } }
		public List<Activity> activities = new List<Activity>();
        private List<Photo> _photos = new List<Photo>();
        public List<Photo> photos { get { return _photos; } }
        private Event _event = new Event();
        public Event events { get { return _event; } set { this._event = value; } }
		
		public string type { get; set; }
        public string postUrl;
        public string userID { get; set; }
        public string content { get; set; }
        public string editableContent { get; set; }
        public string author { get; set; }
        public long time { get; set; }
        public string avatar { get; set; }
        public string postID { get; set; }
        public int plusCount { get; set; }
        public int forwardCount { get; set; }
        public int commentsCount { get; set; }
        public bool isCommentingEnabled { get; set; }
        public bool isLocked { get; set; }
        public bool isMuted { get; set; }
        public bool youPlused { get; set; }
	}

    public class Posts
    {
        public string pageToken { get; set; }
        public string longPageToken { get; set; }
        private ObservableCollection<Post> _posts = new ObservableCollection<Post>();
        public ObservableCollection<Post> posts { get; set; }
    }

	public class Circle
	{
		public class User {
            public string id;
            public string name { get; set; }
            public string avatar { get; set; }
        }
        public string name { get; set; }
        public string description { get; set; }
		public List<User> users = new List<User>();
        public string id;
		public int position;		
	}
	
	public class Community
	{
		public class Category {
            public string id;
            public string name { get; set; }
        }
        public string id, longDescription;
        public string avatar { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int userCount { get; set; }
        public int postsCount { get; set; } 
		public List<Category> categories = new List<Category>();
	}
	
    public class Notification {
        public string title { get; set; }
        public string description { get; set; }
        public string communityID, userID, postID, eventID, id, url;
    }

	public class Notify
	{
		public class Activity {
            public string postID, userID;
            public string avatar { get; set; } 
            public double activityTime; 
            public NotifyType activityType { get; set; }
            public string name { get; set; }
        } 
		public class Notification { 
            public string communityID;
            public Post post { get; set; }
            private List<Activity> _activities = new List<Activity>();
            public List<Activity> activities { get { return this._activities; } set { this._activities = activities; } }
        } 
		public List<Notification> notifications = new List<Notification>();
        public int unreadCount { get; set; }
		public double lastReadTime;
		public double nowReadTime;
	}
	
	public class SearchQuery
	{
		public SearchCategory category;
		public SearchPrivacy privacy;
		public SearchType type;
		//public string precache, burst, bust_size?
	}

    public class UserInfo
    {
        public string name { get; set; }
        public string userID { get; set; }
        public string description { get; set; }
        public string avatar { get; set; }
    }

    public class SearchUsers
    {
        private ObservableCollection<UserInfo> _users = new ObservableCollection<UserInfo>();
        public ObservableCollection<UserInfo> users { get { return _users; } set { _users = value; } }
        public string pageToken { get; set; }
    }

    public class SearchCommunities
    {
        private ObservableCollection<Community> _communities = new ObservableCollection<Community>();
        public ObservableCollection<Community> communities { get { return _communities; } set { _communities = value; } }
        public string pageToken;
    }
	#endregion
	
	/*
	  WELCOME IN THE WORLD OF SHITTY CODE.
	  
	  In the beginning I want to send my thanks to Mohamed Mansour. He is my genious.
	  He made basic Google+ API for JavaScript. 
	  But JS is crap.
	  My library is better.
	  I wrote it in Mono.
	  And it have more beautiful functions.
	  So much win.
	*/
	
	public class GoogLib 
	{
		#region a lot of fucking lists, classes and other dirty stuff
		private RestClient _client;
		private string _foundSession; //nessesary for POSTs
        private string secToken; //
        private string timeStamp;

        int switcher = 0;
		public class Info { public string name, userID, avatar; public List<string> communities = new List<string>();}
		private Info _info = new Info();
		public Info info { get { return this._info; } }
        List<Circle> _circles = new List<Circle>();
        public List<Circle> circles { get { return _circles; } }
        List<Community> _communities = new List<Community>();
        public List<Community> communities { get { return _communities; } }
		
		#endregion

		#region ALOT OF CONST
		//ALOT OF CONST CONST CONST CONST CONST CONST CONST CONST CONST CONST CONST CONST
		//Programmers love CONST CONST CONST CONST CONST CONST CONST CONST CONST
		//CONST
		
		//------------------------ Basic --------------------------------------------------- 
		private const string CIRCLE_API              = "https://plus.google.com/_/socialgraph/lookup/circles/?m=true&rt=j";
		private const string FOLLOWERS_API           = "https://plus.google.com/_/socialgraph/lookup/followers/?m=1000000"; 
		private const string INITIAL_DATA_API        = "https://plus.google.com/_/initialdata?rt=j";

		//------------------------ Circles Management --------------------------------
		private const string MODIFYMEMBER_MUTATE_API = "https://plus.google.com/_/socialgraph/mutate/modifymemberships/"; 
		private const string REMOVEMEMBER_MUTATE_API = "https://plus.google.com/_/socialgraph/mutate/removemember/"; 
		private const string CREATE_MUTATE_API       = "https://plus.google.com/_/socialgraph/mutate/create/"; 
		private const string DELETE_MUTATE_API       = "https://plus.google.com/_/socialgraph/mutate/delete/"; 

		//------------------------ Searching -----------------------------------------------
        private const string QUERY_API = "https://plus.google.com/_/s/"; 

		//-------------------------- Posts management ------------------------------------
		private const string POST_API                = "https://plus.google.com/_/sharebox/post/?spam=20&rt=j"; 
		private const string MUTE_ACTIVITY_API       = "https://plus.google.com/_/stream/muteactivity/"; 
		private const string LOCK_POST_API           = "https://plus.google.com/_/stream/disableshare/";
		private const string DISABLE_COMMENTS_API    = "https://plus.google.com/_/stream/disablecomments/";
		private const string DELETE_ACTIVITY_API     = "https://plus.google.com/_/stream/deleteactivity/?rt=j";

		//-------------------------- Comments -------------------------------------------- 
		private const string COMMENT_API             = "https://plus.google.com/_/stream/comment/";
        private const string DELETE_COMMENT_API = "https://plus.google.com/_/stream/deletecomment&rt=j"; 

		//-------------------------- Profile management ----------------------------------------------
        private const string PROFILE_GET_API = "https://plus.google.com/_/profiles/get/";
		private const string PROFILE_REPORT_API      = "https://plus.google.com/_/profiles/reportabuse";

		//-------------------------- Activities --------------------------------------------
        private const string ACTIVITY_API = "https://plus.google.com/_/stream/getactivity/";
		private const string ACTIVITIES_API          = "https://plus.google.com/_/stream/getactivities/?rt=j";

		//-------------------------- Communities ------------------------------------------
		private const string COMMUNITIES_API         = "https://plus.google.com/_/communities/getcommunities?rt=j";
		private const string COMMUNITY_API           = "https://plus.google.com/_/communities/getcommunity?rt=j"; 
        private const string COMMUNITY_POSTS_API     = "https://plus.google.com/_/communities/getstream";

        //--------------------------Notifications ------------------------------------------
        private const string NOTIFICATIONS_COUNT_API = "https://plus.google.com/_/n/fetchcount?rt=j";
        private const string NOTIFICATIONS_FETCH_API = "https://plus.google.com/_/notifications/fetch?rt=j";
        private const string NOTIFICATIONS_UPDATE_API = "https://plus.google.com/_/notifications/updatelastviewedversion?rt=j";
        private const string NOTIFICATION_READ_API = "https://plus.google.com/_/notifications/setreadstates?rt=j";

		//-------------------------- Other -----------------------------------------------
        private const string LINK_DETAILS_API = "https://plus.google.com/_/sharebox/linkpreview/?rt=j";
		private const string HASHTAGS_API            = "https://plus.google.com/complete/search?hjson=t&client=es-hashtags&q="; 

        //-------------------------- Events --------------------------------------------
        private const string RSVP_API                = "https://plus.google.com/_/events/rsvp/?rt=j";
        private const string POLLING_API             = "https://plus.google.com/_/events/polling/";
        private const string INVITE_API              = "https://plus.google.com/_/events/invite/";
        private const string EVENT_API               = "https://plus.google.com/_/events/event/?rt=j"; //[" + eventID + "]"

		//-------------------------- To implement ---------------------------------------
		private const string PLUS_API                = "https://plus.google.com/_/plusone?rt=j";
		private const string EDIT_POST_API           = "https://plus.google.com/_/stream/edit/?spam=20&rt=j";
		private const string LOOKUP_VISIBLE_API      = "https://plus.google.com/_/socialgraph/lookup/visible/"; 
		private const string LOOKUP_INCOMING_API     = "https://plus.google.com/_/socialgraph/lookup/incoming/";
		private const string MARK_AS_READ_API        = "https://plus.google.com/_/stream/markitemread/"; 
		private const string ENGAGEMENT_API          = "https://plus.google.com/_/stream/getengagementsummary/";
        private const string EDIT_COMMENT_API = "https://plus.google.com/_/stream/editcomment/&rt=j";
        private const string UPLOAD_PHOTO_API        = "https://plus.google.com/_/upload/photos/resumable";
		
		//-------------------------- Unused/Obsolete---------------------------------------------
		private const string LOOKUP_HOVERCARDS_API   = "https://plus.google.com/_/socialgraph/lookup/hovercards/"; //hovercard, not used
		private const string PAGES_API               = "https://plus.google.com/_/pages/get/"; //only return circlesID
		private const string PROFILE_SAVE_API        = "https://plus.google.com/_/profiles/save?_reqid=0"; //too much work to implement this
		private const string PROPERTIES_MUTATE_API   = "https://plus.google.com/_/socialgraph/mutate/properties/"; //garbage
		private const string SORT_MUTATE_API         = "https://plus.google.com/_/socialgraph/mutate/sortorder/"; //garbage
		private const string BLOCK_MUTATE_API        = "https://plus.google.com/_/socialgraph/mutate/block_user/"; //not used
		private const string FIND_PEOPLE_API         = "https://plus.google.com/_/socialgraph/lookup/find_more_people/?m=10000"; //not used
        private const string LANDING_API             = "https://plus.google.com/_/communities/landing"; //replaced by COMMUNITY_POSTS_API & COMMUNITY_API
        private const string NOTIFICATION_API        = "https://plus.google.com/_/notifications/getnotificationsdata"; //replaced by new notifications API
        private const string UPDATE_TIME_API         = "https://plus.google.com/_/notifications/updatelastreadtime"; //replaced by new notifications API

		#endregion
		
		public GoogLib()
		{
			_client = new RestClient();
			_info = new Info();
            _client.CookieContainer = new System.Net.CookieContainer();
            _client.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:32.0) Gecko/20100101 Firefox/32.0.3 gPlus/1.0";
		}

        //------------------------ Private functions ------------------------

		#region Parsing

		//Google create some incredible fucked JSONs which aren't parsable
		//this genius function converts dirty and ugly Google JSONs to standard JSONs
		//+RestSharp needs to have "data" object
		private string _parseJSON (string input)
		{
			string jsonString = input.Replace ("[,", "[null,");
			jsonString = jsonString.Replace (",]", ",null]");
			jsonString = jsonString.Replace (",,", ",null,");
			jsonString = jsonString.Replace (",,", ",null,");
			jsonString = Regex.Replace (jsonString, "{(\\d+):", "{\"$1\":");
			string output = jsonString.Substring(6);
			//output = "{ \"data\" : [" + output + "]}";
			return output;
		}

		//parsing acl items (e.g circles)
		private JArray _parseAclItems (List<AclItem> list)
		{
			JArray array = new JArray ();
			foreach (AclItem item in list) {
				if (item.type == AclType.Public)
					array.Add ((object)new JArray (null, null, 1));
				else if (item.type == AclType.ExtendedCircles)
					array.Add ((object)new JArray (null, null, 4));
				else if (item.type == AclType.YourCircles)
					array.Add ((object)new JArray (null, null, 3));
				else if (item.type == AclType.SpecifiedCircle)
					array.Add ((object)new JArray (null, item.id));
				else if (item.type == AclType.SpecifiedPerson)
					array.Add ((object)new JArray (null, null, item.id));
			}
			return array;
		}

        private string _createSmallImageLink(string url, string x)
        {
            string[] separateURL = url.Split('/');
            //x 46 or 28
            separateURL[separateURL.Count() - 1] = "s" + x + "-c-k-no/" + separateURL[separateURL.Count() - 1];
            return String.Join("/", separateURL);
        }

        private Event _parseEvent (JToken eventData)
        {
            Event events = new Event();
            if (eventData[0].ToString().Contains("gallery"))
                eventData = eventData[178].Last.First.First;
            //var eventData = data[97][data[97].Count() - 1].First.First;
            events.eventID = eventData[105].ToString();        events.title = eventData[2].ToString();
            events.tokenID = eventData[44].ToString();
            events.description = eventData[3].ToString();
            //events.link = "https://plus.google.com/" + eventData[0].ToString();
            //events.start = eventData[73].ToString();
            //events.end = eventData[162].ToString();
            events.userID = eventData[26].ToString();
            if (eventData[72] != null && eventData[72].HasValues)
                events.location = eventData[72].Last.First.First[2].ToString();
            if (eventData[169].HasValues)
            {
                events.user = eventData[169].Last.First.First[2].ToString();
                events.avatar = _parseLink(eventData[169].Last.First.First[2].ToString());
            }
            if (eventData[174].HasValues)
            {
                if (eventData[174][14].HasValues)
                {
                    events.start = eventData[174][14][1].ToString();
                    events.link = _parseLink(eventData[174][14][4].ToString());
                    events.description = eventData[174][14][2].ToString();
                    /*
                    if (eventData[174][14][8].HasValues)
                    {
                        events.youGoing = (Poll)(int)eventData[174][14][8][0][3];
                        events.yourGuestsCount = (int)eventData[174][14][8][0][4];
                    }
                     */
                }
                if (eventData[174][15].HasValues) foreach (var item in eventData[174][15])
                    {
                        if (((int)item[3] != 0) && ((int)item[0] != 7))
                        {
                            try
                            {
                                events.youGoing = (Poll)(int)item[0];
                                events.yourGuestsCount = (int)item[4];
                            }
                            catch { }
                        }
                        switch ((int)item[0])
                        {
                            case 1: events.goingCount = (int)item[1];
                                /*
                                foreach (var i in (JArray)item[2])
                                {
                                    Event.User user = new Event.User();
                                    user.name = i[0][0].ToString();
                                    user.userID = i[0][1].ToString();
                                    user.avatar = _parseLink(i[0][2].ToString());
                                    try
                                    {
                                        user.guests = (int)i[2];
                                    }
                                    catch { }
                                    events.going.Add(user);
                                }
                                 */
                                break;
                            case 2: events.notGoingCount = (int)item[1];
                                /*
                                foreach (var i in (JArray)item[2])
                                {
                                    Event.User user = new Event.User();
                                    user.name = i[0][0].ToString();
                                    user.userID = i[0][1].ToString();
                                    user.avatar = _parseLink(i[0][2].ToString());
                                    try
                                    {
                                        user.guests = (int)i[2];
                                    }
                                    catch { }
                                    events.notGoing.Add(user);
                                }
                                 */
                                break;
                            case 5: events.noResponseCount = (int)item[1];
                                /*
                                foreach (var i in (JArray)item[2])
                                {
                                    Event.User user = new Event.User();
                                    user.name = i[0][0].ToString();
                                    user.userID = i[0][1].ToString();
                                    user.avatar = _parseLink(i[0][2].ToString());
                                    try
                                    {
                                        user.guests = (int)i[2];
                                    }
                                    catch { }
                                    events.noResponse.Add(user);
                                }
                                 */
                                break;
                            case 6: events.maybeCount = (int)item[1];
                                /*
                                foreach (var i in (JArray)item[2])
                                {
                                    Event.User user = new Event.User();
                                    user.name = i[0][0].ToString();
                                    user.userID = i[0][1].ToString();
                                    user.avatar = _parseLink(i[0][2].ToString());
                                    try
                                    {
                                        user.guests = (int)i[2];
                                    }
                                    catch { }
                                    events.maybe.Add(user);
                                }
                                 */
                                break;
                                /*
                            case 7: events.unknownCount = (int)item[1];
                                foreach (var i in (JArray)item[2])
                                {
                                    Event.User user = new Event.User();
                                    user.name = i[0][0].ToString();
                                    user.userID = i[0][1].ToString();
                                    user.avatar = _parseLink(i[0][2].ToString());
                                    try
                                    {
                                        user.guests = (int)i[2];
                                    }
                                    catch { }
                                    events.unknown.Add(user);
                                }
                                break;
                                 */
                        }
                    }
                if (eventData[174][16].HasValues)
                {
                    var imageItem = (from i in eventData[174][16][1]
                                        where i[2].ToString().Contains(".jpg") == true
                                        select i).FirstOrDefault();
                    if (imageItem != null)
                        events.image = imageItem[2].ToString();
                }
            }
            return events;
        }


		//parsing post
		//a lot of parsing, wow
		private Post _parsePost (JToken data)
		{
            Post post = new Post();
            post.type = data[2].ToString();
            post.author = data[3].ToString();
            post.time = Convert.ToInt64(data[5].ToString());
            post.postID = data[8].ToString();
            post.content = data[4].ToString(); //4, 14 - plain/html formatted
            post.editableContent = data[20].ToString();
            post.userID = data[16].ToString();
            //post.avatar = data [18].ToString();
            string postAvatar = data[18].ToString();
            if (postAvatar == "")
                post.avatar = "https://ssl.gstatic.com/s2/profiles/images/silhouette46.png";
            else
                post.avatar = _createSmallImageLink(postAvatar, "46");
            post.postUrl = data[21].ToString();
            post.isCommentingEnabled = Convert.ToBoolean((int)data[35]);
            post.isLocked = !Convert.ToBoolean((int)data[89]);
            post.isMuted = Convert.ToBoolean((int)data[34]);
            post.commentsCount = Convert.ToInt32(data[93].ToString());
            post.plusCount = Convert.ToInt32(data[73][16].ToString());
            post.forwardCount = Convert.ToInt32(data[96].ToString());
            post.youPlused = Convert.ToBoolean((int)data[73][13]);

            foreach (JArray comments in data[7])
            {
                Post.Comment comment = new Post.Comment();
                comment.name = comments[1].ToString();
                comment.content = comments[2].ToString();
                comment.time = comments[3].ToString();
                comment.commentID = comments[4].ToString();
                comment.userID = comments[6].ToString();
                if (comments[15][16].ToString() != "")
                {
                    comment.plusCount = Convert.ToInt32(comments[15][16].ToString());
                    comment.youPlused = Convert.ToBoolean((int)comments[15][13]);
                }
                //comment.avatar = comments [16].ToString ();
                try
                {
                    string avatar = comments[16].ToString();
                    comment.avatar = _createSmallImageLink(comments[16].ToString(), "36");
                    if (avatar == "")
                        comment.avatar = "https://ssl.gstatic.com/s2/profiles/images/silhouette36.png";
                    else
                        comment.avatar = _createSmallImageLink(avatar, "36");
                }
                catch { }
                post.comments.Add(comment);
            }
            try
            {
                if (data[43].HasValues)
                {
                    post.shared.avatar = _parseLink(data[43][4].ToString());
                    post.shared.name = data[43][0].ToString();
                    post.shared.userID = data[43][1].ToString();
                    post.shared.postID = data[39].ToString();
                    post.shared.content = data[48].ToString();
                }
            }
            catch { }

//                try
                {

                    if (data[2].ToString() == "Events")
                    {
                        if (data[97].HasValues)
                        {
                            Event events = _parseEvent(data[97].Last.First.First);
                            post.events = events;
                        }
                    }
                    else if ((data[2].ToString() == "Photos") || ((data[36].Type != JTokenType.Null) && (string)data[36] != "0"))
                    {
                        foreach (var item in data[97].Last.First.First[41])
                        {
                            Post.Photo photo = new Post.Photo();
                            photo.desc = item.Last.First.First[3].ToString();
                            photo.albumUrl = item.Last.First.First[0].ToString();
                            if (item.Last.First.First[5].HasValues)
                                photo.thumbImage = _parseLink(item.Last.First.First[5][0].ToString());
                            photo.image = item.Last.First.First[1].ToString();
                            post.photos.Add(photo);
                        }
                        var mainPhoto = (from p in post.photos
                                         where p.albumUrl.Contains(data[97].Last.First.First[26].ToString())
                                         select p).FirstOrDefault();
                        if (mainPhoto != null)
                            post.photo = mainPhoto.thumbImage;
                        else if (data[97].Last.First.First[5].HasValues)
                            post.photo = _parseLink(data[97].Last.First.First[5][0].ToString());
                    }

                    else //if (data[2].ToString() == "Google+" || data[2].ToString() == "HootSuite")
                    {
                        if (data[97].HasValues)
                        {
                            if (data[97][1] != null)
                            {
                                if (data[97][1].ToString().Contains("www.youtube.com"))
                                {
                                    post.media.title = data[97].Last.First.First[2].ToString();
                                    post.media.url = data[97][1].ToString();
                                    if (data[97].Last.First.First[5].HasValues)
                                    {
                                        post.photo = _parseLink(data[97].Last.First.First[5][0].ToString());
                                        if (post.photo.EndsWith("-n") == false) post.photo += "-n";
                                    }
                                }
                                else if (data[97][1].ToString().Contains("http"))
                                {
                                    post.media.url = data[97].Last.First.First[0].ToString();
                                    post.media.title = data[97].Last.First.First[2].ToString();
                                    post.media.source = GetDomainName(data[97].Last.First.First[0].ToString());
                                    if (data[97].Last.First.First[5].HasValues)
                                        post.media.thumbUrl = _parseLink(data[97].Last.First.First[5][0].ToString());
                                    //post.media.thumbUrl = _parseLink(post.media.thumbUrl); 
                                }
                            }
                        }
                    }
                }
   //             catch
   //             {
   //             }
                //if 

                try
                {
                    if (post.type == "Community")
                    {
                        post.community.id = data[108][0].ToString();
                        post.community.name = data[108][1].ToString();
                        post.community.category = data[108][2].ToString();
                        post.community.categoryID = data[108][3].ToString();
                    }
                }
                catch { }

                //post.activities = _getPostActivity(post.postID);
			return post;
		}

        private string GetDomainName(string url)
        {
            try
            {
                string domain = new Uri(url).DnsSafeHost.ToLower();
                var tokens = domain.Split('.');
                if (tokens.Length > 2)
                {
                    //Add only second level exceptions to the < 3 rule here
                    string[] exceptions = { "info", "firm", "name", "com", "biz", "gen", "ltd", "web", "net", "pro", "org" };
                    var validTokens = 2 + ((tokens[tokens.Length - 2].Length < 3 || exceptions.Contains(tokens[tokens.Length - 2])) ? 1 : 0);
                    domain = string.Join(".", tokens, tokens.Length - validTokens, validTokens);
                }
                return domain;
            }
            catch
            {
                return url;
            }
        }

		
		//parsing user data
		//a lot of parsing of parsing of parsing
		private User _parseUser (JArray data)
		{
			User user = new User ();
			user.userId = data [0].ToString ();
			user.profileUrl = data [2] [2].ToString ();
            user.avatarUrl = _parseLink(data[2][3].ToString());
			user.name = data [2] [4] [3].ToString ();
			foreach (JArray nick in data[2][5][1])
				user.nicks.Add (nick[0].ToString ());
		    if (data[2][6].HasValues) user.occupation = data [2] [6] [1].ToString ();
			foreach (JArray employment in data[2][7][1]) {
				User.HistoryElement element = new User.HistoryElement ();
				element.name = employment [0].ToString ();
				element.title = employment [1].ToString ();
                if (employment[2].HasValues == true)
                {
                    if (employment[2][0].HasValues == true)
                        element.end = employment[2][0][2].ToString();
                    element.start = employment[2][1][2].ToString();
                }
				element.description = employment [4].ToString ();
				user.employment.Add (element);
			}
			foreach (JArray school in data[2][8][1]) {
				User.HistoryElement element = new User.HistoryElement ();
				element.name = school [0].ToString ();
				element.title = school [1].ToString ();
                if (school[2].HasValues == true)
                {
                    if (school[2][0].HasValues == true)
                        element.end = school[2][0][2].ToString();
                    element.start = school[2][1][2].ToString();
                }
				element.description = school [4].ToString ();
				user.education.Add (element);
			}
			user.home = data [2] [9] [1].ToString ();
            user.mapUrl = _parseLink(data[2][10].ToString());
			foreach (var occ in data[2][9][2])
				user.olderHomes.Add (occ.ToString ());
			//========================================
			foreach (var contact in data[2][12][1])
				user.homeContact.telephone.Add(contact.ToString());
			foreach (var contact in data[2][12][2])
				user.homeContact.cellphone.Add(contact.ToString());
			foreach (var contact in data[2][12][3])
				user.homeContact.fax.Add(contact.ToString());
			foreach (var contact in data[2][12][4])
				user.homeContact.pager.Add(contact.ToString());
			foreach (var contact in data[2][12][6])
				user.homeContact.address.Add(contact.ToString());
			foreach (var contact in data[2][12][7])
				user.homeContact.chat.Add(contact[0].ToString(), (IM)Convert.ToInt32(contact[1].ToString()));
			foreach (var contact in data[2][12][9])
				user.homeContact.email.Add(contact[0].ToString());
			//========================================
			foreach (var contact in data[2][13][1])
				user.workContact.telephone.Add(contact.ToString());
			foreach (var contact in data[2][13][2])
				user.workContact.cellphone.Add(contact.ToString());
			foreach (var contact in data[2][13][3])
				user.workContact.fax.Add(contact.ToString());
			foreach (var contact in data[2][13][4])
				user.workContact.pager.Add(contact.ToString());
			foreach (var contact in data[2][13][6])
				user.workContact.address.Add(contact.ToString());
			foreach (var contact in data[2][13][7])
				user.workContact.chat.Add(contact[0].ToString(), (IM)Convert.ToInt32(contact[1].ToString()));
			foreach (var contact in data[2][13][9])
				user.workContact.email.Add(contact[0].ToString());
			//========================================
            user.tagline = data[2][14][1].ToString();
			//user.birthday = data[2][16][3].ToString() + data[2][16][4].ToString() + data[2][16][5].ToString();
            if (data[2][17].HasValues) user.gender = (Gender)Convert.ToInt32(data[2][17][1].ToString());
            if (data[2][19].HasValues) user.braggingRights = data[2][19][1].ToString();
            if (data[2][22].HasValues) user.relationship = (Relationship)Convert.ToInt32(data[2][22][1].ToString());
			foreach (var element in data[2][23][1])
				user.lookingFor.Add((LookingFor)Convert.ToInt32(element[0].ToString()));
			user.intro = data [2] [33] [1].ToString ();
			foreach (JArray links in data[2][51][0]) {
				User.Link link = new User.Link ();
				link.link = links [1].ToString ();
                link.faviconUrl = _parseLink(links[2].ToString());
				link.title = links [3].ToString ();
				user.otherProfiles.Add (link);
			}
			foreach (JArray links in data[2][52][0]) {
				User.Link link = new User.Link ();
				link.link = links [1].ToString ();
                link.faviconUrl = _parseLink(links[2].ToString());
				link.title = links [3].ToString ();
				link.role = links [4].ToString ();
				user.contributeTo.Add (link);
			}
			foreach (JArray links in data[2][53][0]) {
				User.Link link = new User.Link ();
				link.link = links [1].ToString ();
				link.faviconUrl = _parseLink(links [2].ToString ());
				link.title = links [3].ToString ();
				user.links.Add (link);
			}
            if (data[2][76].HasValues) user.skills = data[2][76][1].ToString();
			user.backgroundUrl = data [10] [5].ToString ();
			return user;
		}
		
		//parsing Circles
		private List<Circle> _parseCircle (JArray data)
		{
            List<Circle> circles = new List<Circle>();
			foreach (JArray element in data[switcher][1]) {
				Circle circle = new Circle ();
				circle.id = element [0] [0].ToString ();
				circle.name = element [1] [0].ToString ();
				circle.description = element [1] [2].ToString ();
                //circles.Add(circle);
                if (circle.id != "15")
                    circles.Add(circle);
			}
            /*
			foreach (JArray element in data[1][2]) {
				Circle.User user = new Circle.User ();
				user.id = element [0] [2].ToString ();
				//user.name = element [2] [0].ToString ();
				//user.avatar = _parseLink(element [2] [8].ToString ());
				string circleId = element [3] [0] [2] [0].ToString ();
                if (circleId != "15")
                {
                    var circle = (from item in circles
                                  where item.id == circleId
                                  select item).First();
                    circle.users.Add(user);
                }
			}
             */
            return circles;
		}
		
		//parsing Communities
		private Community _parseCommunity (JArray data)
		{
			Community community = new Community ();
			community.id = data [0] [0] [0].ToString();
			community.name = data [0] [0] [1] [0].ToString();
			community.description = data [0] [0] [1] [1].ToString();
            community.avatar = _createSmallImageLink(_parseLink(data[0][0][1][3].ToString()), "60");
			community.longDescription = data [0] [0] [1] [8].ToString();
			return community;
		}
		
		//parsing Followers
		private User.Follower _parseFollower (JArray data)
		{
			User.Follower follower = new User.Follower ();
			follower.id = data [0] [2].ToString();
			follower.name = data [2] [0].ToString();
            follower.avatar = _parseLink(data[2][8].ToString());
			return follower;
		}	
		
		
		private Notify _parseNotify (JArray data)
		{
            Notify notificationList = new Notify();
            notificationList.lastReadTime = Convert.ToDouble(data[1].ToString());
            notificationList.nowReadTime = Convert.ToDouble(data[2].ToString());
            notificationList.unreadCount = Convert.ToInt32(data[7].ToString());
			foreach (var item in data[0]) {
				Notify.Notification notification = new Notify.Notification ();
                if (((JArray)item).Count == 24)
                    notification.communityID = item[23].ToString().Split('/')[2];
				//notification.communityUrl = item.Value<string>(23) ?? null;
				if (item [18][0].HasValues && item [18][0][0].Type != JTokenType.Null)
					notification.post = _parsePost (item [18] [0][0]);
				foreach (var i in item[2][0][1]) {
					Notify.Activity activity = new Notify.Activity ();
					activity.postID = i[0].ToString();
                    activity.activityType = (NotifyType)Convert.ToInt32(i[1].ToString());
                    activity.activityTime = Convert.ToDouble(i[3].ToString());
					activity.name = i [2] [0].ToString();
					activity.userID = i [2] [3].ToString();
					activity.avatar = _parseLink(i[2][2].ToString());
					notification.activities.Add (activity);
				}
				if (((JArray)item[2]).Count > 1) foreach (var i in item[2][1][1]) {
					Notify.Activity activity = new Notify.Activity ();
					activity.postID = i[0].ToString();
					activity.activityType = (NotifyType)Convert.ToInt32 (i [1].ToString());
                    activity.activityTime = Convert.ToDouble(i[3].ToString());
					activity.name = i [2] [0].ToString();
					activity.userID = i [2] [3].ToString();
                    activity.avatar = _parseLink(i[2][2].ToString());
                    notification.activities.Add(activity);
				}
                notificationList.notifications.Add(notification);
			}
            return notificationList;
		}
		
		#endregion

		#region Connecting and getting basic informations

        public async Task<int> Connect(string email, string password, CookieContainer container)
        {
            //if (container == null)
                return await _getCookie(email, password);
            /*
            else
            {
                _client.CookieContainer = container;
                this._info = await _refreshInfo();
                if (this._info == null)
                    return 2;
                else
                {
                    this._foundSession = await _getSession();
                    return 0;
                }
            }
             */
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

        void getSecTok(string html, ref string secTok, ref string timeStamp)
        {
            string[] stringSeparators = new string[] {"id=\"timeStmp\"\n       value='", "'/>\n<input type=\"hidden\" name=\"secTok\" id=\"secTok\"\n       value='", "'/>\n  <input type=\"hidden\" name=\"smsToken\""};
            string[] split = html.Split(stringSeparators, StringSplitOptions.None).ToArray();
            secTok = split[2];
            timeStamp = split[1];
        }

		//sign in to Google+ using cookies
		//usually programmers save them somewhere, but I'm lazy and cannot into internets
		private async Task<int> _getCookie(string email, string password)
		{
			Console.Write("_getCookie (): Getting cookies... ");
			var request = new RestRequest("https://plus.google.com/", Method.GET);
            var resp = await _client.GetResponseAsync(request);
			if (resp.StatusCode == HttpStatusCode.OK)
			{              
				var cookieRequest = new RestRequest("https://accounts.google.com/ServiceLoginAuth", Method.POST);
                cookieRequest.AddParameter("GALX", resp.Cookies[0].Value);
				cookieRequest.AddParameter("Email", email);
				cookieRequest.AddParameter("Passwd", password);
                cookieRequest.AddParameter("PersistentCookie", "true");
				cookieRequest.AddParameter("continue", "https://plus.google.com/?gpsrc=gplp0");
                cookieRequest.AddParameter("bgresponse", "js_disabled");
                /*
  <input name="service" type="hidden" value="oz">
cookieRequest.AddParameter("_utf8", "&#9731;");
  <input type="hidden" id="pstMsg" name="pstMsg" value="0">
  <input type="hidden" id="dnConn" name="dnConn" value="">
  <input type="hidden" id="checkedDomains" name="checkedDomains"
         value="youtube">
<input id="signIn" name="signIn" class="rc-button rc-button-submit" type="submit" value="Zaloguj się">
  <label class="remember">
  <input  id="PersistentCookie" name="PersistentCookie"
                 type="checkbox" value="yes"
                 checked="checked">
                 */

                cookieRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");

                var cookieResp = await _client.GetResponseAsync(cookieRequest);
                if (cookieResp.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine("OK.");
                    if (cookieResp.ResponseUri.ToString().Contains("accounts.google.com/LoginVerification"))
                        return 4;
                    if (cookieResp.ResponseUri.ToString().Contains("accounts.google.com/SecondFactor"))
                    {
                        getSecTok(cookieResp.Content, ref this.secToken, ref this.timeStamp);
                        return 3;
                    }
                    this._info = await _refreshInfo();
                    if (this._info == null)
                        return 2;
                    else
                    {
                        //MessageBox.Show("_info : OK", "DEBUG", MessageBoxButton.OK);
                        //JArray content = _getInfoFromHTML(cookieResp.Content, 1);
                        //if (content == null) return 1;
                        this._foundSession = await _getSessionId();
                        //MessageBox.Show("_foundSession : OK", "DEBUG", MessageBoxButton.OK);
                        //string sessionString = "\"https://csi.gstatic.com/csi\",\"";
                        //int index = cookieResp.Content.IndexOf(sessionString);
                        //string remain = cookieResp.Content.Substring(index + sessionString.Length);
                        //this._foundSession = remain.Substring(0, remain.IndexOf("\""));
                        //_circles = _parseCircle((JArray)_getInfoFromHTML(cookieResp.Content, 12));
                        _circles = await GetCircles();
                        //MessageBox.Show("_circles : OK", "DEBUG", MessageBoxButton.OK);
                        _communities = await GetYourCommunities();
                        //MessageBox.Show("_communities : NOT_IMPLEMENTED", "DEBUG", MessageBoxButton.OK);
                        return 0;
                    }
                }
                else
                    return 1;
			}
			else    
                return 1;
		}

        public async Task<int> loginUsingToken(string token)
        {
            //https://accounts.google.com/SecondFactor?checkConnection=youtube%3A1126%3A1&checkedDomains=youtube&pstMsg=1&secTok=.AG5fkS-1gjBbwnUJnBwhBCuu7_m7AXVZ6w%3D%3D&smsToken=&smsUserPin=361638&smsVerifyPin=Weryfikuj&timeStmp=1379588679

            var secondFactorRequest = new RestRequest("https://accounts.google.com/SecondFactor", Method.POST);
            secondFactorRequest.AddParameter("secTok", secToken);
            secondFactorRequest.AddParameter("smsUserPin", token);
            secondFactorRequest.AddParameter("timeStmp", timeStamp);
            secondFactorRequest.AddParameter("PersistentCookie", "true");
            //checkConnection	youtube:1126:1
            //secondFactorRequest.AddParameter("checkedDomains","youtube");
            //secondFactorRequest.AddParameter("pstMsg", 1);
            //secondFactorRequest.AddParameter("smsToken", "");
            secondFactorRequest.AddParameter("PersistentOptionSelection", 1);
            //secondFactorRequest.AddParameter("continue", "https://plus.google.com/?gpsrc=gplp0");
            secondFactorRequest.AddParameter("service", "oz");
            secondFactorRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            var secondFactorResp = await _client.GetResponseAsync(secondFactorRequest);
            if ((secondFactorResp.StatusCode != HttpStatusCode.OK) && (secondFactorResp.StatusCode != HttpStatusCode.Found))
                return 1;
            else
            {
                this._info = await _refreshInfo();
                if (this._info == null)
                    return 2;
                else
                {
                    this._foundSession = await _getSessionId();
                    _circles = await GetCircles();
                    _communities = await GetYourCommunities();
                    return 0;
                }
            }
        }


		//at parameter is nessesary to POSTs
        private async Task<string> _getSession()
        {
            Console.Write("_getSession (): Getting \"at\" string...");
            string searchForString = "\"https://csi.gstatic.com/csi\",\"";
            var request = new RestRequest("https://plus.google.com/", Method.GET);
            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                int startIndex = resp.Content.IndexOf(searchForString);
                string remainingText = resp.Content.Substring(startIndex + searchForString.Length);
                Console.WriteLine("OK.");
                return remainingText.Substring(0, remainingText.IndexOf("\""));
            }
            else
                return null;
        }

		
		//initial info about you
		private async Task<Info> _refreshInfo ()
		{
            Info newInfo = new Info();
			Console.Write ("_refreshInfo (): Refreshing info...");
			var request = new RestRequest (INITIAL_DATA_API, Method.POST);
            request.AddParameter("key", 2);
            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    JArray root = JArray.Parse(_parseJSON(resp.Content));
                    try
                    {
                        if ((string)root[0][0][0] == "f.ri")
                            switcher = 1;
                    }
                    catch { }
                    JObject info = JObject.Parse(root[0][switcher][1].ToString());
                    if (info["2"].HasValues)
                    {
                        newInfo.userID = (string)info["2"][0];
                        newInfo.avatar = _parseLink(info["2"][1][3].ToString());
                        newInfo.name = info["2"][1][4][3].ToString();
                        Console.WriteLine("OK.");
                        return newInfo;
                    }
                    else return null;
                }
                catch
                {
                    return null;
                }
            }
            else
                return null;
		}

        //initial info about you
        private async Task<string> _getSessionId()
        {
            var request = new RestRequest(INITIAL_DATA_API, Method.POST);
            request.AddParameter("key", 1);
            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                JArray root = JArray.Parse(_parseJSON(resp.Content));
                JObject info = JObject.Parse(root[0][switcher][1].ToString());
                if (info["1"].HasValues)
                    return info["1"][15].ToString();
                else
                    return null;
            }
            else
                return null;
        }
		
		#endregion
		
		#region Other
		//TIK TOK TIK TOK TIK TOK TIKTOK! 
		private long _getTicks ()
		{
			DateTime centuryBegin = new DateTime (1970, 1, 1);
			DateTime currentDate = DateTime.Now;
			long elapsedTicks = (currentDate.Ticks - centuryBegin.Ticks) / 10000; //odcinamy cyferki
			return elapsedTicks;
		}

        private string _parseLink (string link)
        {
            return (link.Contains("https:") ? link : "https:" + link);
        }

		//INCOMMING!!!!! BUM!
		private async Task<List<User.Follower>> _getFollowersIncoming (string userID)
		{
            List<User.Follower> followers = new List<User.Follower>();
			var request = new RestRequest (LOOKUP_INCOMING_API, Method.POST);
			request.AddParameter ("o", "[null,null,\"" + userID + "\"]");
			request.AddParameter ("at", _foundSession);
			request.AddParameter ("n", 1000);
            var resp = await _client.GetResponseAsync(request);
			JArray root = JArray.Parse(_parseJSON(resp.Content));
			foreach (var follower in root[0][2])
				followers.Add(_parseFollower((JArray)follower));
            return followers;					
		}
		
		//i like your naming, Google
		private async Task<List<User.Follower>> _getFollowersVisible (string userID)
		{
            List<User.Follower> followers = new List<User.Follower>();
			var request = new RestRequest (LOOKUP_VISIBLE_API, Method.POST);
			request.AddParameter ("at", _foundSession);
			request.AddParameter ("o", "[null,null,\"" + userID + "\"]");
            var resp = await _client.GetResponseAsync(request);
			JArray root = JArray.Parse(_parseJSON(resp.Content));
			foreach (var follower in root[0][2])
				followers.Add(_parseFollower((JArray)follower));
            return followers;			
		}
		
				
		private async Task<int> _updateLastReadTime () //TODO: parsing?
		{
			var request = new RestRequest (UPDATE_TIME_API, Method.POST);
			request.AddParameter ("f.req", "[" + this._getTicks () + "]");
			request.AddParameter ("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
                return 0;
            else return 1;
		}

        private async Task<int> _updateLastViewedVersion()
        {
            var request = new RestRequest(NOTIFICATIONS_UPDATE_API, Method.POST);
            request.AddParameter("f.req", "[[\"OGB\"],[\"GPLUS_APP\"," + this._getTicks() + "]]");
            request.AddParameter("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
                return 0;
            else return 1;
        }
		
		public async Task<List<Post.Activity>> _getPostActivity (string postID)
		{
            List<Post.Activity> activities = new List<Post.Activity>();
			var request = new RestRequest (ENGAGEMENT_API, Method.POST);
			request.AddParameter ("id", postID);
			request.AddParameter ("maxResults", 50);
			request.AddParameter ("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
			JArray root = JArray.Parse (_parseJSON (resp.Content));
			foreach (var element in root[0][2])
            {
				Post.Activity activity = new Post.Activity ();
				activity.name = element [0] [0].ToString ();
				activity.userID = element [0] [1].ToString ();
				activity.avatar = element [0] [4].ToString ();
				foreach (var type in element[1])
					activity.type.Add ((ActivityType)Convert.ToInt32 (type [0].ToString ()));
				activities.Add (activity);
			}
            return activities;
		}
		#endregion
		
		#region Media support
		
		//we need to make image JArray - getLinkDetails not work on images from Picasa/Plus
		public JArray _giveMeImage (string content, string userID, string albumID, string photoID, string imageURL, int x, int y)
		{
			JArray array = new JArray ();
			for (int i = 0; i < 14; i++) {
				array.Add (new JValue ((object)null));
			}
			string imageID = "https://plus.google.com/photos/" + userID + "/albums/" + albumID + "/" + photoID;
            array[0] = new JArray(imageID, null, content, imageURL.Replace("https:", String.Empty), null, null, null, x.ToString(), y.ToString());
			//https://plus.google.com/photos/113577538655080453733/albums/5893367887766972193/5893367892992535410?authkey=CN3K4uKI9pWwHg
			array[1] = userID;
			array[2] = albumID;
			array[3] = photoID;
			array [4] = false;
			array [6] = imageURL.Replace("https:", String.Empty);
            //String s = string.Replace("StringToReplace", "NewString");
			array [9] = imageID;
			string [] imageSplit = imageURL.Split('/');
			array [10] = imageSplit[imageSplit.Length - 1];
			array [13] = new JArray ();
			JArray finalArray = new JArray (
					new JArray(249, 35, 1, 0),
					null,
					null,
					null,
					null,
					null,
					new JObject(new JProperty ("27639957", array))
			);
			Console.WriteLine (finalArray.ToString ());
			return finalArray;
		}
		
		#endregion

		//------------------------ Public functions -------------------------
		
		#region Getters

		//get posts from the stream
		public async Task<Posts> GetActivities(string circleId, string personId, string pageToken)
		{
            Posts posts = new Posts();
            posts.posts = new ObservableCollection<Post>();
			string req = null; 
			var request = new RestRequest(ACTIVITIES_API, Method.POST);
            if (((circleId == null) && (personId == null)) ||
                    ((circleId != null) && (personId != null)))
                req = "null,null"; //show all posts
            if (circleId != null)
                req = "null,\"" + circleId + "\"";
            else if (personId != null)
                req = "\"" + personId + "\",null";
            else
                req = "null,null";
			request.AddParameter("f.req", "[[1,2," + req + ",null,null,null,\"social.google.com\",[],null,null,null,null,null,null,[]],\"" + pageToken + "\"]");
			request.AddParameter("at", _foundSession);

            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                //string result = _parseJSON(resp.Content); //super debugging kurwo xD
                JArray root = JArray.Parse (_parseJSON (resp.Content));
                JArray data = (JArray)root[0][switcher][1][0];
                foreach (var p in data)
                    posts.posts.Add(_parsePost(p));
                posts.pageToken = root[0][switcher][1][1].ToString();
                return posts;
            }
            else
            {
                MessageBox.Show("There was a problem while getting activities: " + resp.StatusDescription + ". You may have problem with internet connection or author is a faggot.\nEnsure that you have a properly configured internet connection OR take baseball and contact with developer.", "OP is a faggot", MessageBoxButton.OK);
                return null;
            }
		}

        private JArray _switch(JArray root)
        {
            if (switcher == 1)
                return (JArray)root[0][1];
            else
                return (JArray)root[0][0];
        }

        public async Task<Post> GetActivity(string userID, string postID)
		{
			string param = userID + "?updateId=" + postID;
			var request = new RestRequest(ACTIVITY_API + param + "&rt=j", Method.GET);
            var resp = await _client.GetResponseAsync(request);
			//string result = _parseJSON(resp.Content); //super debugging kurwo xD
			JArray root = JArray.Parse(_parseJSON(resp.Content));
            Post post = _parsePost(root[0][switcher][1]);
            //post.activities = await _getPostActivity(post.postID);
            return post;
		}
		
		//get basic info about circles to list
		public async Task<List<Circle>> GetCircles()
		{
			var request = new RestRequest (CIRCLE_API, Method.POST);
            request.AddParameter("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
			JArray root = JArray.Parse(_parseJSON(resp.Content));
            return _parseCircle((JArray)root[0]);
		}
		
		//get list of communities which you added
		public async Task<List<Community>> GetYourCommunities()
		{
            List<Community> communitiesList = new List<Community>();
			var request = new RestRequest (COMMUNITIES_API, Method.POST);
			request.AddParameter ("f.req", "[[1]]"); 
			request.AddParameter ("at", _foundSession);

            var resp = await _client.GetResponseAsync(request);
			JArray root = JArray.Parse(_parseJSON(resp.Content));
			JArray communities = (JArray)root[0][switcher][2];
			foreach (JArray community in communities)
				communitiesList.Add (_parseCommunity(community));
            return communitiesList;
		}
		
        /*
         * LANDING_API has been deleted
         * GetCommunity() is replaced by:
         * GetCommunity(communityID) - return only community with categories info
         * GetCommunityStream(communityID, categoryID) - return only posts
         */
        public async Task<Community> GetCommunity(string communityID)
        {
            ObservableCollection<Community.Category> categories = new ObservableCollection<Community.Category>();
            var request = new RestRequest (COMMUNITY_API, Method.POST);
            request.AddParameter ("f.req", "[\"" + communityID + "\", false]"); 
            request.AddParameter ("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
            //Console.WriteLine(_parseJSON(resp.Content));
            JArray root = JArray.Parse(_parseJSON(resp.Content));
            JArray communityData = (JArray)root[0][switcher][1][0];
            JArray categoriesData = (JArray)root[0][switcher][1][2][0];

            Community community = new Community();
            community.id = communityData[0].ToString();
            community.name = communityData[1][0].ToString();
            community.description = communityData[1][1].ToString();
            community.avatar = _parseLink(communityData[1][3].ToString());
            community.longDescription = communityData[1][8].ToString();

			foreach (JArray cat in categoriesData)
			{
				Community.Category category = new Community.Category();
				category.id = cat[0].ToString ();
				category.name = cat[1].ToString ();
				community.categories.Add(category);
			}
            return community;
        }


        public async Task<Posts> GetCommunityPosts(string communityID, string categoryID, string pageToken)
        {

            string cat = String.Empty;
            if (pageToken != null)
                cat = "\"" + pageToken + "\"";
            else cat = "null";
            if (categoryID != null)
                categoryID = "\",\"" + categoryID;
            Posts posts = new Posts();
            posts.posts = new ObservableCollection<Post>();
            var request = new RestRequest("https://plus.google.com/_/stream/squarestream?rt=j", Method.POST);
            //["108722991991342704083",null,false,[360,3,[]]]
            //["108722991991342704083",null,"CAIQz9TW1KOtuAIgKCgB",[520,1,[]]]
            //request.AddParameter("f.req", "[\"" + communityID + "\",null,false,null");
            //request.AddParameter("f.req", "[\"" + communityID + "\"," + cat + ",\"" + pageToken + "\",[520,1,[]]]");
            request.AddParameter("f.req", "[[null,null,[2,[null," + cat + "], null, null, false],null],[[\"" + communityID + categoryID + "\"],[8]]]");
            request.AddParameter("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
            JArray root = JArray.Parse (_parseJSON (resp.Content));
            JArray postsData = (JArray)root[0][switcher][1][0];
            foreach (JArray pos in postsData)
            {
                //if (pos.Count == 2)
                posts.posts.Add(_parsePost((JArray)pos));
            }
            posts.pageToken = root[0][switcher][1][1].ToString(); 
            return posts;
        }

        public async Task<Event> GetEvent(string eventID)
        {
            var request = new RestRequest(EVENT_API, Method.POST);
            request.AddParameter("f.req", "[\"" + eventID + "\"]");
            request.AddParameter("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                JArray root = JArray.Parse(_parseJSON(resp.Content));
                return _parseEvent(root[0][switcher][2][97].Last.First.First);
            }
            else return null;
        }

        //gets all profile stuff
		public async Task<User> GetProfile (string userID, bool getFollowers)
		{
            User user = new User();
            var request = new RestRequest(PROFILE_GET_API + userID + "?rt=j", Method.GET);
            var resp = await _client.GetResponseAsync(request);
			JArray root = JArray.Parse(_parseJSON(resp.Content));
			JArray data = (JArray)root[0][switcher][1];
            user = _parseUser(data);
            if (getFollowers == true)
            {
                user.incoming = await _getFollowersIncoming(userID);
                user.visible = await _getFollowersVisible(userID);
            }
            return user;
		}

        public async Task<ObservableCollection<AclItem>> GetAclItems()
        {
            ObservableCollection<AclItem> aclList = new ObservableCollection<AclItem>();
            aclList.Add(new AclItem(null, "Public", AclType.Public));
            aclList.Add(new AclItem(null, "Extended circles", AclType.ExtendedCircles));
            aclList.Add(new AclItem(null, "Your circles", AclType.YourCircles));
            List<Circle> circles = await GetCircles();
            foreach (var circle in circles)
                aclList.Add(new AclItem(circle.id, circle.name, AclType.SpecifiedCircle));
            return aclList;
        }
		
		#endregion
		
		#region Activities: posts, comments, +1
		
		//send new posts with media itepe
		public async Task<string> NewPost (string content, List<AclItem> aclList, string sharedPostID, JArray mediaArray, Tuple<string, string> community)
		{
			JArray array = new JArray ();
			for (int i = 0; i < 49; i++) {
				array.Add (new JValue ((object)null));
			}
			/*
				"latitudeE7":520552180,
				"locationTag":"E30, ³ódzkie",
				"longitudeE7":201293570
			*/
			array[0] = content;
			array[1] = "oz:" + _info.userID + "." + _getTicks().ToString ("x") + ".0";
			array[2] = sharedPostID; //shared post id
            array[6] = "[]";
			array[9] = false;
			array[10] = new JArray(); //notifyUser JArray(new JArray(null, userID))
			array[11] = false;
			array[14] = new JArray();
			array[16] = false;
			array[27] = false;
			array[28] = false;
			array[29] = false;
            array[34] = mediaArray;
            if (community == null)
            {
                array[36] = new JArray(); //new JArray(new JArray(communityID, communityCategory))
                array[37] = new JArray( //aclLists or community
                        new JArray(
                            new JArray(
                                _parseAclItems(aclList)
                                ),
                            null
                            )
                        );
            }
            else
            {
                var comArray = new JArray(community.Item1, community.Item2);
                array[36] = new JArray((object)new JArray(community.Item1, community.Item2));
                array[37] = new JArray(
                    (object)new JArray(
                        (object)new JArray(//[[[null,null,null,[postObj.community[0]]]]]
                            null,
                            null,
                            null,
                            (object)new JArray(community.Item1)
                        )
                    )
                );
                //array[37] = new JArray
            }
            array[48] = new JArray();
            //array[fajnie] = new JArray(520552180, "E30, łódzkie", 201293570);
            var request = new RestRequest(POST_API, Method.POST);
            request.AddParameter("f.req", array.ToString());
            request.AddParameter("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                JArray root = JArray.Parse(_parseJSON(resp.Content));
                JArray element = (JArray)root[0][switcher][1][0][0];
                var id = element[8].ToString();
                return id;
            }
            else return null;
        }
		
        public async Task<string> EditPost (string content, string postId)
        {
            var request = new RestRequest (EDIT_POST_API, Method.POST);
            request.AddParameter ("f.req", "[\"" + postId + "\",\"" + content + "\",null,\"[]\",true,[],null,null,null,false]");
            request.AddParameter ("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                JArray root = JArray.Parse(_parseJSON(resp.Content));
                return root[0][switcher][1][4].ToString();
            }
            else return null;
        }
		
        //add comment
        // - how about error handling?
        // - what is it? xD
        public async Task<int> AddComment (string content, string postID)
        {
            var data = new JArray(postID, "os:" + postID + ":" + _getTicks().ToString (), content, _getTicks(), null, null, 1);
            var request = new RestRequest (COMMENT_API + "?rt=j", Method.POST);
            request.AddParameter ("f.req", data.ToString()); 
            request.AddParameter ("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
                return 0;
            else return 1;
        }

        public async Task<int> EditComment(string commentID, string content)
        {
            var data = new JArray(null, commentID, content, null, null, 1);
            var request = new RestRequest (EDIT_COMMENT_API + "?rt=j", Method.POST);
            request.AddParameter ("f.req", data.ToString()); 
            request.AddParameter ("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
                return 0;
            else return 1;
        }
			
        //as named
        public async Task<int> RemoveComment (string commentID)
        {
            var request = new RestRequest (DELETE_COMMENT_API, Method.POST);
            request.AddParameter ("commentId", commentID); 
            request.AddParameter ("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
                return 0;
            else return 1;
        }

        //remove activity e.g. post
        public async Task<int> RemoveActivity (string activityID)
        {
            var request = new RestRequest (DELETE_ACTIVITY_API, Method.POST);
            request.AddParameter ("itemId", activityID); 
            request.AddParameter ("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
                return 0;
            else return 1;
        }

        //+1 or -1
        public async Task<int> PlusOne (string postID, bool type, string activityType)
        {
            //buzz or comment
            var request = new RestRequest (PLUS_API, Method.POST);
            request.AddParameter ("itemId", activityType + ":" + postID); 
            request.AddParameter ("set", type.ToString());
            request.AddParameter ("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
                return 0;
            else return 1;
        }
		
        public async Task<int> DisableComments (string postID, bool type)
        {
            var request = new RestRequest (DISABLE_COMMENTS_API, Method.POST);
            request.AddParameter ("itemId", postID); 
            request.AddParameter ("disable", type.ToString());
            request.AddParameter ("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
                return 0;
            else return 1;
        }
		
        public async Task<int> LockPost (string postID, bool type)
        {
            var request = new RestRequest (LOCK_POST_API, Method.POST);
            request.AddParameter ("itemId", postID); 
            request.AddParameter ("disable", type.ToString());
            request.AddParameter ("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
                return 0;
            else return 1;
        }
		
        public async Task<int> MuteActivity (string postID, bool type)
        {
            var request = new RestRequest (MUTE_ACTIVITY_API, Method.POST);
            request.AddParameter ("itemId", postID); 
            request.AddParameter ("mute", type.ToString());
            request.AddParameter ("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
                return 0;
            else return 1;
        }	

        #endregion
		
        #region Circles management
        public async Task<int> AddPerson(string circleID, string userID)
        {
            var request = new RestRequest (MODIFYMEMBER_MUTATE_API, Method.POST);
            request.AddParameter ("a", "[[[\"" + circleID + "\"]]]"); 
            request.AddParameter ("m", "[[[[null,null,\"" + userID + "\"],null,[]]]]");
            request.AddParameter ("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
                return 0;
            else return 1;
        }

        public async Task<int> RemovePerson(string circleID, string userID)
        {
            var request = new RestRequest (REMOVEMEMBER_MUTATE_API, Method.POST);
            request.AddParameter ("c", "[\"" + circleID + "\"]"); 
            request.AddParameter ("m", "[[[null,null,\"" + userID + "\"]]]");
            request.AddParameter ("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
                return 0;
            else return 1;
        }

        public async Task<string> CreateCircle(string name, string description)
        {
            var request = new RestRequest (CREATE_MUTATE_API, Method.POST);
            request.AddParameter ("t", "2"); 
            request.AddParameter ("n", name);
            request.AddParameter ("m", "[[]]");
            if (description != null)
                request.AddParameter("d", description);
            request.AddParameter ("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                string result = _parseJSON(resp.Content);
                JArray root = JArray.Parse(_parseJSON(resp.Content));
                return root[0][switcher][0].ToString();
            }
            else return null;
        }

        public async Task<int> RemoveCircle(string circleID)
        {
            var request = new RestRequest (DELETE_MUTATE_API, Method.POST);
            request.AddParameter ("c", "[" + circleID + "]"); 
            request.AddParameter ("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
                return 0;
            else return 1;
        }
        #endregion
		
        #region Searching

        public async Task<Posts> QueryPost(string query, SearchQuery queryOptions, string pageToken, string longPageToken) //TODO: pageToken
        {
            Posts posts = new Posts();
            posts.posts = new ObservableCollection<Post>();
            //query - simple
            //rt - realtime - we don't need it ;)
            //[["siema",1,1,[1]],null]]
            var json = new JArray (
                            new JArray (
                                query,
                                queryOptions.type, //type
                                queryOptions.category, //category
                                new JArray (
                                    queryOptions.privacy //privacy
                                )
                            ),
                            null);
            //new JArray(pageToken));
            if (pageToken != null)
                json.Add(new JArray(pageToken, null, null, null, null, null, longPageToken));
            //else
             //   json.Add((object)null);
            var request = new RestRequest (QUERY_API + "query?rt=j", Method.POST);
            request.AddParameter ("f.req", json.ToString());
            request.AddParameter ("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
            JArray root = JArray.Parse (_parseJSON (resp.Content));
            foreach (var element in root[0][switcher][1][1][0][0])
            //{
            //    var post = await GetActivity(element[16].ToString(), element[8].ToString());
            //    posts.posts.Add(post);
            //}
                posts.posts.Add(_parsePost(element));
            posts.pageToken = root[0][switcher][1][1][2].ToString();
            posts.longPageToken = root[0][switcher][1][1][5].ToString();
            return posts;
        }

        public async Task<SearchUsers> QueryUser(string query, string pageToken)
        {
            SearchUsers users = new SearchUsers();
            var json = new JArray(
                new JArray(
                    query,
                    SearchType.PeoplePages, //type
                    SearchCategory.Best, //category
                    new JArray(
                        SearchPrivacy.Everyone //privacy
                    )
                ),
                null,
                null);
            //new JArray(pageToken));
            if (pageToken != null) json[1] = new JArray(pageToken);
            var request = new RestRequest(QUERY_API + "query?rt=j", Method.POST);
            request.AddParameter("srchrp", json.ToString());
            request.AddParameter("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                JArray root = JArray.Parse(_parseJSON(resp.Content));
                foreach (var u in root[0][switcher][1][0][0])
                {
                    UserInfo user = new UserInfo();
                    user.userID = u[0][2].ToString();
                    user.name = u[1][0].ToString();
                    user.avatar = _parseLink(u[1][8].ToString());
                    users.users.Add(user);
                }
                users.pageToken = root[0][switcher][1][0][2].ToString();
            }
            return users;
        }
        public async Task<SearchCommunities> QueryCommunity(string query, string pageToken)
        {
            SearchCommunities communities = new SearchCommunities();
            var json = new JArray(
                new JArray(
                    query,
                    SearchType.Communities, //type
                    SearchCategory.Best, //category
                    new JArray(
                        SearchPrivacy.Everyone //privacy
                    )
                ),
                null,
                null);
            //new JArray(pageToken));
            if (pageToken != null) json[1] = new JArray(pageToken);
            var request = new RestRequest(QUERY_API + "query?rt=j", Method.POST);
            request.AddParameter("srchrp", json.ToString());
            request.AddParameter("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                JArray root = JArray.Parse(_parseJSON(resp.Content));
                foreach (var c in root[0][switcher][1][7][0])
                {
                    Community community = new Community();
                    community.id = c[0][0].ToString();
                    community.name = c[1].ToString();
                    community.description = c[7].ToString();
                    community.avatar = _parseLink(c[2].ToString());
                    community.postsCount = (int)c[6];
                    community.userCount = (int)c[5];
                    communities.communities.Add(community);
                }
                communities.pageToken = root[0][switcher][1][7][2].ToString();
            }
            return communities;
        }

        #endregion
		
        #region Other
        //untested, i'm afraid reporting some profiles ;/
        public void ReportProfile (string userId, AbuseReason reason) 
        {
            var request = new RestRequest (PROFILE_REPORT_API, Method.POST);
            request.AddParameter ("itemId", userId); 
            request.AddParameter ("userinfo", "[1]");
            request.AddParameter ("abuseReason", reason);
            request.AddParameter ("at", _foundSession);
            _client.ExecuteAsync (request, (resp) =>
                    {
                        Console.WriteLine (_parseJSON (resp.Content));
                    }
            );
        }
		
        //notifications ehe
        public async Task<Notify> ShowNotification () //prototype
        {
            var request = new RestRequest (NOTIFICATION_API, Method.POST);
            /*
             * 2 - dodali Cie do znajomych - added You to friends
             * 4 - wydarzenia - 
             * 9 + pierdy - maly panel - small notifications //null,[],5,null,[],null,true,[],null,null,null,null,2,null,null,null,[9]] 
             * 11 - moja aktywnosc - your activity
             * 12 - wzmianki o mnie - wow, they talk about me!
             * 14 - wpisy innych osob - your circles people posts
             * 18 - wszystkie wpisy - all posts
             */
            //request.AddParameter ("f.req", "[null,[],6,null,[],null,null,[],null,null,15,null,null,null,null,null,[18]]");
            request.AddParameter ("f.req", "[null,[],5,null,[],null,true,[],null,null,null,null,2,null,null,null,[9]]"); 
			request.AddParameter ("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
            int result = await _updateLastReadTime();
			JArray root = JArray.Parse(_parseJSON(resp.Content));
			return _parseNotify((JArray)root[0][switcher]);
            
		}

        public async Task<ObservableCollection<Notification>> GetUnreadNotifications()
        {
            ObservableCollection<Notification> notifications = new ObservableCollection<Notification>();
            var request = new RestRequest(NOTIFICATIONS_FETCH_API, Method.POST);
            request.AddParameter("f.req", "[[\"OGB\"],[null,null,10,[],[1],null,\"GPLUS_APP\",[3]]]");
            request.AddParameter("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
            JArray root = JArray.Parse(_parseJSON(resp.Content));
            foreach (var n in root[0][switcher][1])
            {
                Notification notification = new Notification();
                notification.id = n[0].ToString();
                notification.title = n[4][0][0][2].ToString();
                notification.description = n[4][0][0][3].ToString();
                notification.url = n[4][0][2][2].ToString();
                foreach (var t in n[6])
                {
                    switch ((int)t[0])
                    {
                        case 2: notification.eventID = t[1].ToString();
                            break;
                        case 1: notification.postID = t[1].ToString();
                            if (n[5].HasValues)
                            {
                                string[] url = n[5][0][1].ToString().Split(':');
                                notification.userID = url[0];
                            }
                            break;
                        case 7: notification.userID = t[1].ToString();
                            break;
                        case 4: notification.communityID = t[1].ToString();
                            break;
                    }
                }
                notifications.Add(notification);
            }
            int i = await _updateLastViewedVersion();
            return notifications;
        }


        public async Task<int> SetReadState(string notificationID)
        {
            var request = new RestRequest(NOTIFICATION_READ_API, Method.POST);
            //
            request.AddParameter("f.req", "[[\"OGB\"],[[[\"" + notificationID + "\",null," + _getTicks()*1000 + "]],2]]");
            request.AddParameter("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
                return 0;
            else return 1;
        }

        public async Task<int> GetUnreadNotificationsCount()
        {
            /*
            var request = new RestRequest(NOTIFICATION_API, Method.POST);
            request.AddParameter("f.req", "[null,[],5,null,[],null,true,[],null,null,null,null,2,null,null,null,[9]]");
            request.AddParameter("at", _foundSession);
            var resp = await _client.GetResponseAsync(request); 
            JArray root = JArray.Parse(_parseJSON(resp.Content));
            return (int)root[0][1][7
             */
            var request = new RestRequest(NOTIFICATIONS_COUNT_API, Method.POST);
            request.AddParameter("f.req", "[[\"OGB\"],[[],[1],[3],100,\"GPLUS_APP\"]]");
            //[["OGB"],[[],[1],[3],100,"GPLUS_APP"]]
            request.AddParameter("at", _foundSession);
            var resp = await _client.GetResponseAsync(request);
            JArray root = JArray.Parse(_parseJSON(resp.Content));
            int result;
            try
            {
                result = (int)root[0][switcher][1];
            }
            catch
            {
                result = (int)root[0][1][1];
            }
            return result;
        }

        public bool IsConnected()
        {
            if (_foundSession != null)
                return true;
            else return false;
        }

		#endregion

        #region Media support

        public async Task<JArray> UploadPhoto(Stream stream, string fileName)
        {
            string data = string.Empty;
            JObject jsonObject = new JObject();
            #region Very LONG json
            JArray array = new JArray(
                new JObject(
                    new JProperty("external", 
                        new JObject(
                            new JProperty("name", "file"),
                            new JProperty("filename", Path.GetFileName(fileName)),
                            new JProperty("put", new JObject()),
                            new JProperty("size", stream.Length)
                            )
                        )
                    ),
                 new JObject(
                    new JProperty("inlined", 
                        new JObject(
                            new JProperty("name", "use_upload_size_pref"),
                            new JProperty("content", "true"),
                            new JProperty("contentType", "text/plain")
                            )
                        )
                    ),
                new JObject(
                    new JProperty("inlined", 
                        new JObject(
                            new JProperty("name", "batchid"),
                            new JProperty("content", _getTicks().ToString()),
                            new JProperty("contentType", "text/plain")
                            )
                        )
                    ),
                new JObject(
                    new JProperty("inlined", 
                        new JObject(
                            new JProperty("name", "client"),
                            new JProperty("content", "sharebox"),
                            new JProperty("contentType", "text/plain")
                            )
                        )
                    ),
                new JObject(
                    new JProperty("inlined", 
                        new JObject(
                            new JProperty("name", "disable_asbe_notification"),
                            new JProperty("content", "true"),
                            new JProperty("contentType", "text/plain")
                            )
                        )
                    ),
                new JObject(
                    new JProperty("inlined", 
                        new JObject(
                            new JProperty("name", "album_mode"),
                            new JProperty("content", "temporary"),
                            new JProperty("contentType", "text/plain")
                            )
                        )
                    ),
                new JObject(
                    new JProperty("inlined", 
                        new JObject(
                            new JProperty("name", "album_abs_position"),
                            new JProperty("content", "0"),
                            new JProperty("contentType", "text/plain")
                            )
                        )
                    )
            );
            #endregion
            jsonObject.Add("protocolVersion", "0.8");
            jsonObject.Add("createSessionRequest", new JObject(new JProperty("fields", array)));
            var request = new RestRequest(UPLOAD_PHOTO_API, Method.POST);
            request.AddParameter("application/json; charset=utf-8", jsonObject.ToString(), ParameterType.RequestBody);
            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                JObject root = JObject.Parse(resp.Content);
                /*
                 * it's is nessesary to use native Silverlight libraries (HttpWebRequest)
                 * because RestSharp cannot send binary files without multipart
                 */
                HttpWebRequest sendRequest = (HttpWebRequest)HttpWebRequest.Create(new Uri(UPLOAD_PHOTO_API + "?upload_id=" + root["sessionStatus"]["upload_id"].ToString() + "&file_id=000"));
                sendRequest.Method = "POST";
                sendRequest.ContentType = "application/octet-stream";
                sendRequest.CookieContainer = _client.CookieContainer;
                HttpWebResponse response = await sendRequest.GetRequestStreamAsync(stream);
                System.IO.Stream responseStream = response.GetResponseStream();
                using (var reader = new System.IO.StreamReader(responseStream))
                {
                    data = reader.ReadToEnd();
                }
                responseStream.Close();
                JObject resultObject = JObject.Parse(data);
                JObject info = (JObject)resultObject["sessionStatus"]["additionalInfo"]["uploader_service.GoogleRupioAdditionalInfo"]["completionInfo"]["customerSpecificInfo"];
                JArray result = _giveMeImage(
                    //(string)info["mimeType"],
                    "",
                    (string)info["username"],
                    (string)info["albumid"],
                    (string)info["photoid"],
                    (string)info["url"],
                    (int)info["width"],
                    (int)info["height"]);
                return result;
            }
            else return null;
        }

        //it provide cool creating thumbnails of sites
        public async Task<JArray> GetLinkDetails(string url)
        {
            var request = new RestRequest(LINK_DETAILS_API, Method.POST);
            //request.AddParameter ("susp", false);
            request.AddParameter("at", _foundSession);
            request.AddParameter("f.req", "[\"" + url + "\",false,false,null,null,null,null,null,null,null,null,null,true]");
            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                JArray root = JArray.Parse(_parseJSON(resp.Content));
                //JArray element = (JArray)root[0][1][1][0][0];
                //var id = element[8].ToString();
                //JArray result = new JArray(
                //    root[0][5][0][0],
                //    root[0][5][0][1],
                //    null,
                //    null,
                //    null,
                //    null,
                //    root[0][5][0][2]);
                if (root[0][switcher][5].HasValues) return (JArray)root[0][switcher][5][0];
                else return null;
            }
            else return null;
        }

        #endregion

        public bool isConnected()
        {
            if (_foundSession != null)
            {
                Regex tagRegex = new Regex(@"<[^>]+>");
                return !tagRegex.IsMatch(_foundSession);
            }
            else return false;
        }

        #region Events

        /*
        public int InviteUser(string userID, string eventID)
        {
            //["111399666922347055972","c229kcknjov1lbfm8vpt11ae9f4",null,[[[[null,null,"113577538655080453733"]]]],0,["c229kcknjov1lbfm8vpt11ae9f4","111399666922347055972","CNyKkoGW27rkAg"]]
        */

        public async Task<int> ReportPresence(string eventID, Poll poll, string tokenID)
        {
            var request = new RestRequest(RSVP_API, Method.POST);
            //request.AddParameter ("susp", false);
            request.AddParameter("at", _foundSession);
            request.AddParameter("f.req", "[null," + (int)poll + ",\"" + eventID + "\",null,null,2,null,null,[\"" + eventID + "\",\"" + this.info.userID + "\",\"" + tokenID + "\"]]");
            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
                return 0;
            else return 1;
        }

        public async Task<int> ReportGuestsPresence(string eventID, int count)
        {
            var request = new RestRequest(RSVP_API, Method.POST);
            //request.AddParameter ("susp", false);
            request.AddParameter("at", _foundSession);
            request.AddParameter("f.req", "[null,null,\"" + eventID + "\",null,null,null,null," + count.ToString() + "]");
            var resp = await _client.GetResponseAsync(request);
            if (resp.StatusCode == HttpStatusCode.OK)
                return 0;
            else return 1;
        }


        #endregion
    }
}

//FIN DE SIECLE