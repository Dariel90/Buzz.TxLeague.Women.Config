using System;

namespace Buzz.TxLeague.Women.Config.Dgs.Models
{
    public class CR_Period
    {
        public int Id { get; set; }
        public uint Number { get; set; }
        public string Name { get; set; }
        public DateTime? Date { get; set; }
        public CR_Event Event { get; set; }
    }
}