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
        public Guid AuthorID { get; set; }
        public String AuthorName { get; set; }
        public int NumberOfJourneys { get; set; }
        public int NumberOfDestinations { get; set; }
        public int NumberOfFriends { get; set; }
        public Guid DestinationID { get; set; }
        public Guid JourneyID { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public Footprints.Models.Place Place { get; set; }
        public int NumberOfLikes { get; set; }
        public int NumberOfShares { get; set; }
        public int NumberOfPhotos { get; set; }
        public IEnumerable<Content> Contents { get; set; }

        public IEnumerable<CommentViewModel> Comments { get; set; }
        public DateTimeOffset TakenDate { get; set; }
        public EditDestinationFormViewModel EditDestinationForm { get; set; }
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
                AuthorID = Guid.NewGuid(),
                AuthorName = "Hùng Vi",
                NumberOfJourneys = 22,
                NumberOfDestinations = 120,
                NumberOfFriends = 200,
                DestinationID = DestinationID,
                JourneyID = JourneyID,
                Name = "Trường đại học FPT",
                TakenDate = DateTimeOffset.Now,
                NumberOfLikes = 13,
                NumberOfShares = 2,
                NumberOfPhotos = 20,
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
                    new CommentViewModel{
                        UserCommentId = Guid.NewGuid(),
                        NumberOfLike = 12,
                        Content = "this is a sample content",
                        Time = DateTimeOffset.Now,
                        UserCommentName = "Nhân Trịnh",
                        UserAvatarURL = "../assets/images/people/80/2.jpg"
                    }
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
                }
            };
        }
    }

    public class AddNewDestinationFormViewModel
    {
        [RegularExpression(Common.Constant.GUID_REGEX)]
        public Guid DestinationID { get; set; }
        [Required]
        [RegularExpression(Common.Constant.GUID_REGEX)]
        public String PlaceID { get; set; }
        [Required]
        [RegularExpression(Common.Constant.GUID_REGEX)]
        public Guid JourneyID { get; set; }
        [Required]
        public Double Longitude { get; set; }
        public Double Latitude { get; set; }
        public String Reference { get; set; }
        [Required]
        public String Name { get; set; }
        [DataType(DataType.MultilineText)]
        public String Description { get; set; }
        [Display(Name = "Date")]

        [Required]
        [DataType(DataType.Date)]
        public DateTimeOffset TakenDate { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public int NumberOfLikes { get; set; }

        public static AddNewDestinationFormViewModel GetEmptyObject(Guid JourneyID)
        {
            return new AddNewDestinationFormViewModel
            {
                JourneyID = JourneyID,
                TakenDate = DateTimeOffset.Now
            };
        }
    }

    public class EditDestinationFormViewModel
    {
        [Required]
        [RegularExpression(Common.Constant.GUID_REGEX)]
        public Guid DestinationID { get; set; }
        [Required]
        [RegularExpression(Common.Constant.GUID_REGEX)]
        public String PlaceID { get; set; }
        [Required]
        [RegularExpression(Common.Constant.GUID_REGEX)]
        public Guid JourneyID { get; set; }
        [Required]
        public Double Longitude { get; set; }
        [Required]
        public Double Latitude { get; set; }
        public String Reference { get; set; }
        [Required(ErrorMessage = "You must enter destination name")]
        public String Name { get; set; }
        [DataType(DataType.MultilineText)]
        public String Description { get; set; }
        [Display(Name = "Time")]
        [Required]
        [DataType(DataType.Date)]
        public DateTimeOffset TakenDate { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public int NumberOfLikes { get; set; }

        public string TimeAgo
        {
            get { return DateTimeFormat.TimeAgo(this.TakenDate); }
            private set { }
        }
        public static AddNewDestinationFormViewModel GetEmptyObject(Guid JourneyID)
        {
            return new AddNewDestinationFormViewModel
            {
                JourneyID = JourneyID,
                TakenDate = DateTimeOffset.Now
            };
        }
    }
}