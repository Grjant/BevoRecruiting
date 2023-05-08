using System;
using System.ComponentModel.DataAnnotations;
namespace sp23Team13FinalProject.Models.ViewModels
{
	public class RecruiterSch
	{
        [Required(ErrorMessage = "Start Date is required!")]
        public DateTime? SelectedStartDate { get; set; }
        public DateTime? SelectedEndDate { get; set; }
    }
}

