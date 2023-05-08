using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sp23Team13FinalProject.DAL;
using sp23Team13FinalProject.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore.SqlServer;
using System.IO;
using System.Drawing;
using sp23Team13FinalProject.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

// Tags added so that the student role can only see the details of the position
// A recruiter can edit, delete, and add positions

namespace sp23Team13FinalProject.Controllers
{
    public class PositionsController : Controller
    {
        private readonly AppDbContext _context;
        DateTime globalDate = GlobalModel.GlobalDate;
        private readonly UserManager<AppUser> _userManager;

        public PositionsController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: Positions
        [AllowAnonymous]
        public async Task<IActionResult> Index(string search)
        {
            // Retrieve positions with related company data
            var positions = _context.Positions
                .Include(p => p.ApplicableMajors)
                .Include(p => p.Companies); // Include the Companies navigation property


            if (string.IsNullOrEmpty(search) && User.IsInRole("Student") || User.IsInRole("Recruiter"))
            {
                ViewBag.AllPositions = _context.Positions.Count(p => p.Deadline >= globalDate);
                ViewBag.SelectedPositions = _context.Positions.Count(p => p.Deadline >= globalDate);

                return View(_context.Positions.OrderBy(p => p.PositionTitle)
                    .Include(p => p.ApplicableMajors)
                    .Include(p => p.Companies)
                    .ToList());
            }

            if (User.IsInRole("Student") || User.IsInRole("Recruiter"))
            {
                var query = from p in _context.Positions
                            select p;

                query = query.Where(b => b.PositionTitle.ToLower().Contains(search.ToLower()) ||
                             b.Description.ToLower().Contains(search.ToLower()));

                List<Position> selectedPositions = query.Include(p => p.ApplicableMajors)
                     .Include(p => p.Companies)
                     .ToList();

                //Populate the ViewBag with a count of all positions
                ViewBag.AllPositions = _context.Positions.Count(p => p.Deadline >= globalDate);

                //Populate the ViewBag with a count of selected positions
                ViewBag.SelectedPositions = selectedPositions.Count(p => p.Deadline >= globalDate);

                //Return selected positions to the view, ordered by PositionTitle
                return View(selectedPositions.OrderBy(p => p.PositionTitle));
            }


            if (string.IsNullOrEmpty(search) && User.IsInRole("CSO"))
            {
                ViewBag.AllPositions = _context.Positions.Count();
                ViewBag.SelectedPositions = _context.Positions.Count();

                return View(_context.Positions.OrderBy(p => p.PositionTitle)
                    .Include(p => p.ApplicableMajors)
                    .Include(p => p.Companies)
                    .ToList());
            }

            if (User.IsInRole("Student") || User.IsInRole("Recruiter"))
            {
                var query = from p in _context.Positions
                            select p;

                query = query.Where(b => b.PositionTitle.ToLower().Contains(search.ToLower()) ||
                             b.Description.ToLower().Contains(search.ToLower()));

                List<Position> selectedPositions = query.Include(p => p.ApplicableMajors)
                     .Include(p => p.Companies)
                     .ToList();

                //Populate the ViewBag with a count of all positions
                ViewBag.AllPositions = _context.Positions.Count();

                //Populate the ViewBag with a count of selected positions
                ViewBag.SelectedPositions = selectedPositions.Count();

                //Return selected positions to the view, ordered by PositionTitle
                return View(selectedPositions.OrderBy(p => p.PositionTitle));
            }

            else
            {
                return View();
            }
        }



        // Filter positions based on search criteria
        //if (!string.IsNullOrEmpty(search))
        //{
        //    positions = positions.Where(p =>
        //        p.PositionTitle.Contains(search, StringComparison.OrdinalIgnoreCase) ||
        //        p.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
        //        p.PositionType.ToString().Contains(search, StringComparison.OrdinalIgnoreCase) ||
        //        p.Companies.CompanyName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
        //        p.Companies.Industry.Contains(search, StringComparison.OrdinalIgnoreCase) ||
        //        p.Location.Contains(search, StringComparison.OrdinalIgnoreCase) ||
        //        p.ApplicableMajors.Any(m => m.MajorName.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
        //        p.Deadline.ToString().Contains(search, StringComparison.OrdinalIgnoreCase)
        //    ).ToList();
        //}

        public ActionResult DetailedSearch()
        {
            ViewBag.AllMajors = GetMajors();
            return View();
        }

