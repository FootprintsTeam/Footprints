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
        public IList<User> SearchUser(String TextEntered, int Limit)
        {
            List<User> result = null;
            //Search by UserName
            var query = Db.Cypher.Start(new
            {
                UserName = Node.ByIndexQuery("node_auto_index", "UserName:\"" + TextEntered + "\"")
            }).
            Return(UserName => UserName.As<User>()).
            Limit(Limit).
            Results;
            if (query.Count() > 0 && result == null) result = new List<User>();
            foreach (var item in query)
            {
                result.Add(item);
            }
            //Search by FirstName
            query = Db.Cypher.Start(new
            {
                FirstName = Node.ByIndexQuery("node_auto_index", "FirstName:\"" + TextEntered + "\"")
            }).
            Return(FirstName => FirstName.As<User>()).
            Limit(Limit).
            Results;
            if (query.Count() > 0 && result == null) result = new List<User>();
            foreach (var item in query)
            {
                result.Add(item);
            }
            //Search by LastName
            query = Db.Cypher.Start(new
            {
                LastName = Node.ByIndexQuery("node_auto_index", "LastName:\"" + TextEntered + "\"")
            }).
            Return(LastName => LastName.As<User>()).
            Limit(Limit).
            Results;
            if (query.Count() > 0 && result == null) result = new List<User>();
            foreach (var item in query)
            {
                result.Add(item);
            }
            //Search by Email
            query = Db.Cypher.Start(new
            {
                Email = Node.ByIndexQuery("node_auto_index", "Email:\"" + TextEntered + "\"")
            }).
           Return(Email => Email.As<User>()).
           Limit(Limit).
           Results;
            if (query.Count() > 0 && result == null) result = new List<User>();
            foreach (var item in query)
            {
                result.Add(item);
            }
            return result;
        }
        public IList<Journey> SearchJourney(String TextEntered, int Limit)
        {
            List<Journey> result = null;
            var query = Db.Cypher.Start(new
            {
                Name = Node.ByIndexQuery("node_auto_index", "Name:\"" + TextEntered + "\"")
            }).Match("(Name:Journey)").
            Return(Name => Name.As<Journey>()).Limit(Limit).
            Results;
            if (query.Count() > 0 && result == null) result = new List<Journey>();
            foreach (var item in query)
            {
                if (item != null) result.Add(item);
            }
            query = Db.Cypher.Start(new
            {
                Description = Node.ByIndexQuery("node_auto_index", "Description:\"" + TextEntered + "\"")
            }).Match("(Description:Journey)").
            Return(Description => Description.As<Journey>()).Limit(Limit).
            Results;
            if (query.Count() > 0 && result == null) result = new List<Journey>();
            foreach (var item in query)
            {
                if (item.Name != null) result.Add(item);
            }
            return result;
        }
        public IList<Destination> SearchDestination(String TextEntered, int Limit)
        {
            List<Destination> result = null;
            var query = Db.Cypher.Start(new
            {
                Name = Node.ByIndexQuery("node_auto_index", "Name:\"" + TextEntered + "\"")
            }).Match("(Name:Destination)").
            Return(Name => Name.As<Destination>()).Limit(Limit).
            Results;
            if (query.Count() > 0 && result == null) result = new List<Destination>();
            foreach (var item in query)
            {
                if (item != null) result.Add(item);
            }
            query = Db.Cypher.Start(new
            {
                Description = Node.ByIndexQuery("node_auto_index", "Description:\"" + TextEntered + "\"")
            }).Match("(Description:Destination)").
            Return(Description => Description.As<Destination>()).Limit(Limit).
            Results;
            if (query.Count() > 0 && result == null) result = new List<Destination>();
            foreach (var item in query)
            {
                if (item.Name != null) result.Add(item);
            }
            return result;
        }
        public IList<Journey> SearchPlace(String TextEntered, int Limit)
        {
            var query = Db.Cypher.Start(new
            {
                Name = Node.ByIndexQuery("node_auto_index", "Name:\"" + TextEntered + "\"")
            }).Match("(User:User)-[:HAS]->(Journey:Journey)-[:HAS]->(Destination:Destination)-[:AT]->(Name:Place )")
            .Return(Journey => Journey.As<Journey>()).Limit(Limit).
            Results;
            return query.Count() == 0 ? null : query.ToList<Journey>();
        }
    }
    public interface ISearch
    {
        IList<User> SearchUser(String TextEntered, int Limit);
        IList<Journey> SearchJourney(String TextEntered, int Limit);
        IList<Destination> SearchDestination(String TextEntered, int Limit);
        IList<Journey> SearchPlace(String TextEntered, int Limit);
    }
}