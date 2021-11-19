using System.Collections.Generic;

namespace Buzz.TxLeague.Women.Config.Lineshouse.Models
{
    public class Sport
    {
        public Sport()
        {
            Leagues = new HashSet<League>();
        }

        public short Id { get; set; }
        public string Name { get; set; }
        public ICollection<League> Leagues { get; set; }
    }
}