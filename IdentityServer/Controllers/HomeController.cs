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
                    var memberRole = await _roleManager.FindByNameAsync("Member");
                    if (memberRole is null)
                    {
                        await _roleManager.CreateAsync(new()
                        {
                            Name = "Admin",
                            CreatedTime = DateTime.Now
                        });
                    }
                    await _userManager.AddToRoleAsync(appUser, "Member");
                    return RedirectToAction("Index");
                }
                foreach (var error in identityResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        public IActionResult SignIn(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new UserSignInModel { ReturnUrl = returnUrl });
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(UserSignInModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, true); //username,password,benihatırla,locklama
                if (result.Succeeded)
                {
                    if (!string.IsNullOrWhiteSpace(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    var user = await _userManager.FindByNameAsync(model.Username);
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("AdminPanel");
                    }
                    else
                    {
                        return RedirectToAction("Panel");
                    }

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
            return View();
        }
        [Authorize(Roles = "Member")]
        public IActionResult Panel()
        {
            return View();
        }
        [Authorize(Roles = "Member")]
        public IActionResult MemberPage()
        {
            return View();
        }
    }
}
