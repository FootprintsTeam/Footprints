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
        public IList<User> SearchUser(String textEntered)
        {
            String[] split = textEntered.Split(new char[] {' '});
            foreach (String s in split)
            {
                
            }
            return Db.Cypher.Start(new
            {
                UserName = Node.ByIndexQuery("node_auto_index", "UserName:\"" + textEntered + "\""),
                FirstName = Node.ByIndexQuery("node_auto_index", "FirstName:\"" + textEntered + "\""),
                LastName = Node.ByIndexQuery("node_auto_index", "LastName:\"" + textEntered + "\""),
                Email = Node.ByIndexQuery("node_auto_index", "Email:\"" + textEntered + "\"")
            }).Match("(UserName:User)").Match("(FirstName:User)").Match("(LastName:User)").Match("(Email:User)").
                Return<User>("UserName, FirstName, LastName, Email").Results.ToList<User>();
        }
        public IList<Journey> SearchJourney(String textEntered)
        {
            return Db.Cypher.Start(new
            {
                Name = Node.ByIndexQuery("node_auto_index", "Name:\"" + textEntered + "\""),
                Description = Node.ByIndexQuery("node_auto_index", "Description:\"" + textEntered + "\"")
            }).Match("(Name:Journey)").Match("(Description:Journey)").
            Return<Journey>("Name, Description").Results.ToList<Journey>();
        }
        public IList<Destination> SearchDestination(String textEntered)
        {
            return Db.Cypher.Start(new
            {
                Name = Node.ByIndexQuery("node_auto_index", "Name:\"" + textEntered + "\""),
                Description = Node.ByIndexQuery("node_auto_index", "Description:\"" + textEntered + "\"")
            }).Match("(Name:Destination)").Match("(Description:Destination)").
            Return<Destination>("Name, Description").Results.ToList<Destination>();
        }
    }

    public interface ISearch
    {
        IList<User> SearchUser(String textEntered);
        IList<Journey> SearchJourney(String textEntered);
        IList<Destination> SearchDestination(String textEntered);
    }
}