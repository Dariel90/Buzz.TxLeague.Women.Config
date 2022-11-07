namespace Buzz.TxLeague.Women.Config.Lineshouse.Models
{
    public class TournamentMap
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SportId { get; set; }
        public string CategoryId { get; set; }
        public BtSport Sport { get; set; }
        public uint ExtLeagueId { get; set; }
        public League ExtLeague { get; set; }
    }
}