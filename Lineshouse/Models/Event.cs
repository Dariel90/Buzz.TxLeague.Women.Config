using System.Collections.Generic;

namespace Buzz.TxLeague.Women.Config.Lineshouse.Models
{
    public class Event
    {
        public Event()
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int LeagueId { get; set; }
        public League League { get; set; }
        public ICollection<Fixture> Fixtures { get; set; }
    }
}