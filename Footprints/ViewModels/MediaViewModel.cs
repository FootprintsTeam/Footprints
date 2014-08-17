using Footprints.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Footprints.ViewModels
{
    public class MediaViewModel
    {
        public Guid TargetUserID { get; set; }
        public int NumberOfPhotos { get; set; }
        public IList<Content> Photos { get; set; }
    }

    public class AlbumsViewModel
    {
        public Guid TargetUserID { get; set; }
        public Guid UserID { get; set; }
        public int NumberOfPhotos { get; set; }
        public int NumberOfAlbums { get; set; }
        public IList<AlbumDetailsViewModel> AlbumList { get; set; }

        public static AlbumsViewModel GetSampleObject()
        {
            List<AlbumDetailsViewModel> listAlbumDetails = new List<AlbumDetailsViewModel>();
            listAlbumDetails.Add(AlbumDetailsViewModel.GetSampleObject());
            listAlbumDetails.Add(AlbumDetailsViewModel.GetSampleObject());
            listAlbumDetails.Add(AlbumDetailsViewModel.GetSampleObject());
            return new AlbumsViewModel
            {
                NumberOfPhotos = 159,
                NumberOfAlbums = 10,
                AlbumList = listAlbumDetails
            };
        }
    }

    public class AlbumDetailsViewModel
    {
        public Guid AlbumID { get; set; }
        public int NumberOfPhotos { get; set; }
        public Guid JourneyID { get; set; }
        public String JourneyName { get; set; }
        public Guid DestinationID { get; set; }
        public String DestinationName { get; set; }
        public IList<Content> Photos { get; set; }

        public static AlbumDetailsViewModel GetSampleObject()
        {
            return new AlbumDetailsViewModel
            {
                AlbumID = Guid.NewGuid(),
                NumberOfPhotos = 23,
                JourneyID = Guid.NewGuid(),
                JourneyName = "Journey name",
                DestinationID = Guid.NewGuid(),
                DestinationName = "Destination name"
            };
        }
    }

    public class PhotoUploadFormViewModel
    {
        [RegularExpression(Common.Constant.GUID_REGEX)]
        public Guid DestinationID { get; set; }
        [Required]
        public String ActionName { get; set; }
        [Required]
        public String ControllerName { get; set; }
        [Required]
        public bool DisplaySlide { get; set; }
    }

    public class DeletePhotoFormViewModel
    {
        [Required]
        [RegularExpression(Common.Constant.GUID_REGEX)]
        public Guid ContentID { get; set; }
        [Required]
        [RegularExpression(Common.Constant.GUID_REGEX)]
        public Guid DestinationID { get; set; }
    }
}