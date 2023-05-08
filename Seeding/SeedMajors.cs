using System;
using System.Collections.Generic;
using sp23Team13FinalProject.Models;
using sp23Team13FinalProject.DAL;// Update with the correct namespace for your Company model class

namespace sp23Team13FinalProject.Seeding // Update with your actual namespace
{
    public class SeedMajors
    {
        public static void SeedAllMajors(AppDbContext db) // Update with your actual database context name
        {
            List<Major> majors = new List<Major>
            {
                new Major { MajorName = "MIS" },
                new Major { MajorName = "Finance" },
                new Major { MajorName = "Accounting" },
                new Major { MajorName = "Supply Chain Management" },
                new Major { MajorName = "Marketing" },
                new Major { MajorName = "Business Honors" },
                new Major { MajorName = "Management" }

            };

            db.Majors.AddRange(majors);
            db.SaveChanges();
        }
    }
}