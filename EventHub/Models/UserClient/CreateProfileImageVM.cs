using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models
{
    [Bind(Prefix = nameof(ProfileVM.ImageProfile))]
    public class CreateProfileImageVM
    {
        [Display(Name = "Ändra profilbild")]
        [DataType(DataType.Upload)]
        public IFormFile Image { get; set; }
    }
}
