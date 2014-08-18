using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Footprints.Models;
using Footprints.DAL.Abstract;
using Neo4jClient;

namespace Footprints.DAL.Concrete
{
    public class NewsFeedRepository : RepositoryBase<NewsFeedRepository>, INewsFeedRepository
    {
        private C5.IntervalHeap<Activity> priorityQueue;
        private List<Activity> result;
        private LinkedList<User> friendList = new LinkedList<User>();
        private LinkedList<LinkedList<Activity>> activities = new LinkedList<LinkedList<Activity>>();
        private Activity latestActivity = new Activity(), mostRecentActivity = new Activity();
        private int numberOfFriends, latestFriendPosition, currentFriendPosition;
        public NewsFeedRepository(IGraphClient client) : base(client) { }
        public void LoadEgoNetwork(Guid UserID)
        {
            //SQL-Injection-prone
            var query = Db.Cypher.Match("(User:User)-[ego:EGO* {UserID : User.UserID }]->(friend:User)").
                        Where((User User) => User.UserID == UserID).AndWhere("friend.UserID <> 'TEMP'").
                        Match("(friend)-[:LATEST_ACTIVITY]->(latest_activity:Activity)").
                        Match("(latest_activity)-[:NEXT*]->(next_activity)").
                        Return((friend, latest_activity, next_activity) => new
                        {
                            friend = friend.As<User>(),
                            latest_activity = latest_activity.As<Activity>(),
                            next_activity = next_activity.As<Activity>()
                        }).Results;
            LinkedList<Activity> activity = new LinkedList<Activity>();
            User currentFriend = new User();
            foreach (var item in query)
            {
                if (currentFriend.UserID == item.friend.UserID)
                {
                    activity.AddLast(item.next_activity);
                    // Console.WriteLine(item.next_activity.Timestamp + " " + item.next_activity.UserID);
                }
                else
                {
                    currentFriend = item.friend;
                    if (activity.Count != 0) activities.AddLast(activity);
                    friendList.AddLast(currentFriend);
                    activity = new LinkedList<Activity>();
                    activity.AddLast(item.latest_activity);
                    // Console.WriteLine(item.latest_activity.Timestamp + " " + item.latest_activity.UserID);
                    activity.AddLast(item.next_activity);
                    // Console.WriteLine(item.next_activity.Timestamp + " " + item.next_activity.UserID);
                }
            }
            activities.AddLast(activity);
        }
        public IList<Activity> RetrieveNewsFeed(Guid UserID, int k)
        {
            //If necessary
            LoadEgoNetwork(UserID);
            //Init
            ActivityComparer comparer = new ActivityComparer();
            priorityQueue = new C5.IntervalHeap<Activity>(comparer);
            result = new List<Activity>();

            numberOfFriends = friendList.Count;
            // Add latest activity of closest friend in ego
            if (activities.ElementAt(0).ElementAt(0).Status != Activity.StatusEnum.Deleted)
            {
                result.Add(activities.ElementAt(0).ElementAt(0));
            }            
            // Add next activity of the activity above to priority queue           
            priorityQueue.Add(activities.ElementAt(0).ElementAt(1));
            //Add the latest activity of next friend in ego graph to priority queue
            latestFriendPosition = 0;
            currentFriendPosition = 0;
            latestActivity = (activities.ElementAt(0).ElementAt(1));
            if (numberOfFriends > 1)
            {
                latestActivity = activities.ElementAt(1).ElementAt(0);
                currentFriendPosition = 1;
                priorityQueue.Add(latestActivity);
            }
            while (!priorityQueue.IsEmpty && result.Count < k)
            {
                mostRecentActivity = priorityQueue.FindMax();
                priorityQueue.DeleteMax();
                if (mostRecentActivity.Status != Activity.StatusEnum.Deleted)
                {
                    result.Add(mostRecentActivity);
                }
                var tempActivity = activities.ElementAt(currentFriendPosition).Find(mostRecentActivity);
                if (tempActivity != null && tempActivity.Next != null)
                {
                    priorityQueue.Add(tempActivity.Next.Value);
                }
                else
                {
                    tempActivity = activities.ElementAt(latestFriendPosition).Find(mostRecentActivity);
                    if (tempActivity != null && tempActivity.Next != null)
                    {
                        priorityQueue.Add(tempActivity.Next.Value);
                    }
                }
                if (mostRecentActivity.Timestamp == latestActivity.Timestamp)
                {
                    latestFriendPosition = currentFriendPosition;
                    if (currentFriendPosition < numberOfFriends - 1)
                    {
                        currentFriendPosition++;
                        latestActivity = activities.ElementAt(currentFriendPosition).ElementAt(0);
                        priorityQueue.Add(latestActivity);
                    }
                }
            }
            return result.Count == 0 ? null : result;
        }
        public IList<Activity> LoadMoreNewsfeed(Guid UserID, int l)
        {
            int cnt = 0;
            numberOfFriends = friendList.Count;
            List<Activity> moreNewsfeed = new List<Activity>();
            while (!priorityQueue.IsEmpty && cnt < l)
            {
                mostRecentActivity = priorityQueue.FindMax();
                priorityQueue.DeleteMax();
                if ((mostRecentActivity != null) && (mostRecentActivity.Status != Activity.StatusEnum.Deleted))
                {
                    moreNewsfeed.Add(mostRecentActivity);
                }
                cnt++;
                var tempActivity = activities.ElementAt(currentFriendPosition).Find(mostRecentActivity);
                if (tempActivity != null && tempActivity.Next != null)
                {
                    priorityQueue.Add(tempActivity.Next.Value);
                }
                else
                {
                    tempActivity = activities.ElementAt(latestFriendPosition).Find(mostRecentActivity);
                    if (tempActivity != null && tempActivity.Next != null)
                    {
                        priorityQueue.Add(tempActivity.Next.Value);
                    }
                }
                if (mostRecentActivity.Timestamp == latestActivity.Timestamp)
                {
                    latestFriendPosition = currentFriendPosition;
                    if (currentFriendPosition < numberOfFriends - 1)
                    {
                        currentFriendPosition++;
                        latestActivity = activities.ElementAt(currentFriendPosition).ElementAt(0);
                        priorityQueue.Add(latestActivity);
                    }
                }
            }
            return moreNewsfeed.Count == 0 ? null : moreNewsfeed;
        }
    }
    public class ActivityComparer : Comparer<Activity>
    {
        public override int Compare(Activity x, Activity y)
        {
            return x.Timestamp.CompareTo(y.Timestamp);
        }
    }
    public interface INewsFeedRepository
    {
        void LoadEgoNetwork(Guid UserID);
        IList<Activity> RetrieveNewsFeed(Guid UserID, int k);
        IList<Activity> LoadMoreNewsfeed(Guid UserID, int l);
    }
}