using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Buzz.TxLeague.Women.Config.Dgs.Models.DGS
{
    public partial class League
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short IdLeague { get; set; }
        public string IdSport { get; set; }
        public short IdLeagueRegion { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public short LeagueOrder { get; set; }
        public bool? TeamFkrequired { get; set; }
        public DateTime LastModification { get; set; }
        public short LastModificationUser { get; set; }
    }
}
