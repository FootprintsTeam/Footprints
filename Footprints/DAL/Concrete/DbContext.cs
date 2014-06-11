using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.DAL.Abstract;

namespace Footprints.DAL.Concrete
{
    public class DbContext : IDbContext
    {
        private IUserRepository _user;
        private ICommentRepository _comment;
        private IJourneyRepository _journey;
        private IDestinationRepository _destination;

        public DbContext(IUserRepository user, ICommentRepository comment, IJourneyRepository journey, IDestinationRepository destination) {
            _user = user;
            _comment = comment;
            _journey = journey;
            _destination = destination;
        }
        public IUserRepository Users { get { return _user; } }
        public ICommentRepository Comments { get { return _comment; } }
        public IDestinationRepository Destinations { get { return _destination; } }
        public IJourneyRepository Journeys { get { return _journey; } }
    }
}