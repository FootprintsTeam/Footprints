﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.DAL.Concrete;
using Footprints.Models;
using Neo4jClient;
namespace Footprints.Services
{
    public interface IDestinationService
    {
        Destination GetDestination(Guid destinationID);
        Destination GetDestinationDetail(Guid DestinationID);
        Destination GetDestinationDetailWithLimitedContent(Guid DestinationID, int Limit);
        bool AddNewDestination(Guid UserID, Destination Destination, Place Place, Guid JourneyID);
        bool UpdateDestination(Guid UserID, Destination Destination);
        bool UpdateDestinationForAdmin(Destination Destination);
        void DeleteDestination(Guid UserID, Guid DestinationID);
        void AddNewContent(Content Content, Guid DestinationID, Guid UserID);
        void UpdateContent(Guid UserID, Content Content);
        void DeleteContent(Guid UserID, Guid ContentID);
        IList<Content> GetAllContent(Guid DestinationID);
        void LikeDestination(Guid UserID, Guid DestinationID);
        void UnlikeDestination(Guid UserID, Guid DestinationID);
        IList<User> GetAllUserLiked(Guid DestinationID);
        void ShareDestination(Guid UserID, Guid DestinationID, String Content);
        IList<User> GetAllUserShared(Guid DestinationID);
        IList<Destination> GetAllDestination();
        Place GetDestinationPlace(Guid DestinationID);
        bool UserAlreadyLike(Guid UserID, Guid DestinationID);
        int GetNumberOfDestination(Guid UserID);
        int GetNumberOfLike(Guid DestinationID);
        int GetNumberOfShare(Guid DestinationID);
        bool UserAlreadyShared(Guid UserID, Guid DestinationID);
        int GetNumberOfContent(Guid UserID);
        int GetNumberOfContentInDestination(Guid DestinationID);
        IList<Content> GetContentListWithSkipAndLimit(int Skip, int Limit, Guid DestinationID);
        void DeleteDestinationForAdmin(Guid DestinationID);
        bool UpdateDestination(Guid UserID, Guid DestinationID, String Name, String Description, DateTimeOffset TakenDate, Place Place, DateTimeOffset Timestamp);
        int GetMaxOrderNumber(Guid JourneyID);
        Destination GetDestinationDetailWithComment(Guid DestinationID, int Limit);
        long GetNumberOfCreatedDestinationBetweenDays(String Start, String End);
        long GetNumberOfDestination();
    }
    public class DestinationService : IDestinationService
    {
        IDestinationRepository _destinationRepo;
        public bool UserAlreadyLike(Guid userID, Guid destinationID) {
            return _destinationRepo.UserAlreadyLike(userID, destinationID);
        }
        public DestinationService(IDestinationRepository destinationRepo)
        {
            this._destinationRepo = destinationRepo;
        }
        public Destination GetDestination(Guid DestinationID)
        {
            return _destinationRepo.GetDestination(DestinationID);
        }
        public Place GetDestinationPlace(Guid DestinationID)
        {
            return _destinationRepo.GetDestinationPlace(DestinationID);
        }
        public Destination GetDestinationDetail(Guid DestinationID)
        {
            return _destinationRepo.GetDestinationDetail(DestinationID);
        }
        public Destination GetDestinationDetailWithLimitedContent(Guid DestinationID, int Limit)
        {
            return _destinationRepo.GetDestinationDetailWithLimitedContent(DestinationID, Limit);
        }
        public bool AddNewDestination(Guid UserID, Destination Destination, Place Place, Guid JourneyID)
        {
            return _destinationRepo.AddNewDestination(UserID, Destination, Place, JourneyID);
        }
        public bool UpdateDestination(Guid UserID, Destination Destination)
        {
            return _destinationRepo.UpdateDestination(UserID, Destination);
        }
        public bool UpdateDestinationForAdmin(Destination Destination)
        {
            return _destinationRepo.UpdateDestinationForAdmin(Destination);
        }
        public void DeleteDestination(Guid UserID, Guid DestinationID)
        {
            _destinationRepo.DeleteDestination(UserID, DestinationID);
        }
        public void AddNewContent(Content Content, Guid DestinationID, Guid UserID)
        {
            _destinationRepo.AddNewContent(Content, DestinationID, UserID);
        }
        public void UpdateContent(Guid UserID, Content Content)
        {
            _destinationRepo.UpdateContent(UserID, Content);
        }
        public void DeleteContent(Guid UserID, Guid ContentID)
        {
            _destinationRepo.DeleteContent(UserID, ContentID);
        }
        public IList<Content> GetAllContent(Guid DestinationID)
        {
            return _destinationRepo.GetAllContent(DestinationID);
        }
        public void LikeDestination(Guid UserID, Guid DestinationID)
        {
            _destinationRepo.LikeDestination(UserID, DestinationID);
        }
        public void UnlikeDestination(Guid UserID, Guid DestinationID)
        {
            _destinationRepo.UnlikeDestination(UserID, DestinationID);
        }
        public IList<User> GetAllUserLiked(Guid DestinationID)
        {
            return _destinationRepo.GetAllUserLiked(DestinationID);
        }
        public void ShareDestination(Guid UserID, Guid DestinationID, String Content)
        {
            _destinationRepo.ShareDestination(UserID, DestinationID, Content);
        }
        public IList<User> GetAllUserShared(Guid DestinationID)
        {
            return _destinationRepo.GetAllUserShared(DestinationID);
        }
        public IList<Destination> GetAllDestination()
        {
            return _destinationRepo.GetAllDestination();
        }
        public int GetNumberOfDestination(Guid UserID)
        {
            return _destinationRepo.GetNumberOfDestination(UserID);
        }
        public int GetNumberOfLike(Guid DestinationID)
        {
            return _destinationRepo.GetNumberOfLike(DestinationID);
        }
        public int GetNumberOfShare(Guid DestinationID)
        {
            return _destinationRepo.GetNumberOfShare(DestinationID);
        }
        public bool UserAlreadyShared(Guid UserID, Guid DestinationID)
        {
            return _destinationRepo.UserAlreadyShared(UserID, DestinationID);
        }
        public int GetNumberOfContent(Guid UserID)
        {
            return _destinationRepo.GetNumberOfContent(UserID);
        }
        public int GetNumberOfContentInDestination(Guid DestinationID)
        {
            return _destinationRepo.GetNumberOfContentInDestination(DestinationID);
        }
        public IList<Content> GetContentListWithSkipAndLimit(int Skip, int Limit, Guid DestinationID)
        {
            return _destinationRepo.GetContentListWithSkipAndLimit(Skip, Limit, DestinationID);
        }
        public void DeleteDestinationForAdmin(Guid DestinationID)
        {
            _destinationRepo.DeleteDestinationForAdmin(DestinationID);
        }
        public bool UpdateDestination(Guid UserID, Guid DestinationID, String Name, String Description, DateTimeOffset TakenDate, Place Place, DateTimeOffset Timestamp)
        {
            return _destinationRepo.UpdateDestination(UserID, DestinationID, Name, Description, TakenDate, Place, Timestamp);
        }
        public int GetMaxOrderNumber(Guid JourneyID) {
            return _destinationRepo.GetMaxOrderNumber(JourneyID);
        }
        public Destination GetDestinationDetailWithComment(Guid DestinationID, int Limit)
        {
            return _destinationRepo.GetDestinationDetailWithComment(DestinationID, Limit);
        }
        public long GetNumberOfCreatedDestinationBetweenDays(String Start, String End)
        {
            return _destinationRepo.GetNumberOfCreatedDestinationBetweenDays(Start, End);
        }
        public long GetNumberOfDestination()
        {
            return _destinationRepo.GetNumberOfDestination();
        }
    }
}