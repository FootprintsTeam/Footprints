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

        public IList<Comment> GetAllCommentOnJourney(Guid JourneyID)
        {
            return Db.Cypher.Match("(comment:Comment)").Where((Comment comment) => comment.JourneyID == JourneyID).Return(comment => comment.As<Comment>()).Results.ToList<Comment>();
        }

        public IList<Comment> GetAllCommentOnDestination(Guid DestinationID)
        {
            return Db.Cypher.Match("(comment:Comment)").Where((Comment comment) => comment.DestinationID == DestinationID).Return(comment => comment.As<Comment>()).Results.ToList<Comment>();
        }

        public Comment GetAComment(Guid CommentID)
        {
            var query = Db.Cypher.Match("(comment:Comment)").Where((Comment comment) => comment.CommentID == CommentID).Return(comment => comment.As<Comment>()).Results;
            return query.FirstOrDefault<Comment>();
        }

        public bool UpdateComment(Guid UserID, Comment Comment)
        {
            Db.Cypher.OptionalMatch("(Comment:Comment)-[rel:COMMENT_BY]->(User:User)").Where((Comment comment) => comment.CommentID == Comment.CommentID).
                        AndWhere((User User) => User.UserID == UserID).With("Comment, rel").Where("rel IS NOT NULL").Set("Comment = {Comment}").WithParam("Comment", Comment).ExecuteWithoutResults();
            return true;
        }

        public bool AddDestinationComment(Guid UserID, Comment Comment)
        {
            //Cypher Query
            //CREATE (Comment:Comment {CommentID : '1', DestinationID : '1', NumberOfLike : 0, Timestamp : '04/07/2014'})
            //CREATE (Activity:Activity {Type : 'COMMENT_ON_DESTINATION', UserID : '1', DestinationID : '1', Timestamp : '04/07/2014'})
            //WITH Comment, Activity
            //MATCH (Destination:Destination)
            //WHERE (Destination.DestinationID = '1')
            //CREATE (Comment)-[:COMMENT_ON_DESTINATION]->(Destination)
            //CREATE (Activity)-[:COMMENT_ON_DESTINATION]->(Destination)
            //WITH Comment, Activity
            //MATCH (User:User)
            //WHERE (User.UserID = '1')
            //CREATE (Comment)-[:COMMENT_BY]->(User)
            //WITH User, Activity
            //MATCH (User)-[f:LATEST_ACTIVITY]->(nextActivity)
            //DELETE f
            //CREATE (User)-[:LATEST_ACTIVITY]->(Activity)
            //CREATE (Activity)-[:NEXT]->(nextActivity)
            //WITH User
            //MATCH (User)-[:FRIEND]->(friend)
            //WITH User, COLLECT(friend) AS friends
            //UNWIND friends AS fr
            //MATCH (fr)-[rel:EGO {UserID : fr.UserID}]->(NextFriendInEgo)
            //OPTIONAL MATCH (previousUser)-[r1:EGO {UserID : fr.UserID}]->(User)-[r2:EGO {UserID : fr.UserID}]->(nextUser)
            //WITH fr, User, rel, previousUser, r1, r2, nextUser, NextFriendInEgo
            //WHERE NextFriendInEgo <>  User
            //CREATE (fr)-[:EGO {UserID : fr.UserID }]->(User)
            //CREATE (User)-[:EGO {UserID : fr.UserID}]->(NextFriendInEgo)
            //WITH fr, previousUser, nextUser
            //WHERE previousUser IS NOT NULL AND nextUser IS NOT NULL
            //CREATE (previousUser)-[:EGO {UserID : fr.UserID}]->(nextUser)
            Activity activity = new Activity
            {
                ActivityID = Guid.NewGuid(),
                Type = "COMMENT_ON_DESTINATION",
                UserID = UserID,
                DestinationID = Comment.DestinationID,
                Timestamp = DateTimeOffset.Now
            };
            CypherQuery query = new CypherQuery(" CREATE (Comment:Comment {Comment}) " +
                                                " CREATE (Activity:Activity {Activity}) " +
                                                " WITH Comment, Activity " +
                                                " MATCH (User:User) " +
                                                " WHERE (User.UserID = {UserID}) " +
                                                " CREATE (Comment)-[:COMMENT_BY]->(User) " +
                                                " WITH User, Activity " +
                                                " MATCH (User)-[f:LATEST_ACTIVITY]->(nextActivity) " +
                                                " DELETE f " +
                                                " CREATE (User)-[:LATEST_ACTIVITY]->(Activity) " +
                                                " CREATE (Activity)-[:NEXT]->(nextActivity) " +
                                                " WITH User " +
                                                " MATCH (User)-[:FRIEND]->(friend) " +
                                                " WITH User, COLLECT(friend) AS friends " +
                                                " UNWIND friends AS fr " +
                                                " MATCH (fr)-[rel:EGO {UserID : fr.UserID}]->(NextFriendInEgo) " +
                                                " OPTIONAL MATCH (previousUser)-[r1:EGO {UserID : fr.UserID}]->(User)-[r2:EGO {UserID : fr.UserID}]->(nextUser) " +
                                                " WITH fr, User, rel, previousUser, r1, r2, nextUser, NextFriendInEgo " +
                                                " WHERE NextFriendInEgo <>  User " +
                                                " CREATE (fr)-[:EGO {UserID : fr.UserID }]->(User) " +
                                                " CREATE (User)-[:EGO {UserID : fr.UserID}]->(NextFriendInEgo) " +
                                                " WITH fr, previousUser, nextUser " +
                                                " WHERE previousUser IS NOT NULL AND nextUser IS NOT NULL " +
                                                " CREATE (previousUser)-[:EGO {UserID : fr.UserID}]->(nextUser)",
                                                new Dictionary<String, Object> { { "Comment", Comment }, { "Activity", activity }, { "DestinationID", Comment.DestinationID }, { "UserID", UserID } }, CypherResultMode.Projection);
            ((IRawGraphClient)Db).ExecuteCypher(query);
            return true;
        }

        public bool AddJourneyComment(Guid UserID, Comment Comment)
        {
            Activity activity = new Activity
            {
                ActivityID = Guid.NewGuid(),
                Type = "COMMENT_ON_JOURNEY",
                UserID = UserID,
                JourneyID = Comment.JourneyID,
                Timestamp = DateTimeOffset.Now
            };

            CypherQuery query = new CypherQuery(" CREATE (Comment:Comment {Comment}) " +
                                                " CREATE (Activity:Activity {Activity}) " +
                                                " WITH Comment, Activity " +
                                                " MATCH (User:User) " +
                                                " WHERE (User.UserID = {UserID}) " +
                                                " CREATE (Comment)-[:COMMENT_BY]->(User) " +
                                                " WITH User, Activity " +
                                                " MATCH (User)-[f:LATEST_ACTIVITY]->(nextActivity) " +
                                                " DELETE f " +
                                                " CREATE (User)-[:LATEST_ACTIVITY]->(Activity) " +
                                                " CREATE (Activity)-[:NEXT]->(nextActivity) " +
                                                " WITH User " +
                                                " MATCH (User)-[:FRIEND]->(friend) " +
                                                " WITH User, COLLECT(friend) AS friends " +
                                                " UNWIND friends AS fr " +
                                                " MATCH (fr)-[rel:EGO {UserID : fr.UserID}]->(NextFriendInEgo) " +
                                                " OPTIONAL MATCH (previousUser)-[r1:EGO {UserID : fr.UserID}]->(User)-[r2:EGO {UserID : fr.UserID}]->(nextUser) " +
                                                " WITH fr, User, rel, previousUser, r1, r2, nextUser, NextFriendInEgo " +
                                                " WHERE NextFriendInEgo <>  User " +
                                                " CREATE (fr)-[:EGO {UserID : fr.UserID }]->(User) " +
                                                " CREATE (User)-[:EGO {UserID : fr.UserID}]->(NextFriendInEgo) " +
                                                " WITH fr, previousUser, nextUser " +
                                                " WHERE previousUser IS NOT NULL AND nextUser IS NOT NULL " +
                                                " CREATE (previousUser)-[:EGO {UserID : fr.UserID}]->(nextUser)",
                                                new Dictionary<String, Object> { { "Comment", Comment }, { "Activity", activity }, { "JourneyID", Comment.JourneyID }, { "UserID", UserID } }, CypherResultMode.Projection);
            ((IRawGraphClient)Db).ExecuteCypher(query);
            return true;
        }

        public void LikeAComment(Guid UserID, Guid CommentID)
        {
            Db.Cypher.Match("(User:User), (Comment:Comment)").Where((User User) => User.UserID == UserID).AndWhere((Comment Comment) => Comment.CommentID == CommentID).
                Create("(Comment)-[:LIKED_BY]->(User)").Set("Comment.NumberOfLike = Comment.NumberOfLike + 1")
                .ExecuteWithoutResults();
        }

        public void UnlikeAComment(Guid UserID, Guid CommentID)
        {
            Db.Cypher.Match("(Comment:Comment)-[rel:LIKED_BY]->(User:User)").Where((User User) => User.UserID == UserID).AndWhere((Comment Comment) => Comment.CommentID == CommentID).
                 Set("Comment.NumberOfLike = Comment.NumberOfLike - 1").Delete("rel")
                .ExecuteWithoutResults();
        }

        public IList<User> GetAllUserLikeComment(Guid CommentID)
        {
            return Db.Cypher.Match("(Comment:Comment)-[:LIKED_BY]->(User:User)").Where((Comment Comment) => Comment.CommentID == CommentID).Return(user => user.As<User>()).Results.ToList<User>();
        }
        //TODO
        public void DeleteAComment(Guid UserID,Guid CommentID)
        {
            Db.Cypher.OptionalMatch("(Comment:Comment)-[rel:COMMENT_BY]->(User:User)").
                        Where((Comment comment) => comment.CommentID == CommentID).
                        AndWhere((User User) => User.UserID == UserID).
                        With("Comment, rel").Where("rel IS NOT NULL").
                        Match("(Comment)-[r]-()").
                        Match("(Activity:Activity)").
                        Where((Activity Activity) => Activity.CommentID == CommentID).
                        Set("Activity.Status = 'DELETED'").
                        Delete("Comment, r").
                        ExecuteWithoutResults();
        }
    }

    public interface ICommentRepository : IRepository<CommentRepository>
    {
        IList<Comment> GetAllCommentOnDestination(Guid DestinationID);
        IList<Comment> GetAllCommentOnJourney(Guid JounreyID);
        bool AddDestinationComment(Guid UserID, Comment Comment);
        bool AddJourneyComment(Guid UserID, Comment Comment);
        Comment GetAComment(Guid CommentID);
        bool UpdateComment(Guid UserID, Comment Comment);
        void LikeAComment(Guid UserID, Guid CommentID);
        void UnlikeAComment(Guid UserID, Guid CommentID);
        IList<User> GetAllUserLikeComment(Guid CommentID);
        void DeleteAComment(Guid UserID, Guid CommentID);
    }
}
