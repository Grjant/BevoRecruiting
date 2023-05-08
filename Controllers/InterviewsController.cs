using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using sp23Team13FinalProject.DAL;
using sp23Team13FinalProject.Models;
using sp23Team13FinalProject.Models.ViewModels;
using sp23Team13FinalProject.Utilities;
using static System.Reflection.Metadata.BlobBuilder;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace sp23Team13FinalProject.Controllers
{
    public class InterviewsController : Controller
    {
        DateTime globalDate = GlobalModel.GlobalDate;
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public InterviewsController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "CSO")]
        public IActionResult Map(DateTime date)
        {
            if (date == DateTime.MinValue)
            {
                date = globalDate; // set default to current date
            }
            // Get the list of interviews that occur on the selected day
            var interviewsOnDate = _context.Interviews
                .Where(i => i.Date.Date == date.Date)
                .ToList();
            // Create a dictionary to track which rooms are booked, along with their start times
            var bookedRooms = new Dictionary<string, List<DateTime>>();
            // Initialize the dictionary with empty lists for each room
            foreach (var roomNumber in _context.Interviews.Select(i => i.RoomNumber).Distinct())
            {
                bookedRooms[roomNumber] = new List<DateTime>();
            }
            // Add start times to the list for each booked room in the dictionary
            foreach (var interview in interviewsOnDate)
            {
                if (!string.IsNullOrEmpty(interview.RoomNumber))
                {
                    if (bookedRooms.ContainsKey(interview.RoomNumber))
                    {
                        var bookedTimesForRoom = bookedRooms[interview.RoomNumber];
                        if (bookedTimesForRoom.Count == 0 || bookedTimesForRoom.Last() != interview.StartTime)
                        {
                            bookedTimesForRoom.Add(interview.StartTime);
                        }
                    }
                }
            }
            // Sort the start times for each room
            foreach (var room in bookedRooms)
            {
                room.Value.Sort();
            }
            // Pass the dictionary of booked rooms and their start times to the view
            ViewBag.BookedRooms = bookedRooms;
            ViewBag.Date = date;
            return View();
        }

        [Authorize(Roles = "CSO,Recruiter")]
        public IActionResult RecruiterSearchForm()
        {
            //get all the interviews
            var interviews = _context.Interviews
                .Include(i => i.Applicant)
                .Include(i => i.Recruiter)
                .Include(i => i.Position)
                .ThenInclude(a => a.Companies)
                .ToList();

            if (interviews.Count == 0)
            {
                return View("Error", new string[] { "No interviews are currently scheduled" });
            }

            // Create a new instance of the view model and populate it with data
            var viewModel = new Models.ViewModels.RecruiterSch
            {
            };
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "CSO,Recruiter")]
        public IActionResult RecruiterSearchForm(Models.ViewModels.RecruiterSch svm)
        {
            Debug.WriteLine("svm: " + svm);

            //Initial Query
            var query = from p in _context.Interviews
                        select p;

            //Start Range
            if (svm.SelectedStartDate != null)
            {
                var startDate = svm.SelectedStartDate.Value.Date;
                query = query.Where(p => p.Date >= startDate);
            }

            //End Range
            if (svm.SelectedEndDate != null)
            {
                var endDate = svm.SelectedEndDate.Value.Date;
                query = query.Where(p => p.Date.Date <= endDate);
            }

            var username = User.Identity.Name;

            List<Interview> selectedInterviews;
            if (User.IsInRole("CSO"))
            {
                 selectedInterviews = query
                    .Include(i => i.Applicant)
                    .Include(i => i.Recruiter)
                    .Include(i => i.Position)
                    .ThenInclude(a => a.Companies)
                    .ToList();
            }
            else
            {

                selectedInterviews = query
                    .Include(i => i.Applicant)
                    .Include(i => i.Recruiter)
                    .Include(i => i.Position)
                    .ThenInclude(a => a.Companies)
                    .Where(i => username == i.Recruiter.UserName)
                    .ToList();
            }
            ViewBag.SelectedRooms = selectedInterviews.ToList();
            return View("Index", selectedInterviews.OrderBy(p => p.RoomNumber));
        }



        [Authorize(Roles = "CSO")]
        public IActionResult InterviewSearchForm()
        {
            //get all the interviews
            var interviews = _context.Interviews
                .Include(i=> i.Applicant)
                .Include(i => i.Recruiter)
                .Include(i => i.Position)
                .ThenInclude(a => a.Companies)
                .ToList();

            if (interviews.Count == 0)
            {
                return View("Error", new string[] { "No interviews are currently scheduled" });
            }

            var companies = _context.Companies.ToList();

            // Create a new instance of the view model and populate it with data
            var viewModel = new Models.ViewModels.InterviewScheduleViewModel
            {
                Interviews = interviews,
                Companies = companies
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult InterviewSearchForm(Models.ViewModels.InterviewScheduleViewModel svm)
        {
            Debug.WriteLine("svm: " + svm);
            //get all the interviews
            var interviews = _context.Interviews
                .Include(i => i.Applicant)
                .Include(i => i.Recruiter)
                .Include(i => i.Position)
                .ThenInclude(a => a.Companies)
                .ToList();

            //Initial Query
            var query = from p in _context.Interviews
                        select p;

            //Search by Company
            if (svm.SelectedCompany != null)
            {
                query = query.Where(p => p.Position.Companies != null && p.Position.Companies.CompanyName.Contains(svm.SelectedCompany));
            }

            //Search by Date
            if (svm.SelectedDate != null)
            {
                query = query.Where(p => p.Date == svm.SelectedDate);
            }

            //Search by Room Number
            if (svm.SelectedRoomNumber != null)
            {
                query = query.Where(p => p.RoomNumber == svm.SelectedRoomNumber);
            }

            List<Interview> selectedInterviews = query
                .Include(i => i.Applicant)
                .Include(i => i.Recruiter)
                .Include(i => i.Position)
                .ThenInclude(a => a.Companies)
                .ToList();

            ViewBag.AllInterviews = _context.Interviews.ToList();
            ViewBag.SelectedInterviews = selectedInterviews.ToList();
            return View("Index", selectedInterviews.OrderBy(p => p.InterviewerName));
        }


        // GET: Interviews
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Student"))
            {
                // Display all interviews for a student
                // Get the current user's username
                var username = User.Identity.Name;

                // Retrieve all interviews for the current student
                var myInterviews = await _context.Interviews
                    .Include(i=>i.Applicant)
                    .Include(i => i.Position)
                    .ThenInclude(p => p.Companies)
                    .Where(i => i.Applicant.UserName == username)
                    .ToListAsync();
                return View(myInterviews);
            }
            else if (User.IsInRole("Recruiter"))
            {
                // Display all interviews for the recruiter's company
                // Get the current user's username
                var username = User.Identity.Name;

                // Get the recruiter's company name from the database
                var companyName = _context.Users
                    .Where(r => r.UserName == username)
                    .Select(r => r.Company)
                    .FirstOrDefault();

                // Get the list of interviews for the current user's company
                var interviews = _context.Interviews
                    .Include(a=>a.Applicant)
                    .Include(p=>p.Position)
                    .ThenInclude(p => p.Companies)
                    .Join(_context.Users,
                        i => i.Recruiter.Id,
                        u => u.Id,
                        (i, u) => new { Interview = i, Interviewer = u })
                    .Where(r => r.Interviewer.Company == companyName && r.Interview.Recruiter.UserName == username)
                    .Select(r => r.Interview)
                    .ToList();

                // Get the list of users in the Recruiter role for the current user's company
                var recruiters = _userManager.GetUsersInRoleAsync("Recruiter").Result
                    .Where(u => u.Company == companyName)
                    .Select(u => new { Value = u.Id, Text = u.FirstName + " " + u.LastName })
                    .ToList();

                return View(interviews);
            }
            else if (User.IsInRole("CSO"))
            {
                // Display all interviews
                var interviews = await _context.Interviews
                    .Include(i => i.Applicant)
                    .Include(r => r.Recruiter.Company)
                    .Include(i => i.Position)
                    .ThenInclude(p => p.Companies)
                    .ToListAsync();

                return View(interviews);
            }
            else
            {
                var interviews = await _context.Interviews
                    .Include(r => r.Recruiter.Company)
                    .Include(i => i.Position)
                    .ThenInclude(p => p.Companies)
                    .ToListAsync();

                return View(interviews);
            }
        }


        // GET: Interviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Interviews == null)
            {
                return NotFound();
            }

            var interview = await _context.Interviews
                .Include(p => p.Recruiter)
                .Include(p => p.Applicant)
                .Include(p => p.Position)
                .ThenInclude(c => c.Companies)
                .FirstOrDefaultAsync(m => m.InterviewID == id);


            if (interview == null)
            {
                return NotFound();
            }

            return View(interview);
        }

        [Authorize(Roles = "Student")]
        public IActionResult UpdatePosition()
        {
            // Get the current user's username
            var username = User.Identity.Name;

            var app = _context.Applications // application status
                //.Where(a => a.Student.UserName == username)
                .Select(a => a.ApplicationStatus)
                .FirstOrDefault();

            // Get the positions that the user has applied to
            var positions = _context.Applications
                .Include(i => i.Interview)
                .Include(a => a.Position)
                .Where(a => a.Student.UserName == username && a.ApplicationStatus == ApplicationStatus.Accepted && a.InterviewScheduled == false)
                //.Where(a => a.Student.UserName == username && ApplicationStatus.Accepted ==  app && a.InterviewScheduled == false )
                .Select(a => new SelectListItem { Value = a.Position.PositionID.ToString(), Text = a.Position.PositionTitle })
                .ToList();

            if (positions.Count == 0)
            {
                return View("Error", new string[] { "All interviews Scheduled or No Applications elgible for an Interview!" });
            }

            // Create a new instance of the view model and populate it with data
            var viewModel = new Models.ViewModels.InterviewViewModel
            {
                Positions = positions,
                SelectedPositionId = positions.FirstOrDefault()?.Value
            };
            return View(viewModel);
        }

        // GET: Interviews/SelectTimeSlot
        // Select Time Slot
        public IActionResult SelectTimeSlot(int positionId)
        {
            Debug.WriteLine("PositionId: " + positionId);

            var companyId = _context.Positions
                .Where(p => p.PositionID == positionId)
                .Select(p => p.Companies.CompanyID)
                .FirstOrDefault();

            var slots = _context.Interviews
                .Include(s => s.Position)
                .ThenInclude(p => p.Companies)
                .Where(i => companyId == i.Position.Companies.CompanyID
                && i.InterviewAvailable == true
                && i.Date >= i.Position.Deadline.AddHours(48))
                .Select(s => new SelectListItem
                {
                    Value = s.InterviewID.ToString(),
                    Text = s.Date.ToString("MM/dd/yyyy") + " " + s.StartTime.ToString("hh:mm tt")
                })
                .ToList();

            if (slots.Count == 0)
            {
                return View("Error", new string[] { "No Time Slots Available" });
            }

            var viewModel = new TimeSlotViewModel
            {
                SelectedPositionId = positionId,
                TimeSlots = slots,
                SelectedInterviewId = slots.FirstOrDefault()?.Value
            };
            return View(viewModel);
        }

        public IActionResult ScheduleTimeSlot(int positionId, int selectedInterviewId)
        {
            Debug.WriteLine("SelectedInterviewId: " + selectedInterviewId);
            Debug.WriteLine("PositionId: " + positionId);

            var interview = _context.Interviews.FirstOrDefault(i => i.InterviewID == selectedInterviewId);
            var position = _context.Positions.FirstOrDefault(p => p.PositionID == positionId);
            var userId = _userManager.GetUserId(User);
            AppUser applicant = _userManager.FindByIdAsync(userId).Result;

            interview.Position = position;
            interview.Applicant = applicant;
            interview.InterviewAvailable = false;

            // Set InterviewScheduled to true for the associated application
            var application = _context.Applications
                .FirstOrDefault(a => a.Student.Id == applicant.Id && a.Position.PositionID == positionId);
            application.InterviewScheduled = true;

            //TODO need to check if this works
            if (position.Deadline <= globalDate.AddDays(-2))
            {
                return View("Error", new String[] { "Interviews may not take place until 48 hours after the application deadline!" });
            }

            _context.SaveChanges();

            var stuemail = applicant.Email;

            var recemail = _context.Interviews
                .Where(p => p.InterviewerName == interview.InterviewerName)
                .Select(p =>p.Recruiter.Email)
                .FirstOrDefault();

            // Send the email to confirm order details have been added
            // Once a student has selected an interview slot,he/ she should be sent a confirmation email
            // with the date, time, room number, position and interviewer’s name.
            Interview dbInterview = _context.Interviews.Find(selectedInterviewId);
            try
            {
                String emailBody = "Hello!\n\nThank you for scheduling an Interview\n\n " +
                    "Date: " + dbInterview.Date.ToString("MM/dd/yyyy") + "\n\n" +
                    "Start Time: " + dbInterview.StartTime.ToString("hh:mm tt") + "\n\n" +
                    "End Time: " + dbInterview.EndTime.ToString("hh:mm tt") + "\n\n" +
                    "Position: " + dbInterview.Position.PositionTitle + "\n\n" +
                    "Interviewers Name: " + dbInterview.InterviewerName;
                Utilities.EmailMessaging.SendEmail("Bevo Recruiting - Interview Schedule", emailBody, stuemail, recemail);
            }
            catch (Exception ex)
            {
                return View("Error", new String[] { "There was a problem sending the email", ex.Message });
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Student, CSO")]
        public IActionResult ReScheduleTimeSlot(int positionId, int selectedInterviewId)
        {
            Debug.WriteLine("SelectedInterviewId: " + selectedInterviewId);
            Debug.WriteLine("PositionId: " + positionId);

            // this is the new interview object, it should have the default applicant ID
            var interview = _context.Interviews
                .Include(u => u.Applicant)
                .FirstOrDefault(i => i.InterviewID == selectedInterviewId);
           
            var position = _context.Positions.FirstOrDefault(p => p.PositionID == positionId);
            var userId = _userManager.GetUserId(User);

            //TODO need to check if this works
            if (position.Deadline <= globalDate.AddDays(-2))
            {
                return View("Error", new String[] { "Interviews may not take place until 48 hours after the application deadline!" });
            }

            //lets try this

            //AppUser applicant = _userManager.FindByIdAsync(userId).Result;
            var username = User.Identity.Name;

            //find the ID of the student associated with the old interview -- this is returning no applicant which is wrong
            var in_cso = _context.Interviews
                    .Include(i => i.Applicant)
                    .Where(i=> i.InterviewAvailable == false && i.Position.PositionID == positionId)
                    //.Where(i => i.InterviewID == interview.InterviewID) //selects applicant of new interview
                    .Select(i => i.Applicant)
                    .FirstOrDefault();
            Debug.WriteLine("in cso: " + in_cso);

            AppUser applicant;
            if (User.IsInRole("CSO"))
            {
                
                applicant = in_cso;
            }
            else 
            {
                applicant = _userManager.FindByIdAsync(userId).Result;
                //in_student;
            }

                //if (interview.Applicant != applicant)
            Debug.WriteLine("UserApplicantId: " + applicant); // users email -- old interview
            Debug.WriteLine("InterviewApplicantID: " + interview.Applicant); // default value (no applicant) -- new interview

            //get the id passed through the method (slot choosen for reschedule)
            var new_interview = _context.Interviews
               .Where(u => u.InterviewAvailable == true )
               .FirstOrDefault(i => i.InterviewID == selectedInterviewId);

            //get the id of the old interview
            var old_interview = _context.Interviews
                .Include(u => u.Applicant)
                .Where(u => u.InterviewAvailable == false && applicant == u.Applicant) //do not change this line
                .FirstOrDefault();

            Debug.WriteLine("old_interview: " + old_interview);

            new_interview.Applicant = applicant;
            new_interview.InterviewAvailable = false;
            new_interview.Position = position;
            
            AppUser user = new AppUser();
            user.UserName = "No Applicant";
            user.FirstName = "";
            user.LastName = "";

            old_interview.Applicant = user; //old_interview.applicant is null as a CSO 
            old_interview.InterviewAvailable = true;
            //do not need to update position id

            _context.SaveChanges();

            var email = applicant.Email;
            
            var recemail = _context.Interviews
                .Where(p => p.InterviewerName == new_interview.InterviewerName)
                .Select(p =>p.Recruiter.Email)
                .FirstOrDefault();
            // Send the email to confirm order details have been added
            // Once a student has selected an interview slot,he/ she should be sent a confirmation email
            // with the date, time, room number, position and interviewer’s name.
            Interview dbInterview = _context.Interviews.Find(selectedInterviewId);
            try
            {
                String emailBody = "Hello!\n\nThank you for rescheduling an Interview!\n\n " +
                    "Date: " + dbInterview.Date.ToString("MM/dd/yyyy") + "\n\n" +
                    "Start Time: " + dbInterview.StartTime.ToString("hh:mm tt") + "\n\n" +
                    "End Time: " + dbInterview.EndTime.ToString("hh:mm tt") + "\n\n" +
                    "Position: " + dbInterview.Position.PositionTitle + "\n\n" +
                    "Interviewers Name: " + dbInterview.InterviewerName;
                Utilities.EmailMessaging.SendEmail("Bevo Recruiting - Interview Schedule", emailBody, email, recemail);
            }
            catch (Exception ex)
            {
                return View("Error", new String[] { "There was a problem sending the email", ex.Message });
            }

            return RedirectToAction("Index");
        }


        //GET: Interviews/CreateSlot
        [Authorize(Roles = "Recruiter, CSO")]
        public IActionResult CreateSlot()
        {
            Debug.WriteLine("global date: " + GlobalModel.GlobalDate);

            // Get the current user's username
            var username = User.Identity.Name;

            // Get the positions for the current user's company
            if (User.IsInRole("Recruiter"))
            {

                // Get the recruiter's company name from the database
                var companyName = _context.Users
                    .Where(r => r.UserName == username)
                    .Select(r => r.Company)
                    .FirstOrDefault();
                ViewBag.CompanyName = new SelectList(_context.Positions
                    .Where(p => p.Companies == companyName)
                    .Select(p => companyName.CompanyName)
                    .Distinct()
                    .ToList());

                // Get the list of users in the Recruiter role for the current user's company
                var recruiters = _userManager.GetUsersInRoleAsync("Recruiter").Result
                    .Where(u => u.Company == companyName)
                    .Select(u => new { Value = u.Id, Text = u.FirstName + " " + u.LastName })
                    .ToList();
                ViewBag.Recruiters = new SelectList(recruiters, "Value", "Text");
            }
            else
            {
                ViewBag.CompanyName = new SelectList(_context.Positions
                    .Select(p => p.Companies.CompanyName)
                    .Distinct()
                    .ToList());
                var recruiters = _userManager.GetUsersInRoleAsync("Recruiter").Result
                                .Concat(_userManager.GetUsersInRoleAsync("CSO").Result) // add CSOs to the list
                                .Select(u => new { Value = u.Id, Text = u.FirstName + " " + u.LastName })
                                .ToList();
                ViewBag.Recruiters = new SelectList(recruiters, "Value", "Text");
            }

        // Create the list of valid interview start times
        var selectListItems = new List<SelectListItem>
        {
            new SelectListItem { Value = "08:00:00", Text = "8:00 AM" },
            new SelectListItem { Value = "09:00:00", Text = "9:00 AM" },
            new SelectListItem { Value = "10:00:00", Text = "10:00 AM" },
            new SelectListItem { Value = "11:00:00", Text = "11:00 AM" },
            new SelectListItem { Value = "13:00:00", Text = "1:00 PM" },
            new SelectListItem { Value = "14:00:00", Text = "2:00 PM" },
            new SelectListItem { Value = "15:00:00", Text = "3:00 PM" },
            new SelectListItem { Value = "16:00:00", Text = "4:00 PM" },
        };
                var selectListItemsEnd = new List<SelectListItem>
        {
            new SelectListItem { Value = "09:00:00", Text = "9:00 AM" },
            new SelectListItem { Value = "10:00:00", Text = "10:00 AM" },
            new SelectListItem { Value = "11:00:00", Text = "11:00 AM" },
            new SelectListItem { Value = "13:00:00", Text = "1:00 PM" },
            new SelectListItem { Value = "14:00:00", Text = "2:00 PM" },
            new SelectListItem { Value = "15:00:00", Text = "3:00 PM" },
            new SelectListItem { Value = "16:00:00", Text = "4:00 PM" },
             new SelectListItem { Value = "17:00:00", Text = "5:00 PM" },
        };
            ViewBag.InterviewStartTimes = selectListItems;
            ViewBag.InterviewCompanyEndTimes = selectListItemsEnd;
            return View();
 }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Recruiter, CSO")]
        public async Task<IActionResult> CreateSlot([Bind("InterviewerName, Date,StartTime,CompanyEndTime, RoomNumber")] Interview interview)
        {

            // Get the selected recruiter's ID
            var recruiterId = interview.InterviewerName;

            // Get the recruiter's email from the database
            var recruiter = await _userManager.FindByIdAsync(recruiterId);
            var recruiterEmail = recruiter.Email;
            var interviewerName = recruiter.FirstName + " " + recruiter.LastName;
            var username = User.Identity.Name;

            Company companyName;
            if (User.IsInRole("CSO"))
            {
                companyName = _context.Users
                    .Where(r => r.UserName == recruiterEmail)
                    .Select(r => r.Company)
                    .FirstOrDefault();
            }
            else
            {
                companyName = _context.Users
                .Where(r => r.UserName == username)
                .Select(r => r.Company)
                .FirstOrDefault();
            }

            var position = _context.Positions
                .Where(p => p.Companies == companyName)
                .FirstOrDefault();
                //.ToList();
            //works
            if ((interview.DayofTheWeek == null && (interview.Date.DayOfWeek == DayOfWeek.Saturday || interview.Date.DayOfWeek == DayOfWeek.Sunday)))
            {
                return View("Error", new String[] { "Interviews can only be scheduled on weekdays." });
            }

            //if (_context.Interviews.Any(i => i.Date == interview.Date && i.StartTime == interview.StartTime))
            //{
            //    return View("Error", new String[] { "Interview time slot is already taken." });
            //}

            Debug.WriteLine("username: " + username); 
            Debug.WriteLine("recruiterEmail: " + recruiterEmail);


            AppUser user = new AppUser();
            if (User.IsInRole("CSO"))
            {
                // Get the currently logged in user's details
                var user1 = await _userManager.GetUserAsync(User);

                if (username == recruiterEmail) // if recruiter name
                {
                    user.UserName = "Being Used by a CSO";
                    user.FirstName = user1.FirstName;
                    user.LastName = user1.LastName;
                }
                else
                {
                    user.UserName = "No Applicant";
                    user.FirstName = "";
                    user.LastName = "";
                }
            }
            else
            {
                user.UserName = "No Applicant";
                user.FirstName = "";
                user.LastName = "";
            }
            var Default = user;


            // create slots
            // Calculate the end time of the lunch break
            var lunchBreak = interview.Date.Date.AddHours(12);
            // Loop through the hours between the start and end times
            var startTime = interview.StartTime;
            var endTime = interview.CompanyEndTime;
            var currentDate = interview.Date.Date;
            if (globalDate >= interview.Date)//.Add(interview.StartTime))
            {
                if (globalDate >= interview.StartTime)
                {
                    return View("Error", new String[] { "Time Slot has already passed!" });
                }
            }

            if (globalDate > currentDate)
            {
                return View("Error", new String[] { "This date has already passed!" });
            }

            if (endTime <= startTime)
            {
                return View("Error", new String[] { "End Time cannot be eariler than Start Time!" });
            }

            while (currentDate.AddHours(startTime.Hour) < currentDate.AddHours(endTime.Hour))
            {
                if (currentDate.AddHours(startTime.Hour) == lunchBreak)
                {
                    startTime = startTime.AddHours(1); // Skip the hour between 12pm-1pm
                    continue;
                }
                // Check if there is an available room for the interview
                var availableRooms = new List<string>() { "1", "2", "3", "4" };
                var takenRooms = _context.Interviews
                    .Where(i => i.Date == interview.Date && i.StartTime == interview.StartTime)
                    .Select(i => i.RoomNumber)
                    .ToList();
                int sumOfRooms = takenRooms.Select(int.Parse).Sum();
                if (sumOfRooms >= 9)
                {
                    return View("Error", new String[] { $"No rooms available for at {interview.StartTime}." });
                }
                var availableRoom = availableRooms.Except(takenRooms).FirstOrDefault();
                if (availableRoom == null)
                {
                    return View("Error", new String[] { "No rooms available for this time slot." });
                }

                // Assign the available room to the interview
                interview.RoomNumber = availableRoom;


                // Create a new interview slot
                var newInterview = new Interview
                {
                    InterviewerName = interviewerName,
                    Date = currentDate,
                    Recruiter = recruiter,
                    Position = position,
                    StartTime = startTime,
                    CompanyEndTime = startTime.AddHours(1),
                    RoomNumber = interview.RoomNumber,
                    Applicant = Default,
                    InterviewAvailable = true
                };

                // Save the new interview slot to the database
                _context.Add(newInterview);
                await _context.SaveChangesAsync();
                startTime = startTime.AddHours(1); // Move to the next hour
            }

            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Student, CSO")]
        public async Task<IActionResult> Edit(int? id)
        {
            var interview = await _context.Interviews
                .Include(i => i.Applicant)
                .Include(i => i.Position)
                .ThenInclude(i => i.Applications)
                .FirstOrDefaultAsync(i => i.InterviewID == id);

            //TODO you can only modify an interview if a day before (not 24 hours. midnight day before)
            // Check if interview is at least a day before the interview date

            var timeUntilInterview = interview.Date - globalDate.Date;
            if (timeUntilInterview <= TimeSpan.Zero) //if the time until interview is less than 0
            {
                // If the interview is less than a day away, it cannot be modified
                return View("Error", new String[] { "Forbidden: You cannot reschedule the day of!" });
            }

            //if (User.IsInRole("CSO, Student"))
            //{
                if (interview.Applicant.UserName == "No Applicant") // LOL this is a funny implementation
                {
                    return View("Error", new String[] { "Forbidden: You cannot reschedule a time slot with no applicant!" });
                }
           // }
            var positionId = interview.Position.PositionID;
            var companyId = _context.Positions
                .Where(p => p.PositionID == positionId)
                .Select(p => p.Companies.CompanyID)
                .FirstOrDefault();
            //var position = _context.Positions.FirstOrDefault(p => p.PositionID == positionId);

            //if (position.Deadline <= globalDate.AddDays(-2))
            //{
            //    return View("Error", new String[] { "Interviews may not take place until 48 hours after the application deadline!" });
            //}

            var slots = _context.Interviews
                .Include(s => s.Position)
                .ThenInclude(p => p.Companies)
                .Where(i => companyId == i.Position.Companies.CompanyID
                && i.InterviewAvailable == true
                && i.Date >= i.Position.Deadline.AddHours(48))
                .Select(s => new SelectListItem
                {
                    Value = s.InterviewID.ToString(),
                    Text = s.Date.ToString("MM/dd/yyyy") + " " + s.StartTime.ToString("hh:mm tt")
                })
                .ToList();

            if (slots.Count == 0)
            {
                return View("Error", new string[] { "No Time Slots Available" });
            }

            var viewModel = new TimeSlotViewModel
            {
                SelectedPositionId = positionId,
                TimeSlots = slots,
                SelectedInterviewId = slots.FirstOrDefault()?.Value
            };
            return View("Edit",viewModel); //re --- schedule time slot?
        }

        // POST: Interviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InterviewID,InterviewerName,RoomNumber,StartTime")] Interview interview)
        {
            if (id != interview.InterviewID)
            {
                return NotFound();
            }
            
            var positionId = _context.Positions
                .Where(p => p.PositionID == interview.Position.PositionID)
                .Select(p => p)
                .FirstOrDefault();

            //var position = _context.Positions.FirstOrDefault(p => p.PositionID == positionId);

            if (positionId.Deadline <= globalDate.AddDays(-2))
            {
                return View("Error", new String[] { "Interviews may not take place until 48 hours after the application deadline!" });
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(interview);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InterviewExists(interview.InterviewID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(interview);
        }

        // GET: Interviews/Delete/5
        [Authorize(Roles = "Recruiter,CSO")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Interviews == null)
            {
                return NotFound();
            }

            var interview = await _context.Interviews
                .FirstOrDefaultAsync(m => m.InterviewID == id);
            if (interview == null)
            {
                return NotFound();
            }

            return View(interview);
        }

        // POST: Interviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Recruiter,CSO")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Interviews == null)
            {
                return Problem("Entity set 'AppDbContext.Interviews'  is null.");
            }
            //var interview = await _context.Interviews.FindAsync(id);
            var interview = await _context.Interviews
                .Include(i => i.Applicant)
                .Include(i => i.Position)
                .ThenInclude(i=> i.Applications)
    .           FirstOrDefaultAsync(i => i.InterviewID == id);
            var applicant = interview.Applicant;
            var positionId = interview.Position.PositionID;
            // Set InterviewScheduled to true for the associated application
            var application = _context.Applications
                .FirstOrDefault(a => a.Student == applicant && a.Position.PositionID == positionId);
            if (application != null)
            {
                application.InterviewScheduled = false;
            }
             
            if (interview != null)
            {
                _context.Interviews.Remove(interview);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InterviewExists(int id)
        {
            return (_context.Interviews?.Any(e => e.InterviewID == id)).GetValueOrDefault();
        }
    }
}