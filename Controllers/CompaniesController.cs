using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sp23Team13FinalProject.DAL;
using sp23Team13FinalProject.Models;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.Design;

namespace sp23Team13FinalProject.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CompaniesController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Companies
        [AllowAnonymous]
        public async Task<IActionResult> Index(List<Company>? companies)
        {
            if (companies != null && companies.Any())
            {
                return View(companies);
            }
            else
            {
                List<Company> AllCompanies = _context.Companies.ToList();
                return View(AllCompanies);
            }
        }




        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .Include(p => p.Positions)
                .ThenInclude(c => c.Applications)
                .ThenInclude(i => i.Interview)
                .FirstOrDefaultAsync(m => m.CompanyID == id);

            ViewBag.InterviewDates = _context.Interviews
            .Include(p => p.Position)
            .ThenInclude(p => p.Companies)
            .Where(i => id == i.Position.Companies.CompanyID)
            .Select(i => new { i.Date, i.StartTime })
            .ToList();

            //    .Where(p => p.Companies.CompanyID == id)
            //    .SelectMany(p => p.Applications)
            //    .SelectMany(a => a.Interviews)
            //    .ToList();

            //var company2 =  _context.Interviews
            //    .Include(i => i.Position)
            //    .ThenInclude(i => i.Companies)
            //    .Where(i => i.Recruiter.Company == )

            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }


        // use a viewbag to populate




        // GET: Companies/Create
        [Authorize(Roles = "CSO")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CompanyID,CompanyName,Industry1,Industry2,Industry3")] Company company)
        {
            if (ModelState.IsValid)
            {
                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        [Authorize(Roles = "CSO, Recruiter")]
        public async Task<IActionResult> Edit(int? id)
        {

            // get user
            // if CSO, let them in


            // if not, they must be recruiter
            // check their companyId against the id that was passed in

            var currentUser = await _userManager.GetUserAsync(User);

            if (User.IsInRole("CSO"))
            {
                // CSO can edit any company
                var company = await _context.Companies.FindAsync(id);
                if (company == null)
                {
                    return NotFound();
                }
                return View(company);
            }
            else if (User.IsInRole("Recruiter"))
            {
                // Recruiter can only edit their own company
                var recruiter = await _context.Users.Include(r => r.Company).FirstOrDefaultAsync(r => r.Id == currentUser.Id);

                if (recruiter == null || recruiter.Company.CompanyID != id)
                {
                    return Forbid();
                }
                var company = await _context.Companies.FindAsync(id);
                if (company == null)
                {
                    return NotFound();
                }
                return View(company);
            }
            else
            {
                return Forbid();
            }

        }


        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CompanyID,CompanyName,Industry1,Industry2,Industry3, Description, Email")] Company company)
        {
            if (id != company.CompanyID)
            {
                return NotFound();
            }

            ModelState.Remove("Positions");
            ModelState.Remove("Recruiters");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.CompanyID))
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
            return View(company);
        }

        [AllowAnonymous]
        public IActionResult DetailedSearch()
        {
            return View();
        }

        public IActionResult DisplaySearchResults(CompanySearch svm)
        {
            //Initial Query
            var query = _context.Companies.Include(c => c.Positions).AsQueryable();

            //Search by Company Name
            if (!String.IsNullOrEmpty(svm.NameSearch))
            {
                query = query.Where(c => c.CompanyName.Contains(svm.NameSearch));
            }

            //Search by Industry
            if (!String.IsNullOrEmpty(svm.IndustrySearch))
            {
                query = query.Where(c => c.Industry1.Contains(svm.IndustrySearch) || c.Industry2.Contains(svm.IndustrySearch) || c.Industry3.Contains(svm.IndustrySearch));

            }

            //Search by Location
            if (!String.IsNullOrEmpty(svm.LocationSearch))
            {
                query = query.Where(c => c.Positions.Any(p => p.Location != null && p.Location.Contains(svm.LocationSearch)));
            }

            //Search by Position Type
            if (svm.PositionTypeSearch.HasValue)
            {
                query = query.Where(c => c.Positions.Any(p => p.PositionType == svm.PositionTypeSearch.Value));
            }

            List<Company> selectedCompanies = query.ToList();

            return View("Index", selectedCompanies.OrderBy(c => c.CompanyName));
        }



        private bool CompanyExists(int id)
        {
          return (_context.Companies?.Any(e => e.CompanyID == id)).GetValueOrDefault();
        }
    }
}
