using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Footprints.DAL.Core
{
    public class Db
    {
        public string connectionString { get; set; }

        public Db(string conn = null)
        {
            
        }
    }
}