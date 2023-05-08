using System;
using System.Collections.Generic;
using sp23Team13FinalProject.Models;
using sp23Team13FinalProject.DAL;// Update with the correct namespace for your Company model class

namespace sp23Team13FinalProject.Seeding // Update with your actual namespace
{
    public class SeedCompanies
    {
        public static void SeedAllCompanies(AppDbContext db) // Update with your actual database context name
        {
            List<Company> companies = new List<Company>
            {
                new Company { CompanyName = "Shell", Email = "shell@example.com", Description = "Shell Oil Company, including its consolidated companies and its share in equity companies, is one of America's leading oild and natural gas producers, natural gas marketers, gasoline marketers and petrochemical manufacturers.",Industry1 = "Energy", Industry2 = "Chemicals" },
                new Company { CompanyName = "Deloitte", Email="deloitte@example.com", Description="Deloitte is one of the leading professional services organizations in the United States specializing in audit, tax, consulting, and financial advisory services with clients in more than 20 industries.", Industry1 = "Accounting", Industry2 = "Consulting", Industry3 = "Technology" },
                new Company { CompanyName = "Capital One", Email = "capitalone@example.com", Description="Capital One offers a broad spectrum of financial products and services to consumers, small businesses and commercial clients.", Industry1 = "Financial Services" },
                new Company { CompanyName = "Texas Instruments", Email="texasinstruments@example.com", Description="TI is one of the world’s largest global leaders in analog and digital semiconductor design and manufacturing", Industry1 = "Manufacturing" },
                new Company { CompanyName = "Hilton Worldwide", Email = "hiltonworldwide@example.com", Description="Hilton Worldwide offers business and leisure travelers the finest in accommodations, service, amenities and value.", Industry1 = "Hospitality" },
                new Company { CompanyName = "Accenture", Email="accenture@example.com",Description="Accenture is a global management consulting, technology services and outsourcing company.", Industry1 = "Consulting", Industry2 = "Technology" },
                new Company { CompanyName = "Adlucent", Email="adlucent@example.com", Description="Adlucent is a technology and analytics company specializing in selling products through the Internet for online retail clients.", Industry1 = "Marketing", Industry2 = "Technology" },
                new Company { CompanyName = "Academy Sports & Outdoors", Email="academysports@example.com", Description="Academy Sports is intensely focused on being a leader in the sporting goods, outdoor and lifestyle retail arena.", Industry1 = "Retail"}

            };

            db.Companies.AddRange(companies);
            db.SaveChanges();
        }
    }
}
