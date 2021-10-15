using IdentityServer.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View(new UserCreateModel());
        }
        [HttpPost]
        public IActionResult Create(UserCreateModel model)
        {
            if (ModelState.IsValid)
            {

            }
            return View(model);
        }
    }
}
