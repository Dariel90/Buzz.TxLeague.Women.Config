using Buzz.TxLeague.Women.Config.Lineshouse.Enums;
using System;

namespace Buzz.TxLeague.Women.Config.Lineshouse.Models
{
    public class Fixture
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public bool IsMain { get; set; }
        public bool IsLive { get; set; }
        public FixtureType Type { get; set; }
        public DateTime? Date { get; set; }
        public Event Event { get; set; }
    }
}