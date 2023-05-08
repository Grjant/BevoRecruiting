using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using sp23Team13FinalProject.Models;
namespace sp23Team13FinalProject.Models.ViewModels
{
    public class ApplicationViewModel
    {
        //public int PositionID { get; set; }
        public string PositionTitle { get; set; }
        public string CompanyName { get; set; }
        public Int32 SelectedPositionID { get; set; }
        public Int32 SelectedApplicationID { get; set; }
        public ApplicationStatus ApplicationStatus { get; set; }
    }
}
