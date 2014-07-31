using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Footprints.DAL.Abstract;
using Footprints.DAL.Concrete;
using Footprints.Models;
using Footprints.Services;
using Neo4jClient;

namespace Footprints.Tests.DITest
{
    [TestClass]
    public class NewsfeedTest
    {
        GraphClient client;
        NewsFeedRepository newsfeedRepository;
        public NewsfeedTest()
        {
            client = new GraphClient(new Uri("http://54.255.155.78:7474/db/data"));
            client.Connect();
            newsfeedRepository = new NewsFeedRepository(client);
        }
        [TestMethod]
        public void LoadEgoNetWork()
        {
            var x = DateTimeOffset.Now;
            newsfeedRepository.LoadEgoNetwork(new Guid("ad49da1f-0481-4625-b906-66fbb2152474"));
        }

        [TestMethod]
        public void RetrieveNewsfeed() {
            newsfeedRepository.RetrieveNewsFeed(new Guid("ad49da1f-0481-4625-b906-66fbb2152474"), 5);            
        }

        [TestMethod]
        public void LoadMoreNewsfeed() {
            newsfeedRepository.LoadMoreNewsfeed(new Guid("ad49da1f-0481-4625-b906-66fbb2152474"), 5);
        }
    }
}
