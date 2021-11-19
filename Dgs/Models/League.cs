using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Buzz.TxLeague.Women.Config.Dgs.Models
{
    public class League
    {
        public League()
        {

        }
        [Key]
        public uint Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ushort SportId { get; set; }

        public Sport Sport { get; set; }
        public Config Config { get; set; }
    }
}
