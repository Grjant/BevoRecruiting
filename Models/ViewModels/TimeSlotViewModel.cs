using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace sp23Team13FinalProject.Models.ViewModels
{
    public class TimeSlotViewModel
    {
        public Int32 SelectedPositionId { get; set; }
        public String SelectedInterviewId { get; set; }
        public List<SelectListItem> TimeSlots { get; set; }
    }

}

