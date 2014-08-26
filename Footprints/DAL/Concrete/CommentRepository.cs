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
        public IList<Comment> GetAllCommentOnJourney(Guid JourneyID)
        {
            var query = Db.Cypher.Match("(comment:Comment)-[:ON]->(Journey:Journey)").
                        Where((Journey Journey) => Journey.JourneyID == JourneyID).
                        Match("(comment)-[:COMMENT_BY]->(user:User)").
                        Return((comment, user) => new
                        {
                            comment = comment.As<Comment>(),
                            user = user.As<User>()
                        }).OrderBy("comment.Timestamp").Results;
            List<Comment> result = new List<Comment>();
            Comment currentComment = new Comment();
            foreach (var item in query)
            {
                currentComment = item.comment;
                currentComment.User = new User();
                currentComment.User = item.user;
                result.Add(currentComment);
            }
            return query.Count() == 0 ? null : result;
        }
        public IList<Comment> GetAllCommentOnDestination(Guid DestinationID)
        {

            var query = Db.Cypher.Match("(comment:Comment)-[:ON]->(Destination:Destination)").
                        Where((Destination Destination) => Destination.DestinationID == DestinationID).
                        Match("(comment)-[:COMMENT_BY]->(user:User)").
                        Return((comment, user) => new
                        {
                            comment = comment.As<Comment>(),
                            user = user.As<User>()
                        }).OrderBy("comment.Timestamp").Results;
            List<Comment> result = new List<Comment>();
            Comment currentComment = new Comment();
            foreach (var item in query)
            {
                currentComment = item.comment;
                currentComment.User = new User();
                currentComment.User = item.user;
                result.Add(currentComment);
            }
            return query.Count() == 0 ? null : result;
        }
        public Comment GetAComment(Guid CommentID)
        {
            var query = Db.Cypher.Match("(Comment:Comment)-[:COMMENT_BY]->(User:User)").Where((Comment comment) => comment.CommentID == CommentID).
                        Return( (comment, user) => new {
                            comment = comment.As<Comment>(),
                            user = user.As<User>()
                            }).Results;
            Comment result = new Comment();
            foreach (var item in query)
            {
                result = item.comment;
                result.User = new User();
                result.User = item.user;
                return result;
            }
            return null;
        }
        public bool UpdateComment(Guid UserID, Comment Comment)
        {
            Db.Cypher.OptionalMatch("(comment:Comment)-[rel:COMMENT_BY]->(User:User)").
                        Where((Comment comment) => comment.CommentID == Comment.CommentID).
                        AndWhere((User User) => User.UserID == UserID).
                        With("comment, rel").Where("rel IS NOT NULL").
                        Set("comment = {Comment}").WithParam("Comment", Comment).
                        With("Comment").
                        Match("Activity:Activity").Where((Activity Activity) => Activity.CommentID == Comment.CommentID).
                        Set("Activity.Content = Comment.Content").
                        ExecuteWithoutResults();
            return true;
        }
        public bool AddDestinationComment(Guid UserID, Comment Comment)
        {            
            Activity activity = new Activity
            {
                ActivityID = Guid.NewGuid(),
                Status = Activity.StatusEnum.Active,
                Type = "COMMENT_ON_DESTINATION",
                UserID = UserID,
                CommentContent = Comment.Content,
                CommentID = Comment.CommentID,
                DestinationID = Comment.DestinationID,
                Timestamp = DateTimeOffset.Now
            };
            CypherQuery query = new CypherQuery(" CREATE (Comment:Comment {Comment}) " +
                                                " CREATE (Activity:Activity {Activity}) " +
                                                " WITH Comment, Activity " +
                                                " MATCH (Destination:Destination) " +
                                                " WHERE (Destination.DestinationID = {DestinationID}) " +
                                                " CREATE (Comment)-[:ON]->(Destination) " +
                                                " CREATE (Activity)-[:ACT_ON_DESTINATION]->(Destination) " +
                                                " SET Activity.Destination_Name = Destination.Name, Activity.Destination_Description = Destination.Description, Activity.Destination_NumberOfLike = Destination.NumberOfLike, Destination.Destination_NumberOfShare = Destination.NumberOfShare" +
                                                " WITH Comment, Activity " +
                                                " MATCH (User:User) " +
                                                " WHERE (User.UserID = {UserID}) " +
                                                " CREATE (Comment)-[:COMMENT_BY]->(User) " +
                                                " SET Activity.UserName = User.UserName, Activity.FirstName = User.FirstName, Activity.LastName = User.LastName, Activity.ProfilePicURL = User.ProfilePicURL" +
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
                Status = Activity.StatusEnum.Active,
                Type = "COMMENT_ON_JOURNEY",
                UserID = UserID,
                CommentContent = Comment.Content,
                CommentID = Comment.CommentID,
                JourneyID = Comment.JourneyID,
                Timestamp = DateTimeOffset.Now
            };

            CypherQuery query = new CypherQuery(" CREATE (Comment:Comment {Comment}) " +
                                                " CREATE (Activity:Activity {Activity}) " +
                                                " WITH Comment, Activity " +
                                                " MATCH (Journey:Journey) " +
                                                " WHERE (Journey.JourneyID = {JourneyID}) " +
                                                " CREATE (Comment)-[:ON]->(Journey) " +
                                                " CREATE (Activity)-[:ACT_ON_JOURNEY]->(Journey) " +
                                                " SET Activity.Journey_Name = Journey.Name, Activity.Journey_Description = Journey.Description, Activity.Journey_NumberOfLike = Journey.NumberOfLike, Activity.NumberOfShare = Journey.NumberOfShare" +
                                                " WITH Comment, Activity " +
                                                " MATCH (User:User) " +
                                                " WHERE (User.UserID = {UserID}) " +
                                                " CREATE (Comment)-[:COMMENT_BY]->(User) " +
                                                " SET Activity.UserName = User.UserName, Activity.FirstName = User.FirstName, Activity.LastName = User.LastName, Activity.ProfilePicURL = User.ProfilePicURl" +
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
            var query = Db.Cypher.Match("(Comment:Comment)-[:LIKED_BY]->(user:User)").Where((Comment Comment) => Comment.CommentID == CommentID).Return(user => user.As<User>()).Results;
            return query.Count() == 0 ? null : query.ToList<User>();
        }
        //TODO
        public void DeleteAComment(Guid UserID, Guid CommentID)
        {
            Db.Cypher.OptionalMatch("(Comment:Comment)-[rel:COMMENT_BY]->(User:User)").
                        Where((Comment Comment) => Comment.CommentID == CommentID).
                        AndWhere((User User) => User.UserID == UserID).
                        With("Comment, rel").Where("rel IS NOT NULL").
                        Match("(Comment)-[r]-()").
                        Match("(Activity:Activity)").
                        Where((Activity Activity) => Activity.CommentID == CommentID).
                        Set("Activity.Status = 'Deleted'").
                        With("Comment, r").
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
