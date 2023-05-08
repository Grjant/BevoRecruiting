using System;
using System.Collections.Generic;
using System.Linq;
using sp23Team13FinalProject.Models;
using sp23Team13FinalProject.DAL;
using Microsoft.EntityFrameworkCore;

namespace sp23Team13FinalProject.Seeding
{
    public static class SeedApplications
    {
        public static void SeedAllApplications(AppDbContext db)
        {
            if (!db.Applications.Any())
            {
                List<Application> applications = new List<Application>
                {
                    new Application
                    {
                        ApplicationStatus = ApplicationStatus.Accepted,
                        Student = db.Users.First(n => n.FirstName == "Eric" && n.LastName == "Stuart"),
                        Position = db.Positions.First(p => p.PositionTitle == "Account Manager" && p.Companies.CompanyName == "Deloitte")
                    },
                    new Application
                    {
                        ApplicationStatus = ApplicationStatus.Accepted,
                        Student = db.Users.First(n => n.FirstName == "Christopher" && n.LastName == "Baker"),
                        Position = db.Positions.First(p => p.PositionTitle == "Web Development" && p.Companies.CompanyName == "Capital One")
                    },
                    new Application
                    {
                        ApplicationStatus = ApplicationStatus.Accepted,
                        Student = db.Users.First(n => n.FirstName == "Eryn" && n.LastName == "Rice"),
                        Position = db.Positions.First(p => p.PositionTitle == "Amenities Analytics Intern" && p.Companies.CompanyName == "Hilton Worldwide")
                    },
                    new Application
                    {
                        ApplicationStatus = ApplicationStatus.Accepted,
                        Student = db.Users.First(n => n.FirstName == "Tesa" && n.LastName == "Freeley"),
                        Position = db.Positions.First(p => p.PositionTitle == "Amenities Analytics Intern" && p.Companies.CompanyName == "Hilton Worldwide")
                    },
                    new Application
                    {
                        ApplicationStatus = ApplicationStatus.Accepted,
                        Student = db.Users.First(n => n.FirstName == "Lim" && n.LastName == "Chou"),
                        Position = db.Positions.First(p => p.PositionTitle == "Amenities Analytics Intern" && p.Companies.CompanyName == "Hilton Worldwide")
                    },
                    new Application
                    {
                        ApplicationStatus = ApplicationStatus.Accepted,
                        Student = db.Users.First(n => n.FirstName == "Brad" && n.LastName == "Ingram"),
                        Position = db.Positions.First(p => p.PositionTitle == "Supply Chain Internship" && p.Companies.CompanyName == "Shell")
                    },
                    new Application
                    {
                        ApplicationStatus = ApplicationStatus.Accepted,
                        Student = db.Users.First(n => n.FirstName == "Sarah" && n.LastName == "Saunders"),
                        Position = db.Positions.First(p => p.PositionTitle == "Supply Chain Internship" && p.Companies.CompanyName == "Shell")
                    },
                    new Application
                    {
                        ApplicationStatus = ApplicationStatus.Accepted,
                        Student = db.Users.First(n => n.FirstName == "Chuck" && n.LastName == "Luce"),
                        Position = db.Positions.First(p => p.PositionTitle == "Accounting Intern" && p.Companies.CompanyName == "Deloitte")
                    },
                    new Application
                    {
                        ApplicationStatus = ApplicationStatus.Accepted,
                        Student = db.Users.First(n => n.FirstName == "Jim Bob" && n.LastName == "Evans"),
                        Position = db.Positions.First(p => p.PositionTitle == "Account Manager" && p.Companies.CompanyName == "Deloitte")
                    },
                    new Application
                    {
                        ApplicationStatus = ApplicationStatus.Pending,
                        Student = db.Users.First(n => n.FirstName == "Reagan" && n.LastName == "Wood"),
                        Position = db.Positions.First(p => p.PositionTitle == "Account Manager" && p.Companies.CompanyName == "Deloitte")

                        },
                };

                db.Applications.AddRange(applications);
                db.SaveChanges();
            }
        }
    }
}
