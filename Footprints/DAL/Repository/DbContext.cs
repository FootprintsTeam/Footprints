using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.DAL.Repository
{
    public class DbContext
    {
        public static Users Users { get { return new Users(); } }
        public static Comments Comments { get { return new Comments(); } }
        public static Destinations Destinations { get { return new Destinations(); } }
        public static Journeys Journeys { get { return new Journeys(); } }
    }
}