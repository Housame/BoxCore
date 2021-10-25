using System;
using System.Collections.Generic;

namespace EventHub.Models.Entities
{
    public partial class Box
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }
        public string Url { get; set; }
        public string Location { get; set; }
        public string Owner { get; set; }
    }
}
