using IdentityServer.Entities;
using IdentityServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer.Controllers
{
    [AutoValidateAntiforgeryToken] //Bu sunucunun üretmediği x bir noktadan belirli istekler yapılmayacak (güvenli)
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;

        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View(new UserCreateModel());
        }
        [HttpPost]
        public async Task<IActionResult> Create(UserCreateModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = new()
                {
                    Email = model.Email,
                    Gender = model.Gender,
                    UserName = model.Username
                };

                var identityResult = await _userManager.CreateAsync(appUser, model.Password);
                if (identityResult.Succeeded)
                {
                    await _roleManager.CreateAsync(new()
                    {
                        Name = "Admin",
                        CreatedTime = DateTime.Now
                    });
                    await _userManager.AddToRoleAsync(appUser, "Admin");
                    return RedirectToAction("Index");
                }
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(UserSignInModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, true); //username,password,benihatırla,locklama
                if (result.Succeeded)
                {
                    if (User.IsInRole("Admin"))
                    {
                        return RedirectToAction("AdminPanel")
                    }
                    else
                    {
                        return RedirectToAction("Panel");
                    }
                    return RedirectToAction("Index");
                }
                else if (result.IsLockedOut)
                {
                    //hesap kilitli
                }
                else if (result.IsNotAllowed)
                {
                    //hesap doğrulama yapmamış | email & phone gibi
                }
            }
            return View();
        }
        [Authorize(Roles = "Admin, Member")]
        public IActionResult GetUserInfo()
        {
            var userName = User.Identity.Name;
            var role = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value;
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AdminPanel()
        {

        }
    }
}
