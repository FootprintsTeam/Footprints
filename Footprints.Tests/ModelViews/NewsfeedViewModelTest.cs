using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Footprints.ViewModels;

namespace Footprints.Tests.ModelViews
{
    [TestClass]
    public class NewsfeedViewModelTest
    {
        [TestMethod]
        public void CheckGetSampleObject() { 
            //arrange            
            var x = NewsfeedPostViewModel.GetSampleObject();
            Assert.IsTrue(x.Count() == 1);
        }
    }
}
