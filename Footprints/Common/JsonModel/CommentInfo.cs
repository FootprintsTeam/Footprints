using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.Common.JsonModel
{
    public class CommentInfo
    {
        public String HTMLString { get; set; }
        public Guid DestinationID { get; set; }
        public Guid JourneyID { get; set; }
    }
}