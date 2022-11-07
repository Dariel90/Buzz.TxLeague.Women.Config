using System.Collections.Generic;

namespace Buzz.TxLeague.Women.Config.Dgs.Models
{
    public class CR_Event
    {
        public uint Id { get; set; }
        public uint LeagueId { get; set; }
        public string Name { get; set; }
        public ushort SportId { get; set; }
        public EventType Type { get; set; }
        public League League { get; set; }
        public ICollection<CR_Fixture> Fixtures { get; set; }
        public ICollection<CR_Period> Periods { get; set; }
    }
}