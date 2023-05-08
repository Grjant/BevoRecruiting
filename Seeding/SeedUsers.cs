using Microsoft.AspNetCore.Identity;

//TODO: Update these using statements to include your project name
using sp23Team13FinalProject.Models;
using sp23Team13FinalProject.Utilities;
using sp23Team13FinalProject.DAL;
using System.Data;

//TODO: Upddate this namespace to match your project name
namespace sp23Team13FinalProject.Seeding
{
    public static class SeedUsers
    {
        public async static Task<IdentityResult> SeedAllUsers(UserManager<AppUser> userManager, AppDbContext context)
        {
            //Create a list of AddUserModels
            List<AddUserModel> AllUsers = new List<AddUserModel>();

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "cbaker@example.com",
                    Email = "cbaker@example.com",
                    FirstName = "Christopher",
                    LastName = "Baker",
                    ActiveStatus = true,
                    Major = context.Majors.First(m => m.MajorID == 1),
                    PositionType = PositionType.FullTime,
                    GraduationDate = new DateTime(2023, 1, 1),
                    GPA = 3.91m
                },
                Password = "bookworm",
                RoleName = "Student"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "limchou@gogle.com",
                    Email = "limchou@gogle.com",
                    FirstName = "Lim",
                    LastName = "Chou",
                    ActiveStatus = true,
                    Major = context.Majors.First(m => m.MajorID == 2),
                    PositionType = PositionType.Internship,
                    GraduationDate = new DateTime(2024, 1, 1),
                    GPA = 2.63m,
                },
                Password = "allyrally",
                RoleName = "Student"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "j.b.evans@aheca.org",
                    Email = "j.b.evans@aheca.org",
                    FirstName = "Jim Bob",
                    LastName = "Evans",
                    ActiveStatus = true,
                    Major = context.Majors.First(m => m.MajorID == 3),
                    PositionType = PositionType.FullTime,
                    GraduationDate = new DateTime(2023, 1, 1),
                    GPA = 2.64m,
                },
                Password = "billyboy",
                RoleName = "Student"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "tfreeley@minnetonka.ci.us",
                    Email = "tfreeley@minnetonka.ci.us",
                    FirstName = "Tesa",
                    LastName = "Freeley",
                    ActiveStatus = true,
                    Major = context.Majors.First(m => m.MajorID == 3),
                    PositionType = PositionType.Internship,
                    GraduationDate = new DateTime(2023, 1, 1),
                    GPA = 3.98m,
                },
                Password = "dustydusty",
                RoleName = "Student"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "ingram@jack.com",
                    Email = "ingram@jack.com",
                    FirstName = "Brad",
                    LastName = "Ingram",
                    ActiveStatus = true,
                    Major = context.Majors.First(m => m.MajorID == 1),
                    PositionType = PositionType.FullTime,
                    GraduationDate = new DateTime(2023, 1, 1),
                    GPA = 3.15m,
                },
                Password = "joejoejoe",
                RoleName = "Student"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "cluce@gogle.com",
                    Email = "cluce@gogle.com",
                    FirstName = "Chuck",
                    LastName = "Luce",
                    ActiveStatus = true,
                    Major = context.Majors.First(m => m.MajorID == 3),
                    PositionType = PositionType.Internship,
                    GraduationDate = new DateTime(2024, 1, 1),
                    GPA = 3.87m,
                },
                Password = "meganr34",
                RoleName = "Student"
            });
            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "erynrice@aoll.com",
                    Email = "erynrice@aoll.com",
                    FirstName = "Eryn",
                    LastName = "Rice",
                    ActiveStatus = true,
                    Major = context.Majors.First(m => m.MajorID == 5),
                    PositionType = PositionType.Internship,
                    GraduationDate = new DateTime(2026, 1, 1),
                    GPA = 3.92m,
                },
                Password = "radgirl",
                RoleName = "Student"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "saunders@pen.com",
                    Email = "saunders@pen.com",
                    FirstName = "Sarah",
                    LastName = "Saunders",
                    ActiveStatus = true,
                    Major = context.Majors.First(m => m.MajorID == 4),
                    PositionType = PositionType.Internship,
                    GraduationDate = new DateTime(2024, 1, 1),
                    GPA = 3.16m,
                },
                Password = "slowwind",
                RoleName = "Student"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "estuart@anchor.net",
                    Email = "estuart@anchor.net",
                    FirstName = "Eric",
                    LastName = "Stuart",
                    ActiveStatus = true,
                    Major = context.Majors.First(m => m.MajorID == 6),
                    PositionType = PositionType.FullTime,
                    GraduationDate = new DateTime(2023, 1, 1),
                    GPA = 3.58m,
                },
                Password = "stewball",
                RoleName = "Student"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "rwood@voyager.net",
                    Email = "rwood@voyager.net",
                    FirstName = "Reagan",
                    LastName = "Wood",      
                    ActiveStatus = true,
                    Major = context.Majors.First(m => m.MajorID == 3),
                    PositionType = PositionType.FullTime,
                    GraduationDate = new DateTime(2023, 1, 1),
                    GPA = 3.78m,
                },
                Password = "xcellent",
                RoleName = "Student"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "michelle@example.com",
                    Email = "michelle@example.com",
                    FirstName = "Michelle",
                    LastName = "Banks",
                    Company = context.Companies.First(c => c.CompanyName == "Accenture"),
                },
                Password = "jVb0Z6",
                RoleName = "Recruiter"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "elowe@netscrape.net",
                    Email = "elowe@netscrape.net",
                    FirstName = "Ernest",
                    LastName = "Lowe",
                    Company = context.Companies.First(c => c.CompanyName == "Shell"),
                },
                Password = "v3n5AV",
                RoleName = "Recruiter"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "mclarence@aool.com",
                    Email = "mclarence@aool.com",
                    FirstName = "Clarence",
                    LastName = "Martin",
                    Company = context.Companies.First(c => c.CompanyName == "Deloitte"),
                },
                Password = "zBLq3S",
                RoleName = "Recruiter"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "nelson.Kelly@aool.com",
                    Email = "nelson.Kelly@aool.com",
                    FirstName = "Kelly",
                    LastName = "Nelson",
                    Company = context.Companies.First(c => c.CompanyName == "Deloitte"),
                },
                Password = "FSb8rA",
                RoleName = "Recruiter"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "sheff44@ggmail.com",
                    Email = "sheff44@ggmail.com",
                    FirstName = "Martin",
                    LastName = "Sheffield",
                    Company = context.Companies.First(c => c.CompanyName == "Texas Instruments"),
                },
                Password = "4XKLsd",
                RoleName = "Recruiter"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "yhuik9.Taylor@aool.com",
                    Email = "yhuik9.Taylor@aool.com",
                    FirstName = "Rachel",
                    LastName = "Taylor",
                    Company = context.Companies.First(c => c.CompanyName == "Hilton Worldwide"),
                },
                Password = "9yhFS3",
                RoleName = "Recruiter"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "taylordjay@aool.com",
                    Email = "taylordjay@aool.com",
                    FirstName = "Allison",
                    LastName = "Taylor",
                    Company = context.Companies.First(c => c.CompanyName == "Adlucent"),
                },
                Password = "Vjb1wl",
                RoleName = "Recruiter"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "smartinmartin.Martin@aool.com",
                    Email = "smartinmartin.Martin@aool.com",
                    FirstName = "Gregory",
                    LastName = "Martinez",
                    Company = context.Companies.First(c => c.CompanyName == "Capital One"),
                },
                Password = "1rKkMW",
                RoleName = "Recruiter"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "tanner@ggmail.com",
                    Email = "tanner@ggmail.com",
                    FirstName = "Jeremy",
                    LastName = "Tanner",
                    Company = context.Companies.First(c => c.CompanyName == "Shell"),
                },
                Password = "w9wPff",
                RoleName = "Recruiter"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "tee_frank@hootmail.com",
                    Email = "tee_frank@hootmail.com",
                    FirstName = "Frank",
                    LastName = "Tee",
                    Company = context.Companies.First(c => c.CompanyName == "Academy Sports & Outdoors"),
                },
                Password = "1EIwbx",
                RoleName = "Recruiter"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "ra@aoo.com",
                    Email = "ra@aoo.com",
                    FirstName = "Allen",
                    LastName = "Rogers"
                },
                Password = "3wCynC",
                RoleName = "CSO"
            });

            AllUsers.Add(new AddUserModel()
            {
                User = new AppUser()
                {
                    UserName = "captain@enterprise.net",
                    Email = "captain@enterprise.net",
                    FirstName = "Jean Luc",
                    LastName = "Picard"
                },
                Password = "Pbon0r",
                RoleName = "CSO"
            });

            //create flag to help with errors
            String errorFlag = "Start";

            //create an identity result
            IdentityResult result = new IdentityResult();
            //call the method to seed the user
            try
            {
                foreach (AddUserModel aum in AllUsers)
                {
                    errorFlag = aum.User.Email;
                    result = await Utilities.AddUser.AddUserWithRoleAsync(aum, userManager, context);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("There was a problem adding the user with email: " 
                    + errorFlag, ex);
            }

            return result;
        }
    }
}
