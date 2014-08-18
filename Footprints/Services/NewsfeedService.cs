using Footprints.DAL.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.Models;

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
        public IList<Activity> RetrieveNewsFeed(Guid UserID, int k)
        {
            return newsfeedRepository.RetrieveNewsFeed(UserID, k);
        }
        public void LoadMoreNewsfeed(Guid UserID, int l)
        {
            newsfeedRepository.LoadMoreNewsfeed(UserID, l);
        }
    }
    public interface INewsfeedService
    {
        void LoadEgoNetwork(Guid UserID);
        IList<Activity> RetrieveNewsFeed(Guid UserID, int k);
        IList<Activity> LoadMoreNewsfeed(Guid UserID, int l);
    }
}