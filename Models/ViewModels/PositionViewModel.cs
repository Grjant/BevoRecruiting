using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace sp23Team13FinalProject.Models.ViewModels
{
    public class PositionViewModel
    {
        public Int32 PositionID { get; set; }

        [Display(Name = "Position")]
        public String PositionTitle { get; set; }

        [Display(Name = "Type")]
        public PositionType PositionType { get; set; }

        public String Description { get; set; }

        public String Location { get; set; }

        [Display(Name = "Deadline")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Deadline { get; set; }

        // Add this property for the selected majors
        [Display(Name = "Applicable Majors")]
        public int[] SelectedMajorID { get; set; }

        [Display(Name = "Company")]
        public int SelectedCompanyID { get; set; }

        //Company??

        //ignore applications too
    }
}
