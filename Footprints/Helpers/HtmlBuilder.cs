using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Footprints.ViewModels;
using Footprints.Common;
using System.IO;

namespace Footprints.Helpers
{
    public static class HtmlBuilder
    {
        public static string GetPersonalPartialViewName(this NewsfeedBaseWidgetViewModel activity) {
            switch (activity.Type)
            {
                case Constant.ActivityAddNewContent:
                    return "PersonalAddPhotoWidget";                    
                    

                case Constant.ActivityAddNewDestination:
                    return "PersonalDestinationWidget";                    
                    

                case Constant.ActivityAddNewFriend:
                    //Html.RenderPartial("AddFriendWidget", activity);
                    break;

                case Constant.ActivityAddnewJourney:
                    return "PersonalJourneyWidget";                    
                    

                case Constant.ActivityComment:
                    return "PersonalCommentWidget";                    
                    

                case Constant.ActivityShareDestination:
                    return "PersonalShareWidget";                    
                default:
                    System.Diagnostics.Debug.WriteLine(activity.Type + "something");
                    return "false";                    
            }
            return "false";
        }
    }
}