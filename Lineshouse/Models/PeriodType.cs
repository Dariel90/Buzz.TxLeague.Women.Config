namespace Buzz.TxLeague.Women.Config.Lineshouse.Models
{
    public class PeriodType
    {
        public ushort Id { get; set; }
        public ushort SportId { get; set; }
        public string Name { get; set; }
        public ushort Number { get; set; }
        public Sport Sport { get; set; }
    }
}