        public IActionResult DisplaySearchResults(SearchViewModel svm)
        {
            //Initial Query
            var query = from p in _context.Positions
                        select p;

            //Search by Deadline
            if (svm.DateSearch != null)
            {
                query = query.Where(p => p.Deadline <= svm.DateSearch);
            }

            //Search by Company
            if (!String.IsNullOrEmpty(svm.CompanySearch))
            {
                query = query.Where(p => p.Companies != null && p.Companies.CompanyName.Contains(svm.CompanySearch));
            }

            //Search by Type
            if (svm.PositionTypeSearch != null)
            {
                query = query.Where(p => p.PositionType == svm.PositionTypeSearch);
            }

            //Search by Major

            if (svm.MajorSearch != 0)
            {
                query = query.Where(p => p.ApplicableMajors.Any(m => m.MajorID == svm.MajorSearch));
            }

            //Search by Location

            if (!String.IsNullOrEmpty(svm.LocationSearch))
            {
                query = query.Where(p => p.Location != null &&
                    (p.Location.Contains(svm.LocationSearch) ||
                     p.Location.StartsWith(svm.LocationSearch) ||
                     p.Location.EndsWith(svm.LocationSearch) ||
                     p.Location.Contains(", " + svm.LocationSearch)));
            }

            //Search by Industry
            if (!String.IsNullOrEmpty(svm.IndustrySearch))
            {
                query = query.Where(p => p.Companies != null && (p.Companies.Industry1 + " " + p.Companies.Industry2 + " " + p.Companies.Industry3).Contains(svm.IndustrySearch));
            }
            //Search by Description
            if (String.IsNullOrEmpty(svm.DescriptionSearch) == false)
            {
                query = query.Where(p => p.Description.Contains(svm.DescriptionSearch));
            }

            //Search by Position
            if (String.IsNullOrEmpty(svm.PositionSearch) == false)
            {
                query = query.Where(p => p.PositionTitle.Contains(svm.PositionSearch));

            }


            List<Position> selectedPositions = query.Include(p => p.ApplicableMajors)
                  .Include(p => p.Companies)
                  .ToList();

            ViewBag.AllPositions = _context.Positions.Count(p => p.Deadline > globalDate);
            ViewBag.SelectedPositions = selectedPositions.Count(p => p.Deadline > globalDate);


            return View("Index", selectedPositions.OrderBy(p => p.PositionTitle));

        }

        // GET: Positions/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Positions == null)
            {
                return NotFound();
            }

            var position = await _context.Positions
                .FirstOrDefaultAsync(m => m.PositionID == id);
            if (position == null)
            {
                return NotFound();
            }

            var positions = _context.Positions
           .Include(p => p.ApplicableMajors)
           .Include(p => p.Companies) // Include the Companies navigation property
           .ToList();

            // need all the interviews that have already been scheduled (like the student scheduled) for this position
            var positionid = id;
            ViewBag.InterviewsForPosition = _context.Interviews
                .Include(p => p.Position)
                .Where(i => i.Position.PositionID == positionid && i.InterviewAvailable == false)
                .Select(i => new { i.Date, i.StartTime })
                .ToList();


            return View(position);
        }

        //TODO: should CSO also be allowed this??
        // GET: Positions/Create
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> CreateAsync()
        {

            ViewBag.AllMajors = GetAllMajorsMultiSelectList();
            ViewBag.AllCompanies = GetAllCompaniesSelectList();

            if (User.IsInRole("Recruiter"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var loggedUser = _context.Users.Include(c => c.Company).First(m => m.Id == currentUser.Id);
                ViewBag.CurrentCompany = loggedUser.Company.CompanyID;
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PositionViewModel positionViewModel)
        {

            if (ModelState.IsValid)
            {
                // convert ViewModel to model
                var position = new Position
                {
                    PositionTitle = positionViewModel.PositionTitle,
                    PositionType = positionViewModel.PositionType,
                    Description = positionViewModel.Description,
                    Companies = _context.Companies.First(x => x.CompanyID == positionViewModel.SelectedCompanyID),
                    Location = positionViewModel.Location,
                    Deadline = positionViewModel.Deadline
                };

                // Add the selected majors to the position
                if (positionViewModel.SelectedMajorID != null)
                {
                    position.ApplicableMajors = new List<Major>();

                    foreach (var majorID in positionViewModel.SelectedMajorID)
                    {
                        var major = _context.Majors.FirstOrDefault(m => m.MajorID == majorID);

                        if (major != null)
                        {
                            position.ApplicableMajors.Add(major);
                        }
                    }
                }

                // Add the position to the database
                _context.Positions.Add(position);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            // If ModelState is not valid, return to the view with the ViewModel
            ViewBag.AllMajors = new MultiSelectList(_context.Majors, "MajorID", "MajorName");
            return View(positionViewModel);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var position = _context.Positions.Include(p => p.ApplicableMajors).Include(c => c.Companies)
                                              .FirstOrDefault(p => p.PositionID == id);

            if (position == null)
            {
                return NotFound();
            }

            // Populate the ViewBag with the list of companies and majors
            ViewBag.AllMajors = GetAllMajorsMultiSelectList();
            ViewBag.AllCompanies = GetAllCompaniesSelectList();
            if (User.IsInRole("Recruiter"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var loggedUser = _context.Users.Include(c => c.Company).First(m => m.Id == currentUser.Id);
                ViewBag.CurrentCompany = loggedUser.Company.CompanyID;
            }

            var positionViewModel = new PositionViewModel
            {
                PositionID = position.PositionID,
                PositionTitle = position.PositionTitle,
                PositionType = position.PositionType,
                Description = position.Description,
                Location = position.Location,
                Deadline = position.Deadline,
                SelectedCompanyID = position.Companies.CompanyID, // preselect the company
                SelectedMajorID = position.ApplicableMajors.Select(pm => pm.MajorID).ToArray()
            };

            return View(positionViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PositionViewModel positionViewModel)
        {
            if (id != positionViewModel.PositionID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var position = await _context.Positions
                    .Include(p => p.ApplicableMajors)
                    .Include(c => c.Companies)
                    .FirstOrDefaultAsync(p => p.PositionID == id);

                if (position == null)
                {
                    return NotFound();
                }

                // Update the position properties
                position.PositionTitle = positionViewModel.PositionTitle;
                position.PositionType = positionViewModel.PositionType;
                position.Description = positionViewModel.Description;
                position.Location = positionViewModel.Location;
                position.Deadline = positionViewModel.Deadline;

                // Update the applicable majors
                position.ApplicableMajors.Clear();
                if (positionViewModel.SelectedMajorID != null)
                {
                    foreach (var majorID in positionViewModel.SelectedMajorID)
                    {
                        var major = await _context.Majors.FindAsync(majorID);

                        if (major != null)
                        {
                            position.ApplicableMajors.Add(major);
                        }
                    }
                }

                // Update the company
                if (positionViewModel.SelectedCompanyID != null)
                {
                    var company = await _context.Companies.FindAsync(positionViewModel.SelectedCompanyID);
                    if (company != null)
                    {
                        position.Companies = company;
                    }
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PositionExists(positionViewModel.PositionID))
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

            return View(positionViewModel);
        }





        // GET: Positions/Delete/5
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Positions == null)
            {
                return NotFound();
            }

            var position = await _context.Positions
                .FirstOrDefaultAsync(m => m.PositionID == id);
            if (position == null)
            {
                return NotFound();
            }

            return View(position);
        }

        // POST: Positions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Positions == null)
            {
                return Problem("Entity set 'AppDbContext.Positions'  is null.");
            }
            var position = await _context.Positions.FindAsync(id);
            if (position != null)
            {
                _context.Positions.Remove(position);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PositionExists(int id)
        {
            return (_context.Positions?.Any(e => e.PositionID == id)).GetValueOrDefault();
        }

        //Major drop down
        public SelectList GetMajors()
        {
            //create a list of all genres
            List<Major> allmajors = _context.Majors.ToList();

            //ADD ALL GENRES
            Major majors = new Major { MajorID = 0, MajorName = "All Majors" };
            allmajors.Add(majors);

            //List of Genres
            SelectList ListofMajors = new SelectList(allmajors.OrderBy(g => g.MajorID), nameof
                (Major.MajorID), nameof(Major.MajorName));

            return ListofMajors;
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
        private MultiSelectList GetAllMajorsMultiSelectList()
        {
            //Get the list of months from the database
            List<Major> majorList = _context.Majors.ToList();

            //add a dummy entry so the user can select all months
            //Major SelectNone = new Major() { MajorID = 0, MajorName = "All Months" };
            //majorList.Add(SelectNone);

            //convert the list to a SelectList by calling SelectList constructor
            //MonthID and MonthName are the names of the properties on the Month class
            //MonthID is the primary key
            MultiSelectList majorSelectList = new MultiSelectList(majorList.OrderBy(m => m.MajorID), "MajorID", "MajorName");

            //return the MultiSelectList
            return majorSelectList;
        }
    }
}
