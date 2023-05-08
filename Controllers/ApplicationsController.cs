using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using sp23Team13FinalProject.DAL;
using sp23Team13FinalProject.Models;
using sp23Team13FinalProject.Models.ViewModels;
using static System.Reflection.Metadata.BlobBuilder;

namespace sp23Team13FinalProject.Controllers
{
    public class ApplicationsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;


        public ApplicationsController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager; //?? throw new ArgumentNullException(nameof(userManager));
        }


        public async Task<IActionResult> Index()
        {
            List<Application> SelectedApplications;

            if (User.IsInRole("Student"))
            {
                SelectedApplications = await _context.Applications
                                 .Include(p => p.Position)
                                 .ThenInclude(c => c.Companies)
                                 .Where(p => p.Student.UserName == User.Identity.Name)
                                 .ToListAsync();
                return View(SelectedApplications);
            }


            if (User.IsInRole("Recruiter")) //|| User.IsInRole("CSO"))
            {
                var username = User.Identity.Name;

                var companyName = _context.Users
                    .Where(r => r.UserName == username)
                    .Select(r => r.Company.CompanyName)
                    .FirstOrDefault();

                var applications = _context.Applications
                    .Include(a => a.Position)
                    .ThenInclude(c => c.Companies)
                    .Where(a => a.Position.Companies.CompanyName == companyName)
                    .ToList();

                return View(applications);
            }

            else
            {
                var applications = await _context.Applications
                                                  .Include(a => a.Position)
                                                  .ThenInclude(c => c.Companies)
                                                  .ToListAsync();
                return View(applications);
            }
        }

        [Authorize(Roles = "Recruiter,CSO")]
        public async Task<IActionResult> GetStudentsByPosition(int id)
        {
            var position = _context.Positions.Include(a => a.Applications).ThenInclude(s => s.Student).First(x => x.PositionID == id);

            if (position.Applications == null)
            {
                
                return NotFound();
               
            }

            if (position.Deadline > GlobalModel.GlobalDate)
            {
                // position deadline has not passed yet
                return View("Error", new string[] { "This page cannot be viewed until after the position deadline." });
            }

            List<AppUser> students = new List<AppUser>();
            foreach (Application application in position.Applications)
            {
                if (application.ApplicationStatus != ApplicationStatus.Withdrawn)
                {
                    students.Add(application.Student);
                }
            }
            ViewBag.PositionID = id;
            return View(students);
        }


        // GET: Applications/AddStudents
        [Authorize(Roles = "Recruiter,CSO")]
        public IActionResult AddStudents(int id)
        {
            // Retrieve all users from the database
            var position = _context.Positions.FirstOrDefault(p => p.PositionID == id);
            var users = _context.Users.Where(u => !u.Applications.Any(a => a.Position.PositionID == id) && u.GPA.HasValue && u.ActiveStatus).ToList();

            ViewBag.PositionID = id;
            // Pass the users to the view
            return View(users);
        }

        // POST: Applications/AddStudents
        [HttpPost]
        [Authorize(Roles = "Recruiter,CSO")]
        public IActionResult AddStudents(string[] selectedStudents, int positionID)
        {
            if (selectedStudents == null)
            {
                // No students were selected
                return View("Error", "No students selected");
            }

            // Retrieve the selected users from the database
            var selectedUsers = _context.Users.Where(u => selectedStudents.Contains(u.Id)).ToList();

            // Do something with the selected users
            foreach (var id in selectedStudents)
            {
                var application = new Application
                {
                    Position = _context.Positions.First(p => p.PositionID == positionID),
                    Student = _context.Users.First(x => x.Id == id),
                    ApplicationStatus = ApplicationStatus.Accepted,
                };
                _context.Applications.Add(application);
            }

            _context.SaveChangesAsync();


            // Redirect to the list of students by position
            return RedirectToAction("GetStudentsByPosition", new { id = positionID });
        }



        // GET: Applications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Applications == null)
            {
                return NotFound();
            }

            var application = await _context.Applications
                .Include(i => i.Student.Applicant) //student's list of interviews
                .Include(a => a.Position)
                .ThenInclude(a => a.Companies)
                .FirstOrDefaultAsync(a => a.ApplicationID == id);
            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        // GET: Applications/Create
        [Authorize(Roles = "Student,CSO")]
        public async Task<IActionResult> Apply(int id)
        {
            Debug.WriteLine("PositionId: " + id);
            //Debug.WriteLine("Position: " + position);
            //Debug.WriteLine("Application: " + application);


            if (_userManager == null)
            {
                throw new ArgumentNullException(nameof(_userManager), "User manager cannot be null.");
            }

            // Code that uses userManager

            if (User.IsInRole("Student"))
            {
                var user = await _userManager.GetUserAsync(User); // get user obj
                var currentUser = _context.Users.
                    Include(m => m.Major)
                    .First(u => u.Id == user.Id);
                // Check if user has already applied for the selected position
                var hasApplied = _context.Applications.Any(a => a.Student.Id == user.Id && a.Position.PositionID == id);

                if (hasApplied)
                {
                    return View("Error", new string[] { "You have already applied for this position" });
                }
                var positionChecker = _context.Positions.FirstOrDefault(p => p.PositionID == id &&
                                                                            p.PositionType == currentUser.PositionType &&
                                                                            p.Deadline > DateTime.Now &&
                                                                            p.ApplicableMajors.Any(m => m.MajorID == currentUser.Major.MajorID));
                if (positionChecker== null)
                {
                    return View("Error", new string[] { "You are not eligible to apply for this position" });
                }

                var applicationId = _context.Applications
                    .Include(a => a.Position)
                    .Where(p => p.Position.PositionID == id)
                    .Select(a => a.ApplicationID)
                    .FirstOrDefault();

                var position = _context.Positions.FirstOrDefault(p => p.PositionID == id);
                var positionTitle = _context.Positions
                    .Where(p => p.PositionID == id)
                    .Select(p => p.PositionTitle)
                    .FirstOrDefault();
                var companyName = _context.Positions
                    .Where(p => p.PositionID == id)
                    .Select(p => p.Companies.CompanyName)
                    .FirstOrDefault();

                var viewModel = new ApplicationViewModel
                {
                    SelectedPositionID = id,
                    PositionTitle = positionTitle,
                    CompanyName = companyName,
                    ApplicationStatus = ApplicationStatus.Pending,
                    SelectedApplicationID = applicationId
                };

                //var application = _context.Applications.FirstOrDefault(a => a.ApplicationID == applicationId);
                var newApplicaiotn = new Application
                {
                   
                    ApplicationStatus = ApplicationStatus.Pending,
                    Student = user,
                    Position = position
                };

                _context.Add(newApplicaiotn);
                await _context.SaveChangesAsync();

                return View(viewModel);
            }

            return View("Error", new string[] { "You are not eligible to apply for this position" });
        }

        [Authorize(Roles = "Student,CSO")]
        public async Task<IActionResult> Withdraw(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = _context.Applications
                .Include(a => a.Position)
                .Include(s => s.Student)
                .First(u => u.ApplicationID == id);



            if (application == null)
            {
                return NotFound();
            }

            // Check if deadline has passed
            if (application.Position.Deadline < GlobalModel.GlobalDate)
            {
                return View("The application deadline has passed.");
            }

            // Check if user is not authorized to withdraw application
            if (!User.IsInRole("Student") || application.Student.UserName != User.Identity.Name)
            {
                return Forbid();
            }

            // Update application status to Withdrawn
            application.ApplicationStatus = ApplicationStatus.Withdrawn;
            _context.Update(application);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Recruiter,CSO")]
        public async Task<IActionResult> SubmitAcceptedList(int positionID, string[] selectedStudents)
        {
            var position = await _context.Positions
                .Include(p => p.Applications)
                .FirstOrDefaultAsync(p => p.PositionID == positionID);

            if (position == null)
            {
                return NotFound();
            }

            var students = await _context.Applications
                .Include(a => a.Student)
                .Where(a => selectedStudents.Contains(a.Student.Id))
                .ToListAsync();

            foreach (var student in students)
            {
                student.ApplicationStatus = ApplicationStatus.Accepted;
                //await SendEmailToStudent(student.Student.Email, position.PositionTitle, position.Companies.CompanyName, "accepted");
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Recruiter,CSO")]
        public async Task<IActionResult> AcceptanceConfirmation(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = _context.Applications
                .Include(a => a.Position)
                .Include(s => s.Student)
                .First(u => u.ApplicationID == id);

            if (application == null)
            {
                return NotFound();
            }

            if (application.ApplicationStatus == ApplicationStatus.Rejected)
            {
                return View("Error", new string[] { "This application has already been rejected and cannot be accepted." });
            }

            // Update application status to Accepted
            application.ApplicationStatus = ApplicationStatus.Accepted;
            _context.Update(application);
            await _context.SaveChangesAsync();

            //send email
            var stuemail = _context.Applications
                .Include(a => a.Student)
                .Where(a => id == a.ApplicationID)
                .Select(a => a.Student.Email)
                .FirstOrDefault();

            Application dbApplication = _context.Applications.Find(id);
            try
            {
                String emailBody = "Congratulations!\n\nYou have been accepeted to interview!";
                Utilities.EmailMessaging.SendEmail("Bevo Recruiting - Application Status", emailBody, stuemail, stuemail);
            }
            catch (Exception ex)
            {
                return View("Error", new String[] { "There was a problem sending the email", ex.Message });
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Recruiter,CSO")]
        public async Task<IActionResult> RejectionConfirmation(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = _context.Applications
                .Include(a => a.Position)
                .Include(s => s.Student)
                .First(u => u.ApplicationID == id);



            if (application == null)
            {
                return NotFound();
            }

            if (application.ApplicationStatus == ApplicationStatus.Accepted)
            {
                return View("Error", new string [] {"This application has already been accepted and cannot be rejected."});
            }

            // Update application status to Rejected
            application.ApplicationStatus = ApplicationStatus.Rejected;
            _context.Update(application);
            await _context.SaveChangesAsync();

            //send email
            var stuemail = _context.Applications
                .Include(a => a.Student)
                .Where(a => id == a.ApplicationID)
                .Select(a => a.Student.Email)
                .FirstOrDefault();

            Application dbApplication = _context.Applications.Find(id);
            try
            {
                String emailBody = "Hello!\n\n We regret to inform you we have selected other candidates to interview. We wish you the best in your next career moo!";
                Utilities.EmailMessaging.SendEmail("Bevo Recruiting - Application Status", emailBody, stuemail, stuemail);
            }
            catch (Exception ex)
            {
                return View("Error", new String[] { "There was a problem sending the email", ex.Message });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
