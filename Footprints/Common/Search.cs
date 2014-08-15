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
            return Db.Cypher.Start(new
            {
                UserName = Node.ByIndexQuery("node_auto_index", "UserName:\"" + TextEntered + "\""),
                FirstName = Node.ByIndexQuery("node_auto_index", "FirstName:\"" + TextEntered + "\""),
                LastName = Node.ByIndexQuery("node_auto_index", "LastName:\"" + TextEntered + "\""),
                Email = Node.ByIndexQuery("node_auto_index", "Email:\"" + TextEntered + "\"")
            }).Match("(UserName:User)").Match("(FirstName:User)").Match("(LastName:User)").Match("(Email:User)").
                Return<User>("UserName, FirstName, LastName, Email").Results.ToList<User>();
        }
        public IList<Journey> SearchJourney(String TextEntered)
        {
            return Db.Cypher.Start(new
            {
                Name = Node.ByIndexQuery("node_auto_index", "Name:\"" + TextEntered + "\""),
                Description = Node.ByIndexQuery("node_auto_index", "Description:\"" + TextEntered + "\"")
            }).Match("(Name:Journey)").Match("(Description:Journey)").
            Return<Journey>("Name, Description").Results.ToList<Journey>();
        }
        public IList<Destination> SearchDestination(String TextEntered)
        {
            return Db.Cypher.Start(new
            {
                Name = Node.ByIndexQuery("node_auto_index", "Name:\"" + TextEntered + "\""),
                Description = Node.ByIndexQuery("node_auto_index", "Description:\"" + TextEntered + "\"")
            }).Match("(Name:Destination)").Match("(Description:Destination)").
            Return<Destination>("Name, Description").Results.ToList<Destination>();
        }
    }
    public interface ISearch
    {
        IList<User> SearchUser(String TextEntered);
        IList<Journey> SearchJourney(String TextEntered);
        IList<Destination> SearchDestination(String TextEntered);
    }
}