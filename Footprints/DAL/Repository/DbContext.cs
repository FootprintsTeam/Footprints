using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.DAL.Core;

namespace Footprints.DAL.Repository
{
    public class DbContext
    {
        public static IUserRepository Users { get { return new Users(); } }
        public static ICommentRepository Comments { get { return new Comments(); } }
        public static IDestinationRepository Destinations { get { return new Destinations(); } }
        public static IJourneyRepository Journeys { get { return new Journeys(); } }
    }
}