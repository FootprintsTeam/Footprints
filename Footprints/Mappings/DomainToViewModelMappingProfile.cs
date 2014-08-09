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
            Mapper.CreateMap<User, PersonalViewModel>();
            Mapper.CreateMap<Destination, DestinationViewModel>();
            Mapper.CreateMap<Comment, CommentViewModel>();
            Mapper.CreateMap<User, DestinationViewModel>();
        }
    }
}