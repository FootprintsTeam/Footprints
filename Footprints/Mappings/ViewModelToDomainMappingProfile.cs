using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
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
                
        }
    }
}