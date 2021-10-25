using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class EditOneCompetVM
    {
        public CompetitionVM Competition { get; set; }
        public class CompetitionVM
        {
            [HiddenInput(DisplayValue = false)]
            public int Id { get; set; }
            // Event name
            [Display(Name = "Namn:")]
            [Required(ErrorMessage = "*obligatoriskt")]
            [MaxLength(50, ErrorMessage = "Max 50 characters allowed")]
            public string Name { get; set; }
            //  StartDate
            [DisplayFormat(DataFormatString = "{0:yyyy MMM dd}")]
            [Display(Name = "Startdatum:")]
            [Required(ErrorMessage = "*obligatoriskt")]
            [DataType(DataType.Date)]
            public DateTime StartDate { get; set; }
            //  EndDate
            [DisplayFormat(DataFormatString = "{0:yyyy MMM dd}")]
            [Display(Name = "Slutdatum:")]
            [Required(ErrorMessage = "*obligatoriskt")]
            [DataType(DataType.Date)]
            public DateTime EndDate { get; set; }
            // Location
            [Display(Name = "Plats:")]
            [Required(ErrorMessage = "*obligatoriskt")]
            [MaxLength(50, ErrorMessage = "Max 50 characters allowed")]
            public string Location { get; set; }
            // Description
            [Display(Name = "Beskrivning:")]
            [Required(ErrorMessage = "*obligatoriskt")]
            public string Description { get; set; }
            public string CreatorId { get; set; }
            [Display(Name = "Publicera:")]
            public bool Published { get; set; }
            [Display(Name = "Öppet för bokningar:")]
            public bool OpenForBookings { get; set; }

            [HiddenInput]
            public bool IsAdmin { get; set; }
            public virtual List<SubCompetitionVM> SubCompetition { get; set; }
        }
        public class SubCompetitionVM
        {
            [HiddenInput(DisplayValue = false)]
            public int Id { get; set; }
            [Display(Name = "Datum:")]
            [DisplayFormat(DataFormatString = "{0:yyyy MMM dd}")]
            [DataType(DataType.Date)]
            public DateTime Date { get; set; }
            [Display(Name = "Kön:")]
            public Genders Gender { get; set; }
            [Display(Name = "Klass:")]
            public Difficulties Difficulty { get; set; }
            [Display(Name = "Antal (deltagare/lag):")]
            public int Quantity { get; set; }
            [Display(Name = "Lagstorlek:")]
            public int? QuantityPerTeam { get; set; }
            public bool HasReservations { get; set; }
            [Display(Name = "Pris:")]
            public decimal Price { get; set; }
            // public int CompetitionId { get; set; }
            [HiddenInput(DisplayValue = false)]
            public SubCompetitionTypes Type { get; set; }
        }
    }
}
