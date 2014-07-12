using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.Models;
using Footprints.DAL.Abstract;
using Neo4jClient;
using C5;

namespace Footprints.DAL.Concrete
{
    public class NewsFeedRepository : RepositoryBase<NewsFeedRepository>, INewsFeedRepository
    {
        private IntervalHeap<Activity> priorityQueue;
        private System.Collections.Generic.HashSet<Activity> result;

        private List<User> friendList = new List<User>();
        private List<List<Activity>> activities = new List<List<Activity>>();
        public NewsFeedRepository(IGraphClient client) : base(client) { }
        public void loadEgoNetwork(Guid userID)
        {
            String egoEdges = "ego" + userID;
            List<Activity> activity;
            var query = Db.Cypher.Match("(user:User )-[: " + egoEdges + "*]->(friend)-[:LATEST_ACTIVITY]->(latest_activity)-[:NEXT*]->(next_activity)").Where("user.userID = {userID}").WithParams(new { userID }).
                    Return((friend, latest_activity, next_activity) => new
                    {
                        friends = friend.As<User>(),
                        latest_activity = latest_activity.As<Activity>(),
                        next_activities = next_activity.CollectAs<Activity>()
                    }).OrderBy("latest_activity.timestamp").Results;
            foreach (var item in query)
            {
                friendList.Add(item.friends);
                activity = new List<Activity>();
                activity.Add(item.latest_activity);
                foreach (var i in item.next_activities)
                {
                    activity.Add(i.Data);
                }
                activities.Add(activity);
            }
        }

        public void RetrieveNewsFeed(Guid userId, int k)
        {

        }

    }

    public interface INewsFeedRepository
    {
        void loadEgoNetwork(Guid userID);
        void RetrieveNewsFeed(Guid userId, int k);
    }
}