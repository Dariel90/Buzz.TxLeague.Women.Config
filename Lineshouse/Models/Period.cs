using System;

namespace Buzz.TxLeague.Women.Config.Lineshouse.Models
{
    public class Period
    {
        public uint Id { get; set; }
        public DateTime? Date { get; set; }
        public Event Event { get; set; }
        public PeriodType PeriodType { get; set; }
    }
}