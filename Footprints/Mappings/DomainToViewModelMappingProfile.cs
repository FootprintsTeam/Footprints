using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
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
            
        }
    }
}