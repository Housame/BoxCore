using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EventHub.Models
{
    public class FilterCompetitionsVM
    {
        public List<SelectListItem> Locations { get; set; }
        [Display(Name = "Nivå")]
        public Difficulties Difficulty { get; set; }
        [Display(Name = "Ort")]
        public string SelectedLocation { get; set; }
        [Display(Name ="Startdatum")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        
        [Display(Name = "Slutdatum")]
        public DateTime EndDate { get; set; }
    }
}
