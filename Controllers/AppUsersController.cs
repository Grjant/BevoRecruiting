using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sp23Team13FinalProject.DAL;
using sp23Team13FinalProject.Models;
using sp23Team13FinalProject.Models.ViewModels;
using sp23Team13FinalProject.Utilities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace sp23Team13FinalProject.Controllers
{
    public class AppUsersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AppUsersController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //Aryan you can change this! - Manalie
        [Authorize(Roles = "CSO, Recruiter")]
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .Include(u => u.Company)
                .Where(u => !string.IsNullOrEmpty(u.Email)).ToListAsync();
            return View(users);
        }

        [Authorize(Roles = "CSO, Recruiter")]
        public async Task<IActionResult> StudentSearch(string search)
        {
            var userId = User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
            var appUsers = _context.Users
                .Include(i => i.Major)
                .Where(u => u.Id != userId); //exclude the current user from the list
            var appUsersInRole = from user in appUsers
                                 join userRole in _context.UserRoles
                                 on user.Id equals userRole.UserId
                                 join role in _context.Roles
                                 on userRole.RoleId equals role.Id
                                 where role.Name == "Student" && user.ActiveStatus == true
                                 select user;

            if (string.IsNullOrEmpty(search))
            {
                ViewBag.AllUsers = _context.Users.Count();
                ViewBag.SelectedUsers = appUsersInRole.Count();

                return View(appUsersInRole);
            }

            appUsersInRole = appUsersInRole
             .Where(u => u.FirstName.ToLower().Contains(search.ToLower()) ||
             u.LastName.ToLower().Contains(search.ToLower()) ||
             u.Major.MajorName.Contains(search.ToLower()));
            //u.GraduationDate.Value.ToString("yyyy-MM-dd").Contains(search));

            List<AppUser> selectedUsers = appUsersInRole.ToList();

            ViewBag.AllUsers = _context.Users.Count();
            ViewBag.SelectedUsers = selectedUsers.Count();

            return View(selectedUsers);
        }


        [Authorize(Roles = "CSO, Recruiter")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var appUser = await _context.Users
                .Include(u => u.Major)
                .Include(u => u.Major) // added
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        [Authorize(Roles = "CSO")]
        public IActionResult CreateRecruiter()
        {
            ViewBag.AllCompanies = GetAllCompaniesSelectList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRecruiterProfile(CreateRecruiterProfileViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var aum = new AddUserModel()
                {
                    User = new AppUser()
                    {
                        UserName = viewModel.Email,
                        Email = viewModel.Email,
                        FirstName = viewModel.FirstName,
                        LastName = viewModel.LastName,
                        ActiveStatus = true,
                        Company = _context.Companies.First(x => x.CompanyID == viewModel.SelectedCompanyID),
                    },
                    Password = viewModel.Password,
                    RoleName = "Recruiter"
                };

                // save the new user to the database
                await Utilities.AddUser.AddUserWithRoleAsync(aum, _userManager, _context);

                // redirect to the index action of the AppUsers controller
                return RedirectToAction(nameof(AppUsersController.Index));
            }

            // if the model state is not valid, redisplay the form with validation errors
            ViewBag.AllCompanies = new SelectList(_context.Companies, "CompanyID", "CompanyName");
            return View("CreateRecruiter", viewModel);
        }


        [Authorize(Roles = "CSO")]
        public IActionResult CreateCSO()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCSOProfile(CreateProfileViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var aum = new AddUserModel()
                {
                    User = new AppUser()
                    {
                        UserName = viewModel.Email,
                        Email = viewModel.Email,
                        FirstName = viewModel.FirstName,
                        LastName = viewModel.LastName,
                    },
                    Password = viewModel.Password,
                    RoleName = "CSO"
                };

                // save the new user to the database
                await Utilities.AddUser.AddUserWithRoleAsync(aum, _userManager, _context);

                // redirect to the index action of the AppUsers controller
                return RedirectToAction(nameof(AppUsersController.Index));
            }

            // if the model state is not valid, redisplay the form with validation errors
            return View("CreateCSOProfile", viewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var appUser = await _context.Users
             .Include(u => u.Major)
             .Include(u => u.Company)// Include the navigational properties
             .FirstOrDefaultAsync(u => u.Id == id);
            if (appUser == null)
            {
                return NotFound();
            }

            if (await _userManager.IsInRoleAsync(appUser, "Student"))
            {
                var viewModel = new EditStudentProfileViewModel
                {
                    FirstName = appUser.FirstName,
                    LastName = appUser.LastName,
                    Email = appUser.Email,
                    ActiveStatus = appUser.ActiveStatus,
                    SelectedMajorID = appUser.Major.MajorID,
                    PositionType = appUser.PositionType,
                    GraduationDate = appUser.GraduationDate,
                    GPA = appUser.GPA,
                };
                ViewBag.AllMajors = GetAllMajorsSelectList();
                return View("EditStudentProfile", viewModel);
            }

            else if (await _userManager.IsInRoleAsync(appUser, "Recruiter"))
            {
                var viewModel = new EditRecruiterProfileViewModel
                {
                    FirstName = appUser.FirstName,
                    LastName = appUser.LastName,
                    Email = appUser.Email,
                    ActiveStatus = appUser.ActiveStatus,
                    SelectedCompanyID = appUser.Company?.CompanyID,
                    CompanyName = appUser.Company?.CompanyName
                };
                ViewBag.AllCompanies = GetAllCompaniesSelectList();
                return View("EditRecruiterProfile", viewModel);
            }
            else
            {
                var viewModel = new EditProfileViewModel
                {
                    FirstName = appUser.FirstName,
                    LastName = appUser.LastName,
                    Email = appUser.Email
                };
                return View("EditProfile", viewModel);
            }
        }

        // POST: AppUsers/EditStudentProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudentProfile(string id, EditStudentProfileViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AllMajors = GetAllMajorsSelectList();
                return View("Edit", viewModel);
            }

            var appUser = await _context.Users
                .Include(u => u.Major)
                .FirstOrDefaultAsync(u => u.Id == viewModel.Id);

            if (appUser == null)
            {
                return NotFound();
            }

            appUser.FirstName = viewModel.FirstName;
            appUser.LastName = viewModel.LastName;
            appUser.Email = viewModel.Email;
            appUser.UserName = appUser.Email;
            appUser.NormalizedEmail = appUser.Email.ToUpper();
            appUser.NormalizedUserName = appUser.NormalizedEmail;
            appUser.ActiveStatus = viewModel.ActiveStatus;
            appUser.Major = await _context.Majors.FindAsync(viewModel.SelectedMajorID);
            appUser.PositionType = viewModel.PositionType;
            appUser.GraduationDate = viewModel.GraduationDate;
            appUser.GPA = viewModel.GPA;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRecruiterProfile(string id, EditRecruiterProfileViewModel viewModel)
        {
            if (id == null || viewModel == null)
            {
                return NotFound();
            }

            var appUser = await _context.Users
                .Include(u => u.Company) // Include the Company navigational property
                .FirstOrDefaultAsync(u => u.Id == id);

            if (appUser == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                appUser.FirstName = viewModel.FirstName;
                appUser.LastName = viewModel.LastName;
                appUser.ActiveStatus = viewModel.ActiveStatus;
                if (User.IsInRole("CSO"))
                {
                    appUser.Company = _context.Companies.First(x => x.CompanyID == viewModel.SelectedCompanyID);
                }
                appUser.Email = viewModel.Email;
                appUser.UserName = appUser.Email;
                appUser.NormalizedEmail = appUser.Email.ToUpper();
                appUser.NormalizedUserName = appUser.NormalizedEmail;

                await _userManager.UpdateAsync(appUser);

                return RedirectToAction("Index", "Home");
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(string id, EditProfileViewModel viewModel)
        {
            if (id == null || viewModel == null)
            {
                return NotFound();
            }

            var appUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (appUser == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                appUser.FirstName = viewModel.FirstName;
                appUser.LastName = viewModel.LastName;
                appUser.Email = viewModel.Email;
                appUser.UserName = appUser.Email;
                appUser.NormalizedEmail = appUser.Email.ToUpper();
                appUser.NormalizedUserName = appUser.NormalizedEmail;

                await _userManager.UpdateAsync(appUser);

                return RedirectToAction("Index", "Home");
            }

            return View(viewModel);
        }


        private bool AppUserExists(string id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private SelectList GetAllMajorsSelectList()
        {
            //Get the list of months from the database
            List<Major> majorList = _context.Majors.ToList();

            //add a dummy entry so the user can select all months
            //Major SelectNone = new Major() { MajorID = 0, MajorName = "All Majors" };
            //majorList.Add(SelectNone);

            //convert the list to a SelectList by calling SelectList constructor
            //MonthID and MonthName are the names of the properties on the Month class
            //MonthID is the primary key
            SelectList majorSelectList = new SelectList(majorList.OrderBy(m => m.MajorID), "MajorID", "MajorName");

            //return the SelectList
            return majorSelectList;
        }

        private SelectList GetAllCompaniesSelectList()
        {
            //Get the list of months from the database
            List<Company> companyList = _context.Companies.ToList();

            //add a dummy entry so the user can select all months
            //Major SelectNone = new Major() { MajorID = 0, MajorName = "All Majors" };
            //majorList.Add(SelectNone);

            //convert the list to a SelectList by calling SelectList constructor
            //MonthID and MonthName are the names of the properties on the Month class
            //MonthID is the primary key
            SelectList companySelectList = new SelectList(companyList.OrderBy(m => m.CompanyID), "CompanyID", "CompanyName");

            //return the SelectList
            return companySelectList;
        }
    }
}
