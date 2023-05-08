using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

//TODO: Change these using statements to match your project
using sp23Team13FinalProject.DAL;
using sp23Team13FinalProject.Models;
using sp23Team13FinalProject.Utilities;

//TODO: Change this namespace to match your project
namespace sp23Team13FinalProject.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly PasswordValidator<AppUser> _passwordValidator;
        private readonly AppDbContext _context;

        public AccountController(AppDbContext appDbContext, UserManager<AppUser> userManager, SignInManager<AppUser> signIn)
        {
            _context = appDbContext;
            _userManager = userManager;
            _signInManager = signIn;
            //user manager only has one password validator
            _passwordValidator = (PasswordValidator<AppUser>)userManager.PasswordValidators.FirstOrDefault();
        }

        // GET: /Account/Register
        [AllowAnonymous]
        public IActionResult Register()
        {
            ViewBag.AllMajors = GetAllMajorsSelectList();
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel rvm)
        {
            //if registration data is invalid, return the user to the register page to try again
            if (!ModelState.IsValid)
            {
                ViewBag.AllMajors = GetAllMajorsSelectList();
                return View(rvm);
            }

            // Check if a valid major was selected
            if (rvm.SelectedMajorID == 0 || !_context.Majors.Any(m => m.MajorID == rvm.SelectedMajorID))
            {
                // Add a custom error message to ModelState
                ModelState.AddModelError("SelectedMajorID", "The Major field is required.");
                // Repopulate the majors dropdown
                ViewBag.AllMajors = GetAllMajorsSelectList();
                // Return the view with the validation error
                return View(rvm);
            }
                
            

            //this code maps the RegisterViewModel to the AppUser domain model
            AppUser newUser = new AppUser
            {
                UserName = rvm.Email,
                Email = rvm.Email,

                //TODO: Add the rest of the custom user fields here
                //FirstName is included as an example
                FirstName = rvm.FirstName,
                LastName = rvm.LastName,
                Major = _context.Majors.First(m => m.MajorID == rvm.SelectedMajorID),
                PositionType = (PositionType)rvm.PositionType,
                GraduationDate = rvm.GraduationDate,
                GPA = rvm.GPA
                
        };

            //create AddUserModel
            AddUserModel aum = new AddUserModel()
            {
                User = newUser,
                Password = rvm.Password,

                //TODO: You will need to change this value if you want to 
                //add the user to a different role - just specify the role name.
                RoleName = "Student"
            };


        //This code uses the AddUser utility to create a new user with the specified password
        IdentityResult result = await Utilities.AddUser.AddUserWithRoleAsync(aum, _userManager, _context);

            if (result.Succeeded) //everything is okay
            { 
                //NOTE: This code logs the user into the account that they just created
                //You may or may not want to log a user in directly after they register - check
                //the business rules!
                Microsoft.AspNetCore.Identity.SignInResult result2 = await _signInManager.PasswordSignInAsync(rvm.Email, rvm.Password, false, lockoutOnFailure: false);

                //Send the user to the home page
                return RedirectToAction("Index", "Home");
            }
            else  //the add user operation didn't work, and we need to show an error message
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                //send user back to page with errors
                return View(rvm);
            }
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated) //user has been redirected here from a page they're not authorized to see
            {
                return View("Error", new string[] { "Access Denied" });
            }
            _signInManager.SignOutAsync(); //this removes any old cookies hanging around
            ViewBag.ReturnUrl = returnUrl; //pass along the page the user should go back to
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel lvm, string returnUrl)
        {

            var userloggingin = await _userManager.FindByEmailAsync(lvm.Email);
            if (userloggingin != null && !userloggingin.ActiveStatus)
            {
                ModelState.AddModelError("", "Your account is currently inactive. Please contact an administrator to reactivate your account.");
                return View(lvm);
            }
            //if user forgot to include user name or password,
            //send them back to the login page to try again

            //makes the login button not work
            //if (ModelState.IsValid == false)
            //{
            //    return View(lvm);
            //}

            //attempt to sign the user in using the SignInManager
            Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(lvm.Email, lvm.Password, lvm.RememberMe, lockoutOnFailure: false);

            //if the login worked, take the user to either the url
            //they requested OR the homepage if there isn't a specific url
            if (result.Succeeded)
            {
                //return ?? "/" means if returnUrl is null, substitute "/" (home)
                return Redirect(returnUrl ?? "/");

                var user = await _userManager.FindByEmailAsync(lvm.Email);
                //if (await _userManager.IsInRoleAsync(user, "CSO"))
                //{
                //    return RedirectToAction("Index", "Home");
                //}
                //else if (await _userManager.IsInRoleAsync(user, "Recruiter"))
                //{
                //    return RedirectToAction("Index", "Home");
                //}
                //else if (await _userManager.IsInRoleAsync(user, "Student"))
                //{
                //    return RedirectToAction("Index", "Home");
                //}
                //else // user is not in any of the roles
                //{
                //    //add an error to the model to show invalid attempt
                //    ModelState.AddModelError("", "Invalid login attempt.");
                //    //send user back to login page to try again
                //    return View(lvm);
                //}
            }
            else //log in was not successful
            {
                //add an error to the model to show invalid attempt
                ModelState.AddModelError("", "Invalid login attempt.");
                //send user back to login page to try again
                return View(lvm);
            }
        }

        public IActionResult AccessDenied()
        {
            return View("Error", new string[] { "You are not authorized for this resource" });
        }

        //GET: Account/Index
        public IActionResult Index()
        {
            IndexViewModel ivm = new IndexViewModel();

            //get user info
            String id = User.Identity.Name;
            AppUser user = _context.Users.FirstOrDefault(u => u.UserName == id);

            //populate the view model
            //(i.e. map the domain model to the view model)
            ivm.Email = user.Email;
            ivm.HasPassword = true;
            ivm.UserID = user.Id;
            ivm.UserName = user.UserName;

            //send data to the view
            return View(ivm);
        }



        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel cpvm)
        {
            if (ModelState.IsValid)
            {
                // Find the logged in user using the UserManager
                AppUser userLoggedIn = await _userManager.FindByNameAsync(User.Identity.Name);

                // If the user is a CSO and an email address was provided, find the user with the specified email
                if (User.IsInRole("CSO") && !string.IsNullOrEmpty(cpvm.Email))
                {
                    AppUser userToChange = await _userManager.FindByEmailAsync(cpvm.Email.ToUpper());
                    if (userToChange != null)
                    {
                        var result = await _userManager.ChangePasswordAsync(userToChange, cpvm.OldPassword, cpvm.NewPassword);

                        if (result.Succeeded)
                        {
                            await _signInManager.SignInAsync(userToChange, isPersistent: false);
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "No user was found with the specified email address.");
                    }
                }
                else
                {
                    // Attempt to change the password for the logged in user
                    var result = await _userManager.ChangePasswordAsync(userLoggedIn, cpvm.OldPassword, cpvm.NewPassword);

                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(userLoggedIn, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }

            return View(cpvm);
        }



        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogOff()
        {
            //sign the user out of the application
            _signInManager.SignOutAsync();

            //send the user back to the home page
            return RedirectToAction("Index", "Home");
        }

        private SelectList GetAllMajorsSelectList()
        {
            //Get the list of months from the database
            List<Major> majorList = _context.Majors.ToList();

            //add a dummy entry so the user can select all months
            Major SelectNone = new Major() { MajorID = 0, MajorName = "All Majors" };
            majorList.Add(SelectNone);

            //convert the list to a SelectList by calling SelectList constructor
            //MonthID and MonthName are the names of the properties on the Month class
            //MonthID is the primary key
            SelectList majorSelectList = new SelectList(majorList.OrderBy(m => m.MajorID), "MajorID", "MajorName");

            //return the electList
            return majorSelectList;
        }
    }
}