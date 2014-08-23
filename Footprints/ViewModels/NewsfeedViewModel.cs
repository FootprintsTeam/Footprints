using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.Common;
using Footprints.Models;

namespace Footprints.ViewModels
{
    public class NewsfeedBaseWidgetViewModel
    {
        public Guid UserID { get; set; }
        public string UserName { get; set; }
        public String ProfilePicURL { get; set; }
        public Guid ActivityID { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public int NumberOfLike { get; set; }
        public int NumberOfShare { get; set; }
        public IList<CommentViewModel> Comments { get; set; }
        public string Type { get; set; }
    }

    public class AddPhotoWidgetViewModel : NewsfeedBaseWidgetViewModel
    {
        public int NumberOfPhoto { get; set; }
        public Guid DestinationID { get; set; }
        public string DestinationName { get; set; }
        public string URL { get; set; }
        public static AddPhotoWidgetViewModel GetSampleObject()
        {
            var sample = new AddPhotoWidgetViewModel
            {
                ActivityID = new Guid(),
                UserName = "Nhân Trịnh",
                ProfilePicURL = Constant.DEFAULT_AVATAR_URL,
                Timestamp = DateTimeOffset.Now,
                DestinationID = Guid.NewGuid(),
                DestinationName = "Phố cổ Hà Nội",
                Comments = CommentViewModel.GetSampleObject(),
                NumberOfLike = Constant.DefaultNumberOfShare,
                NumberOfShare = Constant.DefaultNumberOfLike
            };

            return sample;
        }
    }

    public class CommentWidgetViewModel : NewsfeedBaseWidgetViewModel
    {
        public Guid DestinationID { get; set; }
        public string DestinationName { get; set; }
        public string Content { get; set; }
        public static CommentWidgetViewModel GetSampleObject()
        {
            var sample = new CommentWidgetViewModel
            {
                ActivityID = new Guid(),
                UserName = "Nhân Trịnh",
                ProfilePicURL = Constant.DEFAULT_AVATAR_URL,
                Timestamp = DateTimeOffset.Now,
                DestinationID = Guid.NewGuid(),
                DestinationName = "Phố cổ Hà Nội",
                Comments = CommentViewModel.GetSampleObject(),
                NumberOfLike = Constant.DefaultNumberOfShare,
                NumberOfShare = Constant.DefaultNumberOfLike,
                Content = CommentViewModel.GetSampleObject().First().Content
            };
            return sample;
        }
    }

    public class ShareWidgetViewModel : NewsfeedBaseWidgetViewModel
    {
        public Guid DestinationID { get; set; }
        public string DestinationName { get; set; }
        public Guid JourneyID { get; set; }
        public string JourneyName { get; set; }
        public string Content { get; set; }
        public static ShareWidgetViewModel GetSampleObject() {
            var sample = new ShareWidgetViewModel
            {
                ActivityID = new Guid(),
                UserName = "Nhân Trịnh",
                ProfilePicURL = Constant.DEFAULT_AVATAR_URL,
                Timestamp = DateTimeOffset.Now,
                DestinationID = Guid.NewGuid(),
                DestinationName = "Phố cổ Hà Nội",
                Comments = CommentViewModel.GetSampleObject(),
                NumberOfLike = Constant.DefaultNumberOfShare,
                NumberOfShare = Constant.DefaultNumberOfLike,
                Content = CommentViewModel.GetSampleObject().First().Content
            };
            return sample;
        }
    }

    public class PersonalWidgetViewModel : NewsfeedBaseWidgetViewModel
    {
        public int NumberOfJourney { get; set; }
        public int NumberOfDestination { get; set; }
        public int NumberOfFriend { get; set; }
        public DateTimeOffset JoinDate { get; set; }
        public static PersonalWidgetViewModel GetSampleObject() {
            var sample = new PersonalWidgetViewModel {
                ActivityID = new Guid(),
                UserName = "Nhân Trịnh",
                ProfilePicURL = Constant.DEFAULT_AVATAR_URL,
                Timestamp = DateTimeOffset.Now,
                NumberOfJourney  = 10,
                NumberOfDestination = 10,
                NumberOfFriend = 100,
                JoinDate = DateTimeOffset.Now
            };

            return sample;
        }
    }

    public class AddFriendWidgetViewmodel : NewsfeedBaseWidgetViewModel {
        public string FriendName { get; set; }
        public string FriendUserID { get; set; }
        public string FriendProfilePicUrl { get; set; }
        public static AddFriendWidgetViewmodel GetSampleObject()
        {
            var sample = new AddFriendWidgetViewmodel
            {
                ActivityID = new Guid(),
                UserName = "Nhân Trịnh",
                ProfilePicURL = Constant.DEFAULT_AVATAR_URL,
                Timestamp = DateTimeOffset.Now               
            };

            return sample;
        }
    }

    public class DestinationWidgetViewModel : NewsfeedBaseWidgetViewModel
    {
        public Place Place { get; set; }
        public string DestinationName { get; set; }
        public string Description { get; set; }
        public Guid DestinationID { get; set; }
        public static DestinationWidgetViewModel GetSampleObject() {
            var sample = new DestinationWidgetViewModel
            {
                ActivityID = new Guid(),
                UserName = "Nhân Trịnh",
                ProfilePicURL = Constant.DEFAULT_AVATAR_URL,
                Timestamp = DateTimeOffset.Now,
                DestinationID = Guid.NewGuid(),
                DestinationName = "Phố cổ Hà Nội",
                Comments = CommentViewModel.GetSampleObject(),
                NumberOfLike = Constant.DefaultNumberOfShare,
                NumberOfShare = Constant.DefaultNumberOfLike,
                Place = new Place {
                    Latitude = -15.800513,
                    Longitude = -47.91378,
                    PlaceID = Guid.NewGuid().ToString(),
                    Name = "something"
                },
                Description = "Some description"
            };

            return sample;
        }
    }

    public class JourneyWidgetViewModel : NewsfeedBaseWidgetViewModel {
        public string JourneyName { get; set; }
        public string Description { get; set; }
        public Guid JourneyID { get; set; }
        public static JourneyWidgetViewModel GetSampleObject() {
            var sample = new JourneyWidgetViewModel
            {
                ActivityID = new Guid(),
                UserName = "Nhân Trịnh",
                ProfilePicURL = Constant.DEFAULT_AVATAR_URL,
                Timestamp = DateTimeOffset.Now,
                JourneyID = Guid.NewGuid(),
                JourneyName = "Phố cổ Hà Nội",                
                NumberOfLike = Constant.DefaultNumberOfShare,
                NumberOfShare = Constant.DefaultNumberOfLike,                
                Description = "Some description"
            };

            return sample;
        }
    }
}