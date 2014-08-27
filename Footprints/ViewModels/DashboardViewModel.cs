using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.ViewModels
{
    public class DashboardViewModel
    {
        public long TodayRegisterUser { get; set; }
        public long TotalUser { get; set; }
        public long TodayNewJourney { get; set; }
        public long TotalJourney { get; set; }
        public long TodayNewDestination { get; set; }
        public long TotalDestination { get; set; }
    }
}