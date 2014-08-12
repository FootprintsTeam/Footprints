using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Footprints.ViewModels
{
    public class ImageViewModel
    {
        [Required]
        [RegularExpression(Common.Constant.GUID_REGEX)]
        public Guid MasterID { get; set; }
        [Required]
        public String ReturnUrl { get; set; }
    }
}