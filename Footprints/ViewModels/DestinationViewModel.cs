using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Footprints.Common;
using Footprints.Models;

namespace Footprints.ViewModels
{
    public class DestinationViewModel
    {
        public Guid UserID { get; set; }
        public string ProfilePicURL { get; set; }
        public String CoverPhotoUrl { get; set; }
        public String UserName { get; set; }
        public int NumberOfJourney { get; set; }
        public int NumberOfDestination { get; set; }
        public int NumberOfFriend { get; set; }
        public Guid DestinationID { get; set; }
        public Guid AlbumID { get; set; }
        public Guid JourneyID { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public Footprints.Models.Place Place { get; set; }
        public int NumberOfLike { get; set; }
        public int NumberOfShare { get; set; }
        public int NumberOfPhoto { get; set; }
        public IList<Content> Contents { get; set; }

        public IList<CommentViewModel> Comments { get; set; }
        public DateTimeOffset TakenDate { get; set; }
        public EditDestinationFormViewModel EditDestinationForm { get; set; }
        public AddCommentFormViewModel AddCommentForm { get; set; }
        public string TimeAgo
        {
            get { return DateTimeFormat.TimeAgo(this.TakenDate); }
            private set { }
        }

        public static DestinationViewModel GetSampleObject()
        {
            var DestinationID = Guid.NewGuid();
            var JourneyID = Guid.NewGuid();
            var Place = new Models.Place
            {
                PlaceID = "ChIJbQilLLNUNDER5Der2CkuxqM",
                Latitude = 21.028529,
                Longitude = 105.78225999999995,
                Name = "Some place"
            };

            return new DestinationViewModel
            {
                UserID = Guid.NewGuid(),
                UserName = "Hùng Vi",
                NumberOfJourney = 22,
                NumberOfDestination = 120,
                NumberOfFriend = 200,
                DestinationID = DestinationID,
                JourneyID = JourneyID,
                Name = "Trường đại học FPT",
                TakenDate = DateTimeOffset.Now,
                NumberOfLike = 13,
                NumberOfShare = 2,
                NumberOfPhoto = 20,
                Contents = new List<Content> {
                    new Content {
                        ContentID = Guid.NewGuid(),
                        URL = "../assets/images/gallery-2/1.jpg"
                    },
                    new Content {
                        ContentID = Guid.NewGuid(),
                        URL = "../assets/images/gallery-2/2.jpg"
                    },
                    new Content {
                        ContentID = Guid.NewGuid(),
                        URL = "../assets/images/gallery-2/3.jpg"
                    },
                    new Content {
                        ContentID = Guid.NewGuid(),
                        URL = "../assets/images/gallery-2/4.jpg"
                    },
                    new Content {
                        ContentID = Guid.NewGuid(),
                        URL = "../assets/images/gallery-2/5.jpg"
                    },
                    new Content {
                        ContentID = Guid.NewGuid(),
                        URL = "../assets/images/gallery-2/6.jpg"
                    }
                },
                Comments = new List<CommentViewModel>{
                    CommentViewModel.GetSampleObject().First(),
                    CommentViewModel.GetSampleObject().First(),
                    CommentViewModel.GetSampleObject().First(),
                    CommentViewModel.GetSampleObject().First()
                },
                Description = @"Lorem ipsum dolor sit amet, consectetur adipisicing elit. Repellendus, aspernatur ut ....",
                Place = Place,
                EditDestinationForm = new EditDestinationFormViewModel
                {
                    DestinationID = DestinationID,
                    JourneyID = JourneyID,
                    PlaceID = Place.PlaceID,
                    Latitude = Place.Latitude,
                    Longitude = Place.Longitude,
                    Name = "Trường đại học FPT",
                    Description = @"This is contentThis is contentThis is contentThis is contentThis is contentThis is contentThis is contentThis is contentThis is contentThis is contentThis is contentThis is contentThis is contentThis is contentThis is contentThis is content",
                    TakenDate = DateTimeOffset.Now
                },
                AddCommentForm = new AddCommentFormViewModel
                {
                    DestinationID = DestinationID,
                    Content = ""
                }
            };
        }
    }

    public class AddNewDestinationFormViewModel
    {
        [Required(ErrorMessage = "Please choose a place on the map")]
        public String PlaceID { get; set; }
        [Required(ErrorMessage = "Please choose a place on the map")]
        public String PlaceName { get; set; }
        public String Address { get; set; }
        [Required]
        [RegularExpression(Common.Constant.GUID_REGEX)]
        public Guid JourneyID { get; set; }
        [Required(ErrorMessage = "Please choose a place on the map")]
        public Double Longitude { get; set; }
        [Required(ErrorMessage = "Please choose a place on the map")]
        public Double Latitude { get; set; }
        public String Reference { get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public String Description { get; set; }
        [Display(Name = "Start Date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTimeOffset TakenDate { get; set; }
        public static AddNewDestinationFormViewModel GetEmptyObject(Guid JourneyID)
        {
            return new AddNewDestinationFormViewModel
            {
                JourneyID = JourneyID,
                TakenDate = DateTimeOffset.Now
            };
        }
    }

    public class EditDestinationFormViewModel : Place
    {
        public EditDestinationFormViewModel(Place place)
        {
            this.PlaceID = place.PlaceID;
            this.PlaceName = place.Name;
            this.Longitude = place.Longitude;
            this.Latitude = place.Latitude;
            this.Address = place.Address;
            this.Reference = place.Reference;
        }
        public EditDestinationFormViewModel()
        {
        }
        [Required]
        [RegularExpression(Common.Constant.GUID_REGEX)]
        public Guid DestinationID { get; set; }
        [Required(ErrorMessage = "Please choose a place on the map")]
        public String PlaceName { get; set; }
        [Required]
        [RegularExpression(Common.Constant.GUID_REGEX)]
        public Guid JourneyID { get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public String Description { get; set; }
        [Display(Name = "Time")]
        [Required]
        [DataType(DataType.Date)]
        public DateTimeOffset TakenDate { get; set; }
        public DateTimeOffset Timestamp { get; set; }

        public string TimeAgo
        {
            get { return DateTimeFormat.TimeAgo(this.TakenDate); }
            private set { }
        }
    }
    public class DeleteDestinationFormViewModel
    {
        [Required]
        [RegularExpression(Common.Constant.GUID_REGEX)]
        public Guid DestinationID { get; set; }
        [Required]
        [RegularExpression(Common.Constant.GUID_REGEX)]
        public Guid JourneyID { get; set; }
    }

    public class AddCommentFormViewModel
    {
        [Required]
        [RegularExpression(Common.Constant.GUID_REGEX)]
        public Guid DestinationID { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public String Content { get; set; }
        public static AddCommentFormViewModel GetEmptyModel(Guid DestinationID)
        {
            return new AddCommentFormViewModel
            {
                DestinationID = DestinationID,
                Content = ""
            };
        }
    }

    public class EditCommentFormViewModel
    {
        [Required]
        [RegularExpression(Common.Constant.GUID_REGEX)]
        public Guid CommentID { get; set; }
        [Required]
        [RegularExpression(Common.Constant.GUID_REGEX)]
        public Guid DestinationID { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public String Content { get; set; }
    }

    public class DeleteCommentFormViewModel
    {
        [Required]
        [RegularExpression(Common.Constant.GUID_REGEX)]
        public Guid CommentID { get; set; }
        [Required]
        [RegularExpression(Common.Constant.GUID_REGEX)]
        public Guid DestinationID { get; set; }
    }
}