using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Models
{
    public class UserCreateModel
    {        
        [Required(ErrorMessage ="Username is required")]
        public string Username { get; set; }
        [EmailAddress(ErrorMessage ="You have to enter type of Email")]
        [Required(ErrorMessage ="Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Password is required")]
        public string Password { get; set; }
        [Compare("Password",ErrorMessage ="Passwords are can not match matched")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage ="Gender is required")]
        public string Gender { get; set; }
    }
}
