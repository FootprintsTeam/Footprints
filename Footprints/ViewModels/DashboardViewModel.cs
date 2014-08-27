using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.ViewModels
{
    public class DashboardViewModel
    {
        public int TodayRegisterUser { get; set; }
        public int TotalUser { get; set; }
        public int TodayNewJourney { get; set; }
        public int TotalJourney { get; set; }
        public int TodayNewDestination { get; set; }
        public int TotalDestination { get; set; }
    }
}