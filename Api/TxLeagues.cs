using System.Xml.Serialization;

namespace TxLeagueTool.Api
{
    [XmlRootAttribute("competitions")]
    public class TxCompetitions
    {
        [XmlElement("competition")]
        public TxCompetition[] TxLeague { get; set; }
    }
}
