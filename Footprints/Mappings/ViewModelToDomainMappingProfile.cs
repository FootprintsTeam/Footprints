using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Footprints.ViewModels;
using Footprints.Models;

namespace Footprints.Mappings
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public override string ProfileName
        {
            get { return "ViewModelToDomainMappings"; }
        }

        protected override void Configure()
        {
            Mapper.CreateMap<AddNewJourneyViewModel, Journey>();
            Mapper.CreateMap<AddNewDestinationFormViewModel, Place>();
            Mapper.CreateMap<AddNewDestinationFormViewModel, Destination>();
            Mapper.CreateMap<EditDestinationFormViewModel, Place>();
            Mapper.CreateMap<EditDestinationFormViewModel, Destination>();
            Mapper.CreateMap<EditJourneyViewModel, Journey>();
			Mapper.CreateMap<CommentViewModel, Comment>();
            Mapper.CreateMap<DestinationViewModel, EditDestinationFormViewModel>();
            Mapper.CreateMap<DestinationViewModel, DestinationInfoOnMapViewModel>();
        }
    }
}