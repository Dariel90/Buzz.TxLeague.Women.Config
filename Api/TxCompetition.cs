using System.Xml.Serialization;

namespace TxLeagueTool.Api
{
public class TxCompetition
    {
        [XmlAttribute]
        public int cgid { get; set; }
        [XmlAttribute]
        public int spid { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public int pgid { get; set; }
        [XmlAttribute]
        public string mgname { get; set; }
        [XmlAttribute]
        public string pgname { get; set; }
        [XmlAttribute]
        public int sid { get; set; }

        [XmlAttribute]
        public int cnid { get; set; }
    }
}