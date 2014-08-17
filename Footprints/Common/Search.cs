using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.DAL.Abstract;
using Footprints.Models;
using Neo4jClient.Cypher;
using Neo4jClient;
namespace Footprints.Common
{
    public class Search : RepositoryBase<Search>, ISearch
    {
        public Search(IGraphClient client) : base(client) { }

        public IList<Object> GeneralSearch(String TextEntered)
        {

            return null;
        }
        public IList<User> SearchUser(String TextEntered)
        {
            var query = Db.Cypher.Start(new
            {
                UserName = Node.ByIndexQuery("node_auto_index", "UserName:\"" + TextEntered + "\""),
                FirstName = Node.ByIndexQuery("node_auto_index", "FirstName:\"" + TextEntered + "\""),
                LastName = Node.ByIndexQuery("node_auto_index", "LastName:\"" + TextEntered + "\""),
                Email = Node.ByIndexQuery("node_auto_index", "Email:\"" + TextEntered + "\"")
            }).
            Match("(UserName:User)").Match("(FirstName:User)").Match("(LastName:User)").Match("(Email:User)").
            Return((UserName, FirstName, LastName, Email) => new
             {
                 UserName = UserName.As<User>(),
                 FirstName = FirstName.As<User>(),
                 LastName = LastName.As<User>(),
                 Email = Email.As<User>()
             }).
            Results;
            List<User> result = new List<User>();
            foreach (var item in query)
            {
                if (item.UserName != null) result.Add(item.UserName);
                if (item.FirstName != null) result.Add(item.FirstName);
                if (item.LastName != null) result.Add(item.LastName);
                if (item.Email != null) result.Add(item.Email);
            }
            return query.Count() == 0 ? null : result;
        }
        public IList<Journey> SearchJourney(String TextEntered)
        {
            var query = Db.Cypher.Start(new
            {
                Name = Node.ByIndexQuery("node_auto_index", "Name:\"" + TextEntered + "\""),
                Description = Node.ByIndexQuery("node_auto_index", "Description:\"" + TextEntered + "\"")
            }).Match("(Name:Journey)").Match("(Description:Journey)").
            Return((Name, Description) => new 
            {
                Name = Name.As<Journey>(),
                Description = Name.As<Journey>()
            }).
            Results;
            List<Journey> result = new List<Journey>();
            foreach (var item in query)
            {
                if (item.Name != null) result.Add(item.Name);
                if (item.Description != null) result.Add(item.Description);
            }
            return query.Count() == 0 ? null : result;

        }
        public IList<Destination> SearchDestination(String TextEntered)
        {
            var query = Db.Cypher.Start(new
            {
                Name = Node.ByIndexQuery("node_auto_index", "Name:\"" + TextEntered + "\""),
                Description = Node.ByIndexQuery("node_auto_index", "Description:\"" + TextEntered + "\"")
            }).Match("(Name:Destination)").Match("(Description:Destination)").
            Return((Name, Description) => new
            {
                Name = Name.As<Destination>(),
                Description = Description.As<Destination>()
            }).Results;
            List<Destination> result = new List<Destination>();
            foreach (var item in query)
            {
                if (item.Name != null) result.Add(item.Name);
                if (item.Description != null) result.Add(item.Description);
            }
            return query.Count() == 0 ? null : result;
        }
    }
    public interface ISearch
    {
        IList<User> SearchUser(String TextEntered);
        IList<Journey> SearchJourney(String TextEntered);
        IList<Destination> SearchDestination(String TextEntered);
    }
}