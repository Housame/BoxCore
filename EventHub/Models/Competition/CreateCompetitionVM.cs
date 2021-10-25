using EventHub.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class CreateCompetitiontVM
    {
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
        //  Event Date
        [DisplayFormat(DataFormatString = "{0:yyyy MMM dd}")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
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
        public List<TeamSubEventVM> TeamSubComp { get; set; }
        public List<SoloSubEventVM> SoloSubComp { get; set; }
        public List<ISubCompetition> SubCompetition { get; set; }
        public List<SelectListItem> Boxes { get; set; }
        [Display(Name = "Box:")]
        public string Box { get; set; }

        public class TeamSubEventVM: ISubCompetition
        {
            public string Id { get; set; }
            [Display(Name = "Datum:")]
            [DisplayFormat(DataFormatString = "{0:yyyy MMM dd}")]
            [DataType(DataType.Date)]
            public DateTime Date { get; set; }
            [Display(Name = "Kön:")]
            public Genders Gender { get; set; }
            [Display(Name = "Klass:")]
            public Difficulties Difficulty { get; set; }
            [Display(Name = "Pris:")]
            [Required(ErrorMessage = "*obligatoriskt")]
            public decimal Price { get; set; }
            [Display(Name = "Antal (deltagare/lag):")]
            public int Quantity { get; set; }
            [Display(Name = "Lagstorlek:")]
            public int QuantityPerTeam { get; set; }
            public int ConfirmedParticipants { get; set; }
            public int CompetitionId { get; set; }
        }

        public class SoloSubEventVM: ISubCompetition
        {
            public string Id { get; set; }
            [DisplayFormat(DataFormatString = "{0:yyyy MMM dd}")]
            [DataType(DataType.Date)]
            public DateTime Date { get; set; }
            public Genders Gender { get; set; }
            public Difficulties Difficulty { get; set; }
            // Price
            [Display(Name = "Pris:")]
            [Required(ErrorMessage = "*obligatoriskt")]
            public decimal Price { get; set; }
            [Display(Name = "Antal (deltagare/lag):")]
            public int Quantity { get; set; }
            public int ConfirmedParticipants { get; set; }
            public int QuantityPerTeam { get; set; }
            public int CompetitionId { get; set; }
        }
    }
}
