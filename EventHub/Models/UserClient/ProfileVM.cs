using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class ProfileVM
    {
        public ListProfileDetailsVM ListProfile { get; set; }
        public CreateProfileImageVM ImageProfile { get; set; }
    }
}
