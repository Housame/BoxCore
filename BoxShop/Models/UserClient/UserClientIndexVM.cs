﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoxShop.Models
{
    public class UserClientIndexVM
    {
        public UserDisplayVM[] userDisplay { get; set; }
        public UserSignInVM userSignIn { get; set; }
    }
}
