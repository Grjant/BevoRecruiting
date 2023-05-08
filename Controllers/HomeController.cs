using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using sp23Team13FinalProject.Models;
using sp23Team13FinalProject.Models.ViewModels;
using System;

namespace sp23Team13FinalProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMemoryCache _cache;

        public HomeController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IMemoryCache cache)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            // Get the current date from cache or set it to today's date
            DateTime currentDate = GlobalModel.GlobalDate;
            ViewBag.CurrentDate = currentDate;

            return View();
        }

        [HttpPost]
        public IActionResult UpdateDate(DateTime newDate)
        {
            // Update the current date in the GlobalModel class
            GlobalModel.GlobalDate = newDate;

            // Redirect back to the index page
            return RedirectToAction("Index");
        }

        //public async Task<IActionResult> Index()
        //{
        //    // Get the current date from session or set it to today's date
        //    var globalModel = _cache.Get<GlobalModel>("GlobalModel");
        //    if (globalModel == null)
        //    {
        //        globalModel = new GlobalModel { GlobalDate = DateOnly.FromDateTime(DateTime.Today) };
        //        _cache.Set("GlobalModel", globalModel);
        //    }

        //    // Pass the current date to the view
        //    ViewBag.CurrentDate = globalModel.GlobalDate;

        //    return View();
        //}


        //[HttpPost]
        //public IActionResult UpdateDate(DateOnly newDate)
        //{
        //    // Update the current date in the cache
        //    var globalModel = _cache.Get<GlobalModel>("GlobalModel");
        //    if (globalModel == null)
        //    {
        //        globalModel = new GlobalModel { GlobalDate = newDate };
        //    }
        //    else
        //    {
        //        globalModel.GlobalDate = newDate;
        //    }
        //    _cache.Set("GlobalModel", globalModel);

        //    // Redirect back to the index page
        //    return RedirectToAction("Index");
        //}




        //without converting
        //public async Task<IActionResult> Index()
        //{
        //    // Get the current date from cache or set it to today's date
        //    if (!_cache.TryGetValue<DateTime>("CurrentDate", out var currentDate))
        //    {
        //        currentDate = DateTime.Today;
        //        _cache.Set("CurrentDate", currentDate);
        //    }

        //    // Pass the current date to the view
        //    ViewBag.CurrentDate = currentDate;

        //    return View();
        //}




        //     if (!_signInManager.IsSignedIn(User))
        //            {
        //                return RedirectToAction("Login", "Account"); 
        //            }
        //            else
        //            {
        //                var user = await _userManager.GetUserAsync(User);
        //                if (await _userManager.IsInRoleAsync(user, "CSO"))
        //                {
        //                    var model = new SplashViewModel
        //                    {
        //                        FirstName = user.FirstName
        //                    };
        //                    return View("Index", model);
        //                }
        //                else if (await _userManager.IsInRoleAsync(user, "Recruiter"))
        //                {
        //                    var model = new SplashViewModel
        //                    {
        //                        FirstName = user.FirstName
        //                    };
        //                    return View("Index", model);
        //                }
        //                else if (await _userManager.IsInRoleAsync(user, "Student"))
        //                {
        //                    var model = new SplashViewModel
        //                    {
        //                        FirstName = user.FirstName
        //                    };
        //                    return View("Index", model);
        //                }
        //                else
        //                {
        //                    return View();
        //                }
        //            }

    }
}
