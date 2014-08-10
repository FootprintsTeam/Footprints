using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.DAL.Abstract;
using Footprints.Models;
namespace Footprints.Common
{
    public class Search : RepositoryBase<Search>, ISearch
    {
        public IList<User> SearchUser(String textEntered)
        {

        }
    }

    public interface ISearch
    {
        IList<User> SearchUser(String textEntered);
    }
}