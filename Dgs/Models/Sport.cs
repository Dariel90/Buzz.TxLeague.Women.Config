using System.Collections.Generic;


namespace Buzz.TxLeague.Women.Config.Dgs.Models
{
    public class Sport
    {
        public Sport()
        {
            Leagues = new HashSet<League>();
        }

        public ushort Id { get; set; }
        public string Name { get; set; }
        public ICollection<League> Leagues { get; set; }
    }
}
