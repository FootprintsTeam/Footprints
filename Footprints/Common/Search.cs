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
            var username = Db.Cypher.Start(new
            {
                UserName = Node.ByIndexQuery("node_auto_index", "UserName:" + TextEntered + "~")
            }).Match("(UserName:User)").
            Return(UserName => UserName.As<User>()).
            Limit(Limit).
            Results;
            if (username.Count() > 0 && result == null) result = new List<User>();
            foreach (var item in username)
            {
                if (!result.Contains(item)) result.Add(item);
            }
            //Search by FirstName
            var firstname = Db.Cypher.Start(new
            {
                FirstName = Node.ByIndexQuery("node_auto_index", "FirstName:\"" + TextEntered + "\"")
            }).Match("(FirstName:User)").
            Return(FirstName => FirstName.As<User>()).
            Limit(Limit).
            Results;
            if (firstname.Count() > 0 && result == null) result = new List<User>();
            foreach (var item in firstname)
            {
                if (!result.Contains(item)) result.Add(item);
            }
            //Search by LastName
            var lastname = Db.Cypher.Start(new
            {
                LastName = Node.ByIndexQuery("node_auto_index", "LastName:\"" + TextEntered + "\"")
            }).Match("(LastName:User)").
            Return(LastName => LastName.As<User>()).
            Limit(Limit).
            Results;
            if (lastname.Count() > 0 && result == null) result = new List<User>();
            foreach (var item in lastname)
            {
                if (!result.Contains(item)) result.Add(item);
            }
            //Search by Email
            var email = Db.Cypher.Start(new
            {
                Email = Node.ByIndexQuery("node_auto_index", "Email:\"" + TextEntered + "\"")
            }).Match("(Email:User)").
           Return(Email => Email.As<User>()).
           Limit(Limit).
           Results;
            if (email.Count() > 0 && result == null) result = new List<User>();
            foreach (var item in email)
            {
                if (!result.Contains(item)) result.Add(item);
            }
            return result;
        }
        public IList<Journey> SearchJourney(String TextEntered, int Limit)
        {
            List<Journey> result = null;
            var name = Db.Cypher.Start(new
            {
                Name = Node.ByIndexQuery("node_auto_index", "Name:\"" + TextEntered + "\"")
            }).Match("(Name:Journey)").
            Return(Name => Name.As<Journey>()).Limit(Limit).
            Results;
            if (name.Count() > 0 && result == null) result = new List<Journey>();
            foreach (var item in name)
            {
                if (item != null && !result.Contains(item)) result.Add(item);
            }
            var description = Db.Cypher.Start(new
            {
                Description = Node.ByIndexQuery("node_auto_index", "Description:\"" + TextEntered + "\"")
            }).Match("(Description:Journey)").
            Return(Description => Description.As<Journey>()).Limit(Limit).
            Results;
            if (description.Count() > 0 && result == null) result = new List<Journey>();
            foreach (var item in description)
            {
                if (item.Name != null && !result.Contains(item)) result.Add(item);
            }
            return result;
        }
        public IList<Destination> SearchDestination(String TextEntered, int Limit)
        {
            List<Destination> result = null;
            var name = Db.Cypher.Start(new
            {
                Name = Node.ByIndexQuery("node_auto_index", "Name:\"" + TextEntered + "\"")
            }).Match("(Name:Destination)-[:AT]->(Place:Place)").
            Return((Name, Place) => new
            {
                Name = Name.As<Destination>(),
                Place = Place.As<Place>()
            } 
            ).Limit(Limit).
            Results;
            if (name.Count() > 0 && result == null) result = new List<Destination>();
            foreach (var item in name)
            {
                if (item.Name != null)
                {
                    if (item.Place != null) item.Name.Place = item.Place;
                    if (!result.Contains(item.Name)) result.Add(item.Name);
                }
            }

            var description = Db.Cypher.Start(new
            {
                Description = Node.ByIndexQuery("node_auto_index", "Description:\"" + TextEntered + "\"")
            }).Match("(Description:Destination)-[:AT]->(Place:Place)").
            Return((Description, Place) => new
            {
                Description = Description.As<Destination>(),
                Place = Place.As<Place>()
            }
            ).Limit(Limit).Results;
            if (description.Count() > 0 && result == null) result = new List<Destination>();
            foreach (var item in description)
            {
                if (item.Description != null)
                {
                    if (item.Place != null) item.Description.Place = item.Place;
                    if (!result.Contains(item.Description)) result.Add(item.Description);
                }
            }
            return result;
        }
        public IList<Journey> SearchPlace(String TextEntered, int Limit)
        {
            List<Journey> result = null;
            var name = Db.Cypher.Start(new
            {
                Name = Node.ByIndexQuery("node_auto_index", "Name:\"" + TextEntered + "\"")
            }).Match("(User:User)-[:HAS]->(Journey:Journey)-[:HAS]->(Destination:Destination)-[:AT]->(Name:Place)")
            .Return(Journey => Journey.As<Journey>()).Limit(Limit).
            Results;
            if (name.Count() > 0 && result == null) result = new List<Journey>();
            foreach (var item in name)
            {
                if (!result.Contains(item)) result.Add(item);
            }

            var desciption = Db.Cypher.Start(new
            {
                Name = Node.ByIndexQuery("node_auto_index", "Address:\"" + TextEntered + "\"")
            }).Match("(User:User)-[:HAS]->(Journey:Journey)-[:HAS]->(Destination:Destination)-[:AT]->(Address:Place)")
            .Return(Journey => Journey.As<Journey>()).Limit(Limit).
            Results;
            if (desciption.Count() > 0 && result == null) result = new List<Journey>();
            foreach (var item in desciption)
            {
                if (!result.Contains(item)) result.Add(item);
            }
            return result;
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