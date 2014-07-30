using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.DAL.Concrete;
using Footprints.Models;
using Neo4jClient;
namespace Footprints.Service
{
    public interface IDestinationService {
        Destination GetADestination(Guid destinationID);
        Destination GetADestinationDetail(Guid DestinationID);


        //bool AddNewDestination(Guid UserID, Destination Destination, String PlaceID, Guid JourneyID);
        bool AddNewDestination(Guid UserID, Destination Destination, Place Place, Guid JourneyID);


        bool UpdateDestination(Destination Destination);


        //void DeleteDestination(Guid DestinationID);
        void DeleteDestination(Guid UserID, Guid DestinationID);



        void AddNewContent(Content Content, Guid DestinationID, Guid UserID);
        void UpdateContent(Content Content);
        void DeleteContent(Guid ContentID);
        IEnumerable<Content> GetAllContent(Guid DestinationID);
        void AddNewPlace(Place Place);
        int GetNumberOfLike(Guid DestinationID);
        void LikeDestination(Guid UserID, Guid DestinationID);
        void UnlikeDestination(Guid UserID, Guid DestinationID);
        IEnumerable<User> GetAllUserLiked(Guid DestinationID);
        int GetNumberOfShare(Guid DestinationID);
        void ShareDestination(Guid UserID, Guid DestinationID, String Content);
        IEnumerable<User> GetAllUserShared(Guid DestinationID);
    }
    public class DestinationService : IDestinationService
    {
        IDestinationRepository _destinationRepo;
        public DestinationService(IDestinationRepository destinationRepo)
        {
            this._destinationRepo = destinationRepo;
        }
        public Destination GetADestination(Guid DestinationID)
        {
            return _destinationRepo.GetADestination(DestinationID);
        }
        public Destination GetADestinationDetail(Guid DestinationID)
        {
            return _destinationRepo.GetADestinationDetail(DestinationID);
        }


        //public bool AddNewDestination(Guid UserID, Destination Destination, String PlaceID, Guid JourneyID)
        public bool AddNewDestination(Guid UserID, Destination Destination, Place Place, Guid JourneyID)
        {
            return _destinationRepo.AddNewDestination(UserID, Destination, Place, JourneyID);
        }



        public bool UpdateDestination(Destination Destination)
        {
            return _destinationRepo.UpdateDestination(Destination);
        }


        //void DeleteDestination(Guid UserID, Guid DestinationID)
        public void DeleteDestination(Guid UserID, Guid DestinationID)
        {
            _destinationRepo.DeleteDestination(UserID, DestinationID);
        }


        public void AddNewContent(Content Content, Guid DestinationID, Guid UserID)
        {
            _destinationRepo.AddNewContent(Content, DestinationID, UserID);
        }
        public void UpdateContent(Content Content)
        {
            _destinationRepo.UpdateContent(Content);
        }
        public void DeleteContent(Guid ContentID)
        {
            _destinationRepo.DeleteContent(ContentID);
        }
        public IEnumerable<Content> GetAllContent(Guid DestinationID)
        {
            return _destinationRepo.GetAllContent(DestinationID);
        }
        public void AddNewPlace(Place Place)
        {
            _destinationRepo.AddNewPlace(Place);
        }
        public int GetNumberOfLike(Guid DestinationID)
        {
            return _destinationRepo.GetNumberOfLike(DestinationID);
        }
        public void LikeDestination(Guid UserID, Guid DestinationID)
        {
             _destinationRepo.LikeDestination(UserID, DestinationID);
        }
        public void UnlikeDestination(Guid UserID, Guid DestinationID)
        {
            _destinationRepo.UnlikeDestination(UserID, DestinationID);
        }
        public IEnumerable<User> GetAllUserLiked(Guid DestinationID)
        {
            return _destinationRepo.GetAllUserLiked(DestinationID);
        }
        public int GetNumberOfShare(Guid DestinationID)
        {
            return _destinationRepo.GetNumberOfShare(DestinationID);
        }
        public void ShareDestination(Guid UserID, Guid DestinationID, String Content)
        {
            _destinationRepo.ShareDestination(UserID, DestinationID, Content);
        }
        public IEnumerable<User> GetAllUserShared(Guid DestinationID)
        {
            return _destinationRepo.GetAllUserShared(DestinationID);
        }
    }
}