using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.Models;
using System.ComponentModel.DataAnnotations;

namespace Footprints.ViewModels
{
    public class SearchViewModel
    {
        public String Keyword { get; set; }
        public IList<Destination> Destinations { get; set; }
        public IList<User> Users { get; set; }
        public IList<Journey> Journeys { get; set; }
        public IList<Journey> Places { get; set; }
        public IList<SearchType> SearchTypes { get; set; }
    }

    public class SearchDataViewModel
    {
        [Required]
        public String Keyword { get; set; }
        public IList<SearchType> SearchTypes { get; set; }
    }
    public enum SearchType {place,user};
}