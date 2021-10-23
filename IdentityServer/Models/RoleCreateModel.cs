using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Models
{
    public class RoleCreateModel
    {
        [Required(ErrorMessage ="Name Is Required")]
        public string Name { get; set; }
    }
}
