using Footprints.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Footprints.ViewModels
{
    public class JourneyViewModel
    {
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; }
        public Guid JourneyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime TakenDate { get; set; }
        public int NumberOfLikes { get; set; }
        public int NumberOfShares { get; set; }
        public int NumberOfDestinations { get; set; }
        public int NumberOfPhotos { get; set; }
        public CoverPhoto CoverPhoto { get; set; }
        public IEnumerable<Footprints.Models.Destination> Destinations { get; set; }
        public IEnumerable<CommentViewModel> Comments { get; set; }
        public string TimeAgo
        {
            get { return DateTimeFormat.TimeAgo(this.TakenDate); }
            private set { }
        }
        public AddNewDestinationFormViewModel AddNewDestinationFormViewModel { get; set; }
        public static JourneyViewModel GetSampleObject(Guid JourneyID)
        {
            return new JourneyViewModel
            {
                AuthorId = new Guid(),
                AuthorName = "Author Name",
                JourneyId = JourneyID,
                Name = "Journey Name",
                Description = "Journey Description",
                TakenDate = DateTime.Now,
                NumberOfLikes = 21,
                NumberOfShares = 4,
                NumberOfDestinations = 10,
                NumberOfPhotos = 200,
                CoverPhoto = new CoverPhoto
                {
                    PhotoId = new Guid(),
                    PhotoUrl = "https://s3-ap-southeast-1.amazonaws.com/elasticbeanstalk-ap-southeast-1-01156/user_123456/album_12345/avt.JPG"
                },
                Comments = CommentViewModel.GetSampleObject(),
                AddNewDestinationFormViewModel = AddNewDestinationFormViewModel.GetEmptyObject(JourneyID)
            };
        }
    }

    public class AddNewJourneyViewModel {
        public Guid JourneyID { get; set; }

        public Guid UserID { get; set; }

        public String Name { get; set; }

        public String Description { get; set; }

        [Display(Name = "Time")]
        public DateTimeOffset TakenDate { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public int NumberOfLike { get; set; }

        public static AddNewJourneyViewModel GetSampleObject()
        {
            return new AddNewJourneyViewModel
            {
                JourneyID = new Guid(Guid.NewGuid().ToString("N")),
                UserID = new Guid(Guid.NewGuid().ToString("N")),
                Name = "",
                Description = "",
                TakenDate = DateTimeOffset.Now,
                NumberOfLike = 0
            };
        }
    };
    public class CoverPhoto
    {
        public Guid PhotoId { get; set; }
        public String PhotoUrl { get; set; }
    }

}