using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public interface ISubCompetition
    {
        Genders Gender { get; set; }
        Difficulties Difficulty { get; set; }
        string Id { get; set; }
        DateTime Date { get; set; }
        decimal Price { get; set; }
        int Quantity { get; set; }
        int QuantityPerTeam { get; set; }
        int ConfirmedParticipants { get; set; }
    }
}
