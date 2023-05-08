using System;
using System.Collections.Generic;
using System.Linq;
using sp23Team13FinalProject.Models;
using sp23Team13FinalProject.DAL;
using Microsoft.EntityFrameworkCore;

namespace sp23Team13FinalProject.Seeding
{
    public static class SeedPositions
    {
        public static void SeedAllPositions(AppDbContext db)
        {
            if (!db.Positions.Any())
            {
                List<Position> positions = new List<Position>
                {
                    new Position
                    {
                        PositionTitle = "Financial Planning Intern",
                        Description = "",
                        PositionType = PositionType.Internship,
                        Companies = db.Companies.First(c => c.CompanyName == "Academy Sports & Outdoors"),
                        Location = "Orlando, Florida",
                        ApplicableMajors = new List<Major>
                        {
                            db.Majors.First(m => m.MajorName == "Finance"),
                            db.Majors.First(m => m.MajorName == "Accounting" ),
                            db.Majors.First(m => m.MajorName == "Business Honors" )
                        },
                        Deadline = new DateTime(2023, 6, 1)
                    },
                    new Position
                    {
                        PositionTitle = "Digital Product Manager",
                        Description = "",
                        PositionType = PositionType.FullTime,
                        Companies = db.Companies.First(c => c.CompanyName == "Academy Sports & Outdoors"),
                        Location = "Houston, Texas",
                        ApplicableMajors = new List<Major>
                        {
                            db.Majors.First(m => m.MajorName == "MIS" ),
                            db.Majors.First(m => m.MajorName == "Marketing" ),
                            db.Majors.First(m => m.MajorName == "Business Honors" ),
                            db.Majors.First(m => m.MajorName == "Management" )
                        },
                        Deadline = new DateTime(2023, 6, 1)
                    },
                    new Position
                    {
                        PositionTitle = "Consultant",
                        Description = "Full-time consultant position",
                        PositionType = PositionType.FullTime,
                        Companies = db.Companies.First(c => c.CompanyName == "Accenture"),
                        Location = "Houston, Texas",
                        ApplicableMajors = new List<Major>
                     {
                        db.Majors.First(m => m.MajorName == "MIS" ),
                        db.Majors.First(m => m.MajorName == "Accounting" ),
                        db.Majors.First(m => m.MajorName == "Business Honors" )
                    },
                    Deadline = new DateTime(2023, 4, 15)
                    },
                                    new Position
                    {
                    PositionTitle = "Digital Intern",
                    Description = "",
                    PositionType = PositionType.Internship,
                    Companies = db.Companies.First(c => c.CompanyName == "Accenture"),
                    Location = "Dallas, Texas",
                    ApplicableMajors = new List<Major>
                    {
                        db.Majors.First(m => m.MajorName == "MIS" ),
                        db.Majors.First(m => m.MajorName == "Marketing")
                    },
                    Deadline = new DateTime(2023, 5, 20)
                    },
                                    new Position
                    {
                    PositionTitle = "Marketing Intern",
                    Description = "Help our marketing team develop new advertising strategies for local Austin businesses",
                    PositionType = PositionType.Internship,
                    Companies = db.Companies.First(c => c.CompanyName == "Adlucent"),
                    Location = "Austin, Texas",
                    ApplicableMajors = new List<Major>
                    {
                        db.Majors.First(m => m.MajorName == "Marketing" )
                    },
                    Deadline = new DateTime(2023, 4, 30)
                    },
                                    new Position
                    {
                    PositionTitle = "Web Development",
                    Description = "Developing a great new website for customer portfolio management",
                    PositionType = PositionType.FullTime,
                    Companies = db.Companies.First(c => c.CompanyName == "Capital One"),
                    Location = "Richmond, Virginia",
                    ApplicableMajors = new List<Major>
                    {
                        db.Majors.First(m => m.MajorName == "MIS" )
                    },
                    Deadline = new DateTime(2023, 3, 14)
                    }
                    ,
                    new Position
                    {
                    PositionTitle = "Analyst Development Program",
                    Description = "",
                    PositionType = PositionType.Internship,
                    Companies = db.Companies.First(c => c.CompanyName == "Capital One"),
                    Location = "Plano, Texas",
                    ApplicableMajors = new List<Major>
                    {
                        db.Majors.First(m => m.MajorName == "Finance" ),
                        db.Majors.First(m => m.MajorName == "MIS" ),
                        db.Majors.First(m => m.MajorName == "Business Honors" )
                    },
                    Deadline = new DateTime(2023, 5, 20)
                    }
                    ,
                                    new Position
                    {
                    PositionTitle = "Accounting Intern",
                    Description = "Work in our audit group",
                    PositionType = PositionType.Internship,
                    Companies = db.Companies.First(c => c.CompanyName == "Deloitte"),
                    Location = "Austin, Texas",
                    ApplicableMajors = new List<Major>
                    {
                        db.Majors.First(m => m.MajorName == "Accounting" )
                    },
                    Deadline = new DateTime(2023, 5, 1)
                    }
                    ,
                                    new Position
                    {
                    PositionTitle = "Account Manager",
                    Description = "",
                    PositionType = PositionType.FullTime,
                    Companies = db.Companies.First(c => c.CompanyName == "Deloitte"),
                    Location = "Dallas, Texas",
                    ApplicableMajors = new List<Major>
                    {
                        db.Majors.First(m => m.MajorName == "Accounting" ),
                        db.Majors.First(m => m.MajorName == "Business Honors" )
                    },
                    Deadline = new DateTime(2023, 2, 25)
                    }
                    ,
                                    new Position
                    {
                    PositionTitle = "Amenities Analytics Intern",
                    Description = "Help analyze our amenities and customer rewards programs",
                    PositionType = PositionType.Internship,
                    Companies = db.Companies.First(c => c.CompanyName == "Hilton Worldwide"),
                    Location = "New York, New York",
                    ApplicableMajors = new List<Major>
                    {
                        db.Majors.First(m => m.MajorName == "Finance" ),
                        db.Majors.First(m => m.MajorName == "MIS" ),
                        db.Majors.First(m => m.MajorName == "Marketing" ),
                        db.Majors.First(m => m.MajorName == "Business Honors" )
                    },
                    Deadline = new DateTime(2023, 3, 30)
                    }
                    ,
                                    new Position
                    {
                    PositionTitle = "Supply Chain Internship",
                    Description = "",
                    PositionType = PositionType.Internship,
                    Companies = db.Companies.First(c => c.CompanyName == "Shell"),
                    Location = "Houston, Texas",
                    ApplicableMajors = new List<Major>
                    {
                        db.Majors.First(m => m.MajorName == "Supply Chain Management" )
                    },
                    Deadline = new DateTime(2023, 5, 5)
                    }
                    ,new Position
                    {
                    PositionTitle = "Procurements Associate",
                    Description = "Handle procurement and vendor accounts",
                    PositionType = PositionType.FullTime,
                    Companies = db.Companies.First(c => c.CompanyName == "Shell"),
                    Location = "Houston, Texas",
                    ApplicableMajors = new List<Major>
                    {
                        db.Majors.First(m => m.MajorName == "Supply Chain Management" )
                    },
                    Deadline = new DateTime(2023, 5, 30)
                    }
                    ,
                    new Position
                    {
                    PositionTitle = "Sales Rotational Program",
                    Description = "",
                    PositionType = PositionType.FullTime,
                    Companies = db.Companies.First(c => c.CompanyName == "Texas Instruments"),
                    Location = "Dallas, Texas",
                    ApplicableMajors = new List<Major>
                    {
                        db.Majors.First(m => m.MajorName == "Marketing" ),
                        db.Majors.First(m => m.MajorName == "Management" ),
                        db.Majors.First(m => m.MajorName == "Finance"),
                        db.Majors.First(m => m.MajorName == "Accounting" )
                    },
                    Deadline = new DateTime(2023, 5, 30)
                    }



                    // Add other positions with similar structure
                };

                db.Positions.AddRange(positions);
                db.SaveChanges();
            }
        }
    }
}
