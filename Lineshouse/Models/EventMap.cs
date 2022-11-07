namespace Buzz.TxLeague.Women.Config.Lineshouse.Models
{
    public class EventMap
    {
        public string Id { get; set; }
        public string SportId { get; set; }
        public string TournamentId { get; set; }
        public int ExtEventId { get; set; }
        public Event ExtEvent { get; set; }
    }
}