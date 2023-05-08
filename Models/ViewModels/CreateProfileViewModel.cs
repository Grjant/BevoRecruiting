using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace sp23Team13FinalProject.Models.ViewModels
{
	public class CreateProfileViewModel
	{
        public string? Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        // essentially this is the CSO viewmodel	
    }

    public class CreateStudentProfileViewModel : CreateProfileViewModel
    {
        public bool ActiveStatus { get; set; }

        [DisplayName("Major:")]
        public Int32? SelectedMajorID { get; set; }

        public PositionType? PositionType { get; set; }

        public DateTime? GraduationDate { get; set; }

        [Range(0, 4, ErrorMessage = "The GPA field must be between 0 and 4.")]
        public Decimal? GPA { get; set; }
    }

    public class CreateRecruiterProfileViewModel : CreateProfileViewModel
	{
        public bool ActiveStatus { get; set; }

        [DisplayName("Company:")]
        public Int32? SelectedCompanyID { get; set; }
        // remember to add if the company is not found in db, prompt to create a company profile

    }
}

