using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Buzz.TxLeague.Women.Config.Dgs.Models
{
    public class CR_Fixture
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public bool IsMain { get; set; }
        public bool IsLive { get; set; }
        public int Type { get; set; }
        public DateTime? Date { get; set; }
        public bool Disable { get; set; }
        public CR_Event Event { get; set; }

        [NotMapped]
        public bool FixtureEnable
        {
            get
            {
                return !this.Disable;
            }
            set
            {
                this.Disable = !value;
            }
        }
    }
}