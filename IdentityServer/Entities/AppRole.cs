using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Entities
{
    public class AppRole : IdentityRole<int> //tablo id'si int olsun
    {
        public DateTime CreatedTime { get; set; }
    }
}
