using Footprints.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Footprints.Models;
namespace Footprints.ViewModels
{
    public class JourneyViewModel
    {
        public Guid UserID { get; set; }
        public string AuthorName { get; set; }
        public Guid JourneyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset TakenDate { get; set; }
        public int NumberOfLike { get; set; }
        public int NumberOfShare { get; set; }
        public int NumberOfDestination { get; set; }
        public int NumberOfPhoto { get; set; }
        public CoverPhoto CoverPhoto { get; set; }
        public IList<DestinationViewModel> Destinations { get; set; }
        public IList<CommentViewModel> Comments { get; set; }
        public string TimeAgo
        {
            get { return DateTimeFormat.TimeAgo(this.TakenDate); }
            private set { }
        }
        public AddNewDestinationFormViewModel AddNewDestinationFormViewModel { get; set; }
        public static JourneyViewModel GetSampleObject()
        {
            return new JourneyViewModel
            {
                UserID = Guid.NewGuid(),
                AuthorName = "Author Name",
                JourneyId = Guid.NewGuid(),
                Name = "Journey Name",
                Description = "Journey Description",
                TakenDate = DateTime.Now,
                NumberOfLike = 21,
                NumberOfShare = 4,
                NumberOfDestination = 10,
                NumberOfPhoto = 200,
                CoverPhoto = new CoverPhoto
                {
                    PhotoId = new Guid(),
                    PhotoUrl = "https://s3-ap-southeast-1.amazonaws.com/elasticbeanstalk-ap-southeast-1-01156/user_123456/album_12345/avt.JPG"
                },
                Comments = CommentViewModel.GetSampleObject(),
                AddNewDestinationFormViewModel = AddNewDestinationFormViewModel.GetEmptyObject(Guid.NewGuid()),
                Destinations = new List<DestinationViewModel> { 
                    DestinationViewModel.GetSampleObject()
                }
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
                JourneyID = Guid.NewGuid(),
                UserID = Guid.NewGuid(),
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