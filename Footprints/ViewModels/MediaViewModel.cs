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
        public int NumberOfPhotos { get; set; }
        public int NumberOfAlbums { get; set; }
        public List<String> Photos { get; set; }
        public static MediaViewModel GetSampleObject()
        {
            return new MediaViewModel()
            {
                NumberOfAlbums = 10,
                NumberOfPhotos = 159,
                Photos = new List<string>()
                {
                    "../assets/images/gallery-2/3.jpg",
                    "../assets/images/gallery-2/3.jpg",
                    "../assets/images/gallery-2/3.jpg",
                    "../assets/images/gallery-2/3.jpg"
                }
            };
        }
    }

    public class AlbumsViewModel
    {
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
        public List<Content> Photos { get; set; }

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