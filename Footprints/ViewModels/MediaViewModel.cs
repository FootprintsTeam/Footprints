using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.ViewModels
{
    public class MediaViewModel
    {
        public int NumberOfPhotos { get; set; }
        public IEnumerable<AlbumViewModel> Albums { get; set; }
    }

    public class AlbumViewModel
    {
        public Guid AlbumId { get; set; }
        public string AlbumName { get; set; }
        public int NumberOfPhotos { get; set; }
        public Location Location { get; set; }
        public IEnumerable<Photo> CoverPhotos { get; set; }

        public static IEnumerable<AlbumViewModel> GetSampleObject()
        {
            var sample = new AlbumViewModel
            {
                AlbumId = new Guid(),
                AlbumName = "Tên Album...",
                NumberOfPhotos = 23,
                Location = new Location
                {
                    LocationId = new Guid(),
                    LocationName = "Đồ Sơn"
                },
                CoverPhotos = new List<Photo>()
                {
                    new Photo {
                        PhotoId = new Guid(),
                        PhotoUrl = "https://s3-ap-southeast-1.amazonaws.com/elasticbeanstalk-ap-southeast-1-01156/user_123456/album_12345/avt.JPG"
                    },
                    new Photo {
                        PhotoId = new Guid(),
                        PhotoUrl = "https://s3-ap-southeast-1.amazonaws.com/elasticbeanstalk-ap-southeast-1-01156/user_123456/album_12345/avt.JPG"
                    },
                    new Photo {
                        PhotoId = new Guid(),
                        PhotoUrl = "https://s3-ap-southeast-1.amazonaws.com/elasticbeanstalk-ap-southeast-1-01156/user_123456/album_12345/avt.JPG"
                    },
                    new Photo {
                        PhotoId = new Guid(),
                        PhotoUrl = "https://s3-ap-southeast-1.amazonaws.com/elasticbeanstalk-ap-southeast-1-01156/user_123456/album_12345/avt.JPG"
                    }
                }
            };
            var sample2 = sample;
            List<AlbumViewModel> list = new List<AlbumViewModel>();
            list.Add(sample);
            list.Add(sample2);
            return list;
        }
    }

    public class Photo
    {
        public Guid PhotoId { get; set; }
        public string PhotoUrl { get; set; }
    }

    public class Location
    {
        public Guid LocationId { get; set; }
        public string LocationName { get; set; }
    }
}