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
            Mapper.CreateMap<User, PersonalAboutViewModel>();
            Mapper.CreateMap<Journey, JourneyViewModel>();
            Mapper.CreateMap<Journey, JourneyWidgetViewModel>();
            Mapper.CreateMap<User, PersonalViewModel>();
            Mapper.CreateMap<Destination, DestinationViewModel>();
            Mapper.CreateMap<Destination, DestinationWidgetViewModel>().
                ForMember(x => x.DestinationName, y => y.MapFrom(src => src.Name)); 
            Mapper.CreateMap<Destination, CommentWidgetViewModel>().
                ForMember(x => x.DestinationName, y => y.MapFrom(src => src.Name)); 
            Mapper.CreateMap<Destination, AddPhotoWidgetViewModel>().
                ForMember(x => x.DestinationName, y => y.MapFrom(src => src.Name)); 
            Mapper.CreateMap<Comment, CommentViewModel>().
                ForMember(x => x.UserAvatarURL, y => y.MapFrom(src => src.User.ProfilePicURL));            
            Mapper.CreateMap<User, DestinationViewModel>();
            Mapper.CreateMap<User, FriendItemViewModel>();
            Mapper.CreateMap<Content, AddPhotoWidgetViewModel>();
            Mapper.CreateMap<Activity, NewsfeedBaseWidgetViewModel>();
            Mapper.CreateMap<Activity, AddPhotoWidgetViewModel>();
            Mapper.CreateMap<Activity, CommentWidgetViewModel>();
            Mapper.CreateMap<Activity, ShareWidgetViewModel>();
            Mapper.CreateMap<Activity, PersonalWidgetViewModel>();
            Mapper.CreateMap<Activity, DestinationWidgetViewModel>();
            Mapper.CreateMap<Activity, JourneyWidgetViewModel>();
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