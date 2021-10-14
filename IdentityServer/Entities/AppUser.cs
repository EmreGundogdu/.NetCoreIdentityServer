using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Entities
{
    public class AppUser : IdentityUser<int> //int id
    {
        public string ImagePath { get; set; }
        public string Gender { get; set; }
    }
}
