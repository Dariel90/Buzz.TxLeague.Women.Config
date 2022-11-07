using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Buzz.TxLeague.Women.Config.Lineshouse.Models
{
    public class League
    {
        public League()
        {
            this.Events = new HashSet<Event>();
        }

        [Key]
        public int Id { get; set; }

        public short SportId { get; set; }
        public string Name { get; set; }
        public Boolean IsDefault { get; set; }
        public Sport Sport { get; set; }
        public ICollection<Event> Events { get; set; }
    }
}