using System;
using System.ComponentModel.DataAnnotations;

namespace Buzz.TxLeague.Women.Config.Lineshouse.Models
{
    public class League
    {
        [Key]
        public int Id { get; set; }
        public short SportId { get; set; }
        public bool IsDefault { get; set; }
        public string Name { get; set; }
        public Sport Sport { get; set; }
    }
}