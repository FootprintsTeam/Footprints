using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Footprints.Models;
using Footprints.ViewModels;
namespace Footprints.Mappings
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "DomainToViewModelMappings"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<Journey, JourneyViewModel>();
            Mapper.CreateMap<Journey, JourneyWidgetViewModel>().ForMember(x => x.Type, opt => opt.Ignore());;
            Mapper.CreateMap<User, PersonalViewModel>();
            Mapper.CreateMap<Destination, DestinationViewModel>();
            Mapper.CreateMap<Destination, DestinationWidgetViewModel>().
                ForMember(x => x.DestinationName, y => y.MapFrom(src => src.Name));
            Mapper.CreateMap<Destination, CommentWidgetViewModel>().
                ForMember(x => x.DestinationName, y => y.MapFrom(src => src.Name));
            Mapper.CreateMap<Destination, AddPhotoWidgetViewModel>().
                ForMember(x => x.DestinationName, y => y.MapFrom(src => src.Name));
            Mapper.CreateMap<Comment, CommentViewModel>().
                ForMember(x => x.UserAvatarURL, y => y.MapFrom(src => src.User.ProfilePicURL)).
                ForMember(x => x.UserID, y => y.MapFrom(src => src.User.UserID)).
                ForMember(x => x.UserName, y => y.MapFrom(src => src.User.UserName));
            Mapper.CreateMap<User, DestinationViewModel>();
            Mapper.CreateMap<User, FriendItemViewModel>();
            Mapper.CreateMap<Content, AddPhotoWidgetViewModel>();
            Mapper.CreateMap<Activity, NewsfeedBaseWidgetViewModel>();
            Mapper.CreateMap<Activity, AddFriendWidgetViewmodel>();
            Mapper.CreateMap<Activity, AddPhotoWidgetViewModel>().
                ForMember(x => x.DestinationName, y => y.MapFrom(src => src.Destination_Name)).
                ForMember(x => x.URL, y => y.MapFrom(src => src.ContentURL));
            Mapper.CreateMap<Activity, CommentWidgetViewModel>().
                ForMember(x => x.DestinationName, y => y.MapFrom(src => src.Destination_Name)).
                ForMember(x=>x.Description,y=>y.MapFrom(src=>src.Destination_Description)).
                ForMember(x => x.Content, y => y.MapFrom(src => src.CommentContent));
            Mapper.CreateMap<Activity, ShareWidgetViewModel>().
                ForMember(x => x.DestinationName, y => y.MapFrom(src => src.Destination_Name));
            Mapper.CreateMap<Activity, PersonalWidgetViewModel>();
            Mapper.CreateMap<Activity, DestinationWidgetViewModel>().
                ForMember(x => x.DestinationName, y => y.MapFrom(src => src.Destination_Name)).
                ForMember(x => x.Description, y => y.MapFrom(src => src.Destination_Description));
            Mapper.CreateMap<Activity, JourneyWidgetViewModel>().
                ForMember(x => x.JourneyName, y => y.MapFrom(src => src.Journey_Name));
            Mapper.CreateMap<Activity, Place>().
                ForMember(x => x.Name, y => y.MapFrom(src => src.Place_Name)).
                ForMember(x => x.Address, y => y.MapFrom(src => src.Place_Address));
            //Mapper.CreateMap<User, NewsfeedBaseWidgetViewModel>();
            //Mapper.CreateMap<User, AddPhotoWidgetViewModel>();
            //Mapper.CreateMap<User, CommentWidgetViewModel>();
            //Mapper.CreateMap<User, ShareWidgetViewModel>();
            //Mapper.CreateMap<User, PersonalWidgetViewModel>();
            //Mapper.CreateMap<User, DestinationWidgetViewModel>();
            //Mapper.CreateMap<User, JourneyWidgetViewModel>();
            Mapper.CreateMap<User, PersonalWidgetViewModel>();
            Mapper.CreateMap<User, NewsfeedBaseWidgetViewModel>();

        }
    }
}