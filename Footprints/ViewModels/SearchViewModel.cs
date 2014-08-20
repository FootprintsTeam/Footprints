using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.Models;

namespace Footprints.ViewModels
{
    public class SearchViewModel
    {
        public String Keyword { get; set; }
        public IList<Destination> Destinations { get; set; }
        public IList<User> Users { get; set; }
        public IList<Journey> Journeys { get; set; }
        public IList<Journey> Places { get; set; }
    }

    public class SearchDataViewModel
    {
        public String Keyword { get; set; }
        public SearchType SearchTypes { get; set; }
    }
    public enum SearchType {journey,destination,place,user};
}