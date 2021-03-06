﻿using Footprints.Common;
using Footprints.Models;
using Footprints.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Footprints.Controllers
{
    public class SearchController : Controller
    {
        ISearch search;
        public SearchController(ISearch search)
        {
            this.search = search;
        }
        int NumberOfResultPerBlock = 10;
        public ActionResult Index(SearchDataViewModel dataModel)
        {
            SearchViewModel viewModel = new SearchViewModel();
            //Search place by default
            if (dataModel.SearchTypes == null || dataModel.SearchTypes.Count == 0)
            {
                dataModel.SearchTypes = new List<SearchType> {
                    SearchType.place
                };
            }
            if (dataModel.SearchTypes.Contains(SearchType.user))
            {
                viewModel.Users = search.SearchUser(dataModel.Keyword, NumberOfResultPerBlock);
                if (viewModel.Users != null)
                {
                    viewModel.Users = viewModel.Users.Distinct(new UserEqualityComparer()).ToList();
                }
            }
            viewModel.Keyword = dataModel.Keyword;
            viewModel.SearchTypes = dataModel.SearchTypes;
            if (dataModel.SearchTypes.Contains(SearchType.place))
            {
                List<Journey> journeyList = new List<Journey>();
                viewModel.Keyword = dataModel.Keyword;
                viewModel.Destinations = search.SearchDestination(dataModel.Keyword, NumberOfResultPerBlock);
                if (viewModel.Destinations != null)
                {
                    viewModel.Destinations = viewModel.Destinations.Distinct(new DestinationEqualityComparer()).ToList();
                }
                viewModel.Journeys = (List<Journey>)search.SearchJourney(dataModel.Keyword, NumberOfResultPerBlock);
                if (viewModel.Journeys == null)
                {
                    viewModel.Journeys = new List<Journey>();
                }
                else
                {
                    journeyList.AddRange(viewModel.Journeys);
                }
                viewModel.Places = (List<Journey>)search.SearchPlace(dataModel.Keyword, NumberOfResultPerBlock);
                if (viewModel.Places != null)
                {
                    journeyList.AddRange(viewModel.Places);
                }
                viewModel.Journeys = journeyList.Distinct(new JourneyEqualityComparer()).ToList();
                viewModel.Places = null;
            }
            return View(viewModel);
        }

        protected String RenderPartialViewToString(String viewName, object model)
        {
            if (String.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");
            ViewData.Model = model;
            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult =
                ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext viewContext = new ViewContext
                (ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }

        public ActionResult LazySearch(SearchDataViewModel dataModel)
        {
            InfiniteScrollJsonModel jsonModel = new InfiniteScrollJsonModel();
            if (dataModel.SearchTypes != null && dataModel.SearchTypes.Count > 0)
            {
                bool isEmptyResult = true;
                if (dataModel.SearchTypes.Contains(SearchType.user))
                {
                    IList<User> users = search.SearchUser(dataModel.Keyword, NumberOfResultPerBlock);
                    if (users != null)
                    {
                        users = users.Distinct(new UserEqualityComparer()).ToList();
                        isEmptyResult = false;
                        jsonModel.HTMLString += RenderPartialViewToString("UserList", users);
                    }
                }
                if (dataModel.SearchTypes.Contains(SearchType.place))
                {
                    IList<Destination> destinations = search.SearchDestination(dataModel.Keyword, NumberOfResultPerBlock);
                    if (destinations != null)
                    {
                        destinations = destinations.Distinct(new DestinationEqualityComparer()).ToList();
                        jsonModel.HTMLString += RenderPartialViewToString("DestinationList", destinations);
                    }
                    List<Journey> journeyList = new List<Journey>();
                    var tmpJourneyList = search.SearchJourney(dataModel.Keyword, NumberOfResultPerBlock);
                    if ( tmpJourneyList!= null)
                    {
                        journeyList.AddRange(tmpJourneyList);
                    }
                    tmpJourneyList = search.SearchPlace(dataModel.Keyword, NumberOfResultPerBlock);
                    if (tmpJourneyList != null)
                    {
                        journeyList.AddRange(tmpJourneyList);
                    }
                    if (journeyList.Count > 0)
                    {
                        isEmptyResult = false;
                        journeyList = journeyList.Distinct(new JourneyEqualityComparer()).ToList();
                        jsonModel.HTMLString += RenderPartialViewToString("JourneyList", journeyList);
                    }
                    if (isEmptyResult)
                    {
                        jsonModel.HTMLString += RenderPartialViewToString("NoResultWidGet", null);
                    }
                }
            }
            return Json(jsonModel, JsonRequestBehavior.DenyGet);
        }

        [ChildActionOnly]
        public ActionResult DestinationList(IEnumerable<Destination> model)
        {
            return PartialView("DestinationList", model);
        }
        [ChildActionOnly]
        public ActionResult JourneyList(IEnumerable<Journey> model)
        {
            return PartialView("JourneyList", model);
        }
        public ActionResult UserList(IEnumerable<User> model)
        {
            return PartialView("UserList", model);
        }
        [ChildActionOnly]
        public ActionResult NoResultWidget()
        {
            return View();
        }
        public ActionResult LazyLoadSearchResult(string Keyword, int BlockNumber)
        {
            InfiniteScrollSearchResultJsonModel result = new InfiniteScrollSearchResultJsonModel();
            result.JourneyNoMoreData = true;
            result.DestinationNoMoreData = true;
            result.PlaceNoMoreData = true;
            result.UserNoMoreData = true;
            IList<Journey> journeyList = search.SearchJourney(Keyword, BlockNumber * NumberOfResultPerBlock);
            if (journeyList != null && journeyList.Count > 0)
            {
                result.HTMLString += RenderPartialViewToString("JourneyList", journeyList);
                result.JourneyNoMoreData = false;
            }
            IList<Destination> destinationList = search.SearchDestination(Keyword, BlockNumber * NumberOfResultPerBlock);
            if (destinationList != null && destinationList.Count > 0)
            {
                result.HTMLString += RenderPartialViewToString("DestinationList", destinationList);
                result.DestinationNoMoreData = false;
            }

            IList<Journey> placeList = search.SearchPlace(Keyword, BlockNumber * NumberOfResultPerBlock);
            if (placeList != null && placeList.Count > 0)
            {
                result.HTMLString += RenderPartialViewToString("JourneyList", placeList);
                result.PlaceNoMoreData = false;
            }

            IList<User> userList = search.SearchUser(Keyword, BlockNumber * NumberOfResultPerBlock);
            if (userList != null && userList.Count > 0)
            {
                result.HTMLString += RenderPartialViewToString("UserList", userList);
                result.UserNoMoreData = false;
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
    }
}