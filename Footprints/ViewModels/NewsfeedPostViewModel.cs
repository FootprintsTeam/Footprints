﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.Common;

namespace Footprints.ViewModels
{
    public class NewsfeedPostViewModel
    {
        public Guid AuthorId { get; set; }
        public Guid UserId { get; set; }
        public Guid JourneyId { get; set; }
        public string AuthorAvatarURL { get; set; }
        public string UserAvatarURL { get; set; }
        public string AuthorName { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime Time { get; set; }
        public int NumberOfLikes { get; set; }
        public int NumberOfComments { get; set; }
        public int NumberOfImages { get; set; }
        public IEnumerable<CommentViewModel> Comments { get; set; }
        public string TimeAgo
        {
            get { return DateTimeFormat.TimeAgo(this.Time); }
            private set{}
        }
        public static IEnumerable<NewsfeedPostViewModel> GetSampleObject()
        {
            var sample = new NewsfeedPostViewModel
            {
                AuthorName = "Trịnh Nhân",
                UserName = "Nam",
                AuthorId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                JourneyId = Guid.NewGuid(),
                Description = "Nội Dung Nội DungNội DungNội DungNội DungNội Dung",
                Location = "Hà Nội",
                NumberOfComments = 10,
                NumberOfLikes = 10,
                Time = DateTime.Now,
                Comments = CommentViewModel.GetSampleObject(),
                NumberOfImages = 3
            };         
            List<NewsfeedPostViewModel> list = new List<NewsfeedPostViewModel>();
            list.Add(sample);
            return list;
        }
    }

    public class CommentViewModel
    {
        public Guid UserCommentId { get; set; }
        public string UserAvatarURL { get; set; }
        public string UserCommentName { get; set; }
        public string Content { get; set; }
        public DateTimeOffset Time { get; set; }
        public int NumberOfLike { get; set; }
        public string TimeAgo
        {
            get { return DateTimeFormat.TimeAgo(this.Time); }
            private set { }
        }
        public static IEnumerable<CommentViewModel> GetSampleObject()
        {
            var sample = new CommentViewModel { UserCommentId = Guid.NewGuid(), Content = "đây là một comment", Time = DateTime.Now, NumberOfLike = 10, UserCommentName = "Chiến Thắng" };
            var list = new List<CommentViewModel>();
            list.Add(sample);
            return list;
        }
    }

    public enum CommentType { Journey, Destination}
}