using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.Common;
namespace Footprints.ViewModels
{
    public class NewsfeedViewModel
    {
        public Guid UserID { get; set; }
        public string UserName { get; set; }
        public String ProfilePicURL { get; set; }        
        public Guid ActivityID { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }

    public class AddPhotoWidgetViewModel : NewsfeedViewModel
    {
        public int NumberOfPhoto { get; set; }
        public Guid DestinationID { get; set; }
        public string DestinationName { get; set; }
        public IList<ImageViewModel> Photos { get; set; }
        public IList<CommentViewModel> Comments { get; set; }
        public int NumberOfLike { get; set; }
        public int NumberOfShare { get; set; }
        public static AddPhotoWidgetViewModel GetSampleObject(){
            var sample = new AddPhotoWidgetViewModel
            {
                ActivityID = new Guid(),
                UserName  = "Nhân Trịnh",
                ProfilePicURL = Constant.defaultAvatarUrl,
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
}