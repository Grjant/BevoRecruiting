using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using sp23Team13FinalProject.Models;

namespace sp23Team13FinalProject.Models.ViewModels
{
    public class InterviewViewModel
    {
        public String SelectedPositionId { get; set; }
        public List<SelectListItem> Positions { get; set; }
    }
}

