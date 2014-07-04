using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Footprints.DAL.Abstract;
using Neo4jClient;
using Footprints.Models;
using Neo4jClient.Cypher;
namespace Footprints.DAL.Concrete
{
    public class CommentRepository : RepositoryBase<CommentRepository>, ICommentRepository
    {
        public CommentRepository(IGraphClient client) : base(client) { }

        //TODO

        public List<Comment> getCommentByJourneyID(Guid journeyID)
        {
            var query = Db.Cypher.Match("(comment:Comment)").Where((Comment comment) => comment.journeyID == journeyID).Return(comment => comment.As<Comment>());
            return query.Results.ToList<Comment>();
        }

        public List<Comment> getCommentByDestinationID(Guid destinationID)
        {
            var query = Db.Cypher.Match("(comment:Comment)").Where((Comment comment) => comment.destinationID == destinationID).Return(comment => comment.As<Comment>());
            return query.Results.ToList<Comment>();
        }

        public Comment getCommentByCommentID(Guid commentID)
        {
            var query = Db.Cypher.Match("(comment:Comment)").Where((Comment comment) => comment.commentID == commentID).Return(comment => comment.As<Comment>());
            return query.Results.FirstOrDefault<Comment>();
        }

        public bool updateAComment(Comment comment)
        {
            var query = Db.Cypher.Match("(commentTaken:Comment)").Where((Comment commentTaken) => commentTaken.commentID == comment.commentID).
                                    Set("commentTaken = {comment}").WithParams(new { comment }).Return(commentReturned => commentReturned.As<Comment>());
            return (query != null);
        }

        public bool addNewCommentOnDestination(Guid userID, Comment comment)
        {
            //Cypher Query
            //CREATE (comment:Comment {commentID = '1', destinationID = '1', numberOfLikes = '0', timestamp = '04/07/2014'})
            //CREATE (activity:Activity {type : 'COMMENT_ON_DESTINATION', userID : '1', destinationID : '1', timestamp : '04/07/2014'})
            //WITH comment, activity
            //MATCH (destination:Destination)
            //WHERE (destination.destinationID = '1')
            //CREATE (comment)-[:COMMENT_ON_DESTINATION]->(destination)
            //CREATE (activity)-[:COMMENT_ON_DESTINATION]->(destination)
            //WITH comment, activity
            //MATCH (user:User)
            //WHERE (user.userID = '1')
            //CREATE (comment)-[:COMMENT_BY]->(user)
            //WITH user, activity
            //OPTIONAL MATCH (user)-[f:FIRST]->(nextActivity)
            //CREATE (user)-[:FIRST]->(activity)
            //WITH f, activity, nextActivity
            //WHERE f IS NOT NULL
            //DELETE f
            //CREATE (activity)-[:NEXT]->(nextActivity)
            Activity activity = new Activity
            {
                type = "COMMENT_ON_DESTINATION",
                userID = userID,
                destinationID = comment.destinationID,
                timeStamp = DateTime.Today
            };
            Db.Cypher.Create("(comment:Comment {comment})").WithParams(new { comment }).
                       Create("(activity:Activity {activity})").WithParams(new { activity }).
                       With("comment, activity").
                       Match("(destination:Destination)").Where("destination.destinationID = {destinationID}").WithParams(new { comment.destinationID }).
                       Create("(comment)-[:COMMENT_ON_DESTINATION]->(destination)").
                       Create("(activity)-[:COMMENT_ON_DESTINATION]->(destination)").
                       With("comment, activity").
                       Match("(user:User)").Where("user.userID = {userID}").WithParams(new { userID }).
                       Create("(comment)-[:COMMENT_BY]->(user)").
                       With("user, activity").
                       OptionalMatch("(user)-[f:FIRST]->(nextActivity)").
                       Create("(user)-[:FIRST]->(activity)").
                       With("f, activity, nextActivity").Where("f IS NOT NULL").Delete("f").
                       Create("(activity)-[:NEXT]->(nextActivity)").ExecuteWithoutResults();
            return true;
        }

        public bool addNewCommentOnJourney(Guid userID, Comment comment)
        {
            //Cypher Query
            //CREATE (comment:Comment {commentID = '1', journeyID = '1', numberOfLikes = '0', timestamp = '04/07/2014'})
            //CREATE (activity:Activity {type : 'COMMENT_ON_JOURNEY', userID : '1', journeyID : '1', timestamp : '04/07/2014'})
            //WITH comment, activity
            //MATCH (journey:Journey)
            //WHERE (journey.journeyID = '1')
            //CREATE (comment)-[:COMMENT_ON_JOURNEY]->(journey)
            //CREATE (activity)-[:COMMENT_ON_JOURNEY]->(journey)
            //WITH comment, activity
            //MATCH (user:User)
            //WHERE (user.userID = '1')
            //CREATE (comment)-[:COMMENT_BY]->(user)
            //WITH user, activity
            //OPTIONAL MATCH (user)-[f:FIRST]->(nextActivity)
            //CREATE (user)-[:FIRST]->(activity)
            //WITH f, activity, nextActivity
            //WHERE f IS NOT NULL
            //DELETE f
            //CREATE (activity)-[:NEXT]->(nextActivity)
            Activity activity = new Activity
            {
                type = "COMMENT_ON_JOURNEY",
                userID = userID,
                journeyID = comment.journeyID,
                timeStamp = DateTime.Today
            };
            Db.Cypher.Create("(comment:Comment {comment})").WithParams(new { comment }).
                       Create("(activity:Activity {activity})").WithParams(new { activity }).
                       With("comment, activity").
                       Match("(journey:Journey)").Where("journey.journeyID = {journeyID}").WithParams(new { comment.journeyID }).
                       Create("(comment)-[:COMMENT_ON_JOURNEY]->(journey)").
                       Create("(activity)-[:COMMENT_ON_JOURNEY]->(journey)").
                       With("comment, activity").
                       Match("(user:User)").Where("user.userID = {userID}").WithParams(new { userID }).
                       Create("(comment)-[:COMMENT_BY]->(user)").
                       With("user, activity").
                       OptionalMatch("(user)-[f:FIRST]->(nextActivity)").
                       Create("(user)-[:FIRST]->(activity)").
                       With("f, activity, nextActivity").Where("f IS NOT NULL").Delete("f").
                       Create("(activity)-[:NEXT]->(nextActivity)").ExecuteWithoutResults();
            return true;
        }

    }

    public interface ICommentRepository : IRepository<CommentRepository>
    {
        public List<Comment> getCommentByDestinationID(Guid destinationID);

        public List<Comment> getCommentByJourneyID(Guid journeyID);
        public bool addNewCommentOnDestination(Guid userID, Comment comment);
        public bool addNewCommentOnJourney(Guid userID, Comment comment);
        public Comment getCommentByCommentID(Guid commentID);
        public bool updateAComment(Comment comment);
    }
}
