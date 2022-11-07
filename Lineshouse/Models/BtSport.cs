using System.Collections.Generic;

namespace Buzz.TxLeague.Women.Config.Lineshouse.Models
{
    public class BtSport
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<TournamentMap> Tournaments { get; set; }
    }
}