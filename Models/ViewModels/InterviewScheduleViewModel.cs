using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace sp23Team13FinalProject.Models.ViewModels
{
	public class InterviewScheduleViewModel
	{
        public List<Interview> Interviews { get; set; }
        //public List<SelectListItem> Companies { get; set; }
        public List<Company> Companies { get; set; }
        public DateTime? SelectedDate { get; set; }
        public String? SelectedRoomNumber { get; set; }
        public String? SelectedCompany { get; set; }
    }
}

