using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class EditUserVM
    {
        public int Id { get; set; }
        //FirstName
        [Display(Name = "Förnamn:", Prompt = "Förnamn")]
        [Required(ErrorMessage ="Fältet får inte vara tomt")]
        [MaxLength(30, ErrorMessage = "Max 30 bokstäver tillåtet")]
        public string FirstName { get; set; }

        //LastName
        [Display(Name = "Efternamn:", Prompt = "Efternamn")]
        [Required(ErrorMessage = "Fältet får inte vara tomt")]
        [MaxLength(30, ErrorMessage = "Max 30 bokstäver tillåtet")]
        public string LastName { get; set; }

        //Location
        [Display(Name = "Stad:", Prompt = "Stad")]
        [MaxLength(30, ErrorMessage = "Max 30 bokstäver tillåtet")]
        public string Location { get; set; }

        //Birthday
        [Display(Name = "Födelsedatum:", Prompt = "Födelsedatum")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        //Team
        [Display(Name = "Team:", Prompt = "Team")]
        [MaxLength(30, ErrorMessage = "Max 30 bokstäver tillåtet")]
        public string Team { get; set; }

        //Gender
        [Display(Name = "Kön:", Prompt = "Kön")]
        public SingleGenders Gender { get; set; }

        //ShirtSize
        [Display(Name = "Tröjstorlek:", Prompt = "Tröjstorlek")]
        [Required(ErrorMessage = "Måste välja ett alternativ")]
        public ShirtSizes ShirtSize { get; set; }

        public List<SelectListItem> Boxes { get; set; }

        [Display(Name = "Box:")]
        public int? BoxId { get; set; }

        [Display(Name = "Tillåter nyhetsbrev:")]
        public bool IsAllowingNewsLetter { get; set; }

        [Display(Name = "Visa mina uppgifter för andra atleter:")]
        public bool PublicProfile { get; set; }


    }
}
