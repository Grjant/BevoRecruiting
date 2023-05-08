using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace sp23Team13FinalProject.Models.ViewModels
{
	public class AddStudentViewModel
	{
        public List<SelectListItem> Students { get; set; }
        public string SelectedStudentId { get; set; }
        public string SelectedPositionId { get; set; }
    }
}

