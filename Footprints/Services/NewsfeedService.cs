using Footprints.DAL.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.Services
{
    public class NewsfeedService : INewsfeedService
    {
        INewsFeedRepository newsfeedRepository;
        public NewsfeedService(INewsFeedRepository _NewsfeedRepository)
        {
            newsfeedRepository = _NewsfeedRepository;
        }
        public void LoadEgoNetwork(Guid UserID)
        {
            newsfeedRepository.LoadEgoNetwork(UserID);
        }
        public void RetrieveNewsFeed(Guid UserID, int k)
        {
            newsfeedRepository.RetrieveNewsFeed(UserID, k);
        }
        public void LoadMoreNewsfeed(Guid UserID, int l)
        {
            newsfeedRepository.LoadMoreNewsfeed(UserID, l);
        }
    }
    public interface INewsfeedService
    {
        void LoadEgoNetwork(Guid UserID);
        void RetrieveNewsFeed(Guid UserID, int k);
        void LoadMoreNewsfeed(Guid UserID, int l);
    }
}