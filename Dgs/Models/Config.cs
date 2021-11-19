using System;
using System.Collections.Generic;
using System.Text;

namespace Buzz.TxLeague.Women.Config.Dgs.Models
{
    public class Config
    {
        public int Id { get; set; }
        public uint ProfileId { get; set; }
        public uint LeagueId { get; set; }
        public string Data { get; set; }
        public League League { get; set; }
    }
}
