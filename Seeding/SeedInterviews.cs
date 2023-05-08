using System;
using System.Collections.Generic;
using System.Linq;
using sp23Team13FinalProject.Models;
using sp23Team13FinalProject.DAL;
using Microsoft.EntityFrameworkCore;

namespace sp23Team13FinalProject.Seeding
{
    public static class SeedInterviews
    {
        public static void SeedAllInterviews(AppDbContext db)
        {
            if (!db.Interviews.Any())
            {
                List<Interview> interviews = new List<Interview>
                {
                    new Interview
                    {
                        Date = new DateTime(2023, 5, 13),
                        StartTime = new DateTime(2023, 5, 13, 10, 0, 0),
                        Position = db.Positions.First(x => x.PositionTitle == "Account Manager"),
                        Applicant = db.Users.First(x => x.FirstName == "Eric"),
                        InterviewerName = "Clarence Martin",
                        RoomNumber = "1"
                    },
                    new Interview
                    {
                        Date = new DateTime(2023, 5, 16),
                        StartTime = new DateTime(2023, 5, 16, 14, 0, 0),
                        Position = db.Positions.First(x => x.PositionTitle == "Web Development"),
                        Applicant = db.Users.First(x => x.FirstName == "Christopher"),
                        InterviewerName = "Gregory Martinez",
                        RoomNumber = "2"
                    },
                    new Interview
                    {
                        Date = new DateTime(2023, 4, 1),
                        StartTime = new DateTime(2023, 4, 1, 9, 0, 0),
                        Position = db.Positions.First(x => x.PositionTitle == "Amenities Analytics Intern"),
                        Applicant = db.Users.First(x => x.FirstName == "Eryn"),
                        InterviewerName = "Rachel Taylor",
                        RoomNumber = "1"
                    },
                    new Interview
                    {
                        Date = new DateTime(2023, 4, 1),
                        StartTime = new DateTime(2023, 4, 1, 10, 0, 0),
                        Position = db.Positions.First(x => x.PositionTitle == "Amenities Analytics Intern"),
                        Applicant = db.Users.First(x => x.FirstName == "Tesa"),
                        InterviewerName = "Rachel Taylor",
                        RoomNumber = "1"
                    },
                    new Interview
                    {
                        Date = new DateTime(2023, 4, 2),
                        StartTime = new DateTime(2023, 4, 2, 15, 0, 0),
                        Position = db.Positions.First(x => x.PositionTitle == "Amenities Analytics Intern"),
                        Applicant = db.Users.First(x => x.FirstName == "Lim"),
                        InterviewerName = "Rachel Taylor",
                        RoomNumber = "4"
                    },
                    new Interview
                    {
                        Date = new DateTime(2023, 5, 10),
                        StartTime = new DateTime(2023, 5, 10, 11, 0, 0),
                        Position = db.Positions.First(x => x.PositionTitle == "Supply Chain Internship"),
                        Applicant = db.Users.First(x => x.FirstName == "Sarah"),
                        InterviewerName = "Ernest Lowe",
                        RoomNumber = "1"
                    },
                    new Interview
                    {
                        Date = new DateTime(2023, 5, 16),
                        StartTime = new DateTime(2023, 5, 16, 11, 0, 0),
                        Position = db.Positions.First(x => x.PositionTitle == "Accounting Intern"),
                        Applicant = db.Users.First(x => x.FirstName == "Chuck"),
                        InterviewerName = "Kelly Nelson",
                        RoomNumber = "4"
                    }
                };
            }
        }
    }
}