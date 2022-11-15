using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Buzz.TxLeague.Women.Config.Utils
{
    // using System.Xml.Serialization;
    // XmlSerializer serializer = new XmlSerializer(typeof(TournamentInfo));
    // using (StringReader reader = new StringReader(xml))
    // {
    //    var test = (TournamentInfo)serializer.Deserialize(reader);
    // }

    [XmlRoot(ElementName = "sport")]
    public class Sport
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "category")]
    public class Category
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "current_season")]
    public class CurrentSeason
    {
        [XmlAttribute(AttributeName = "start_date")]
        public DateTime StartDate { get; set; }

        [XmlAttribute(AttributeName = "end_date")]
        public DateTime EndDate { get; set; }

        [XmlAttribute(AttributeName = "year")]
        public int Year { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "tournament")]
    public class Tournament
    {
        [XmlElement(ElementName = "sport")]
        public Sport Sport { get; set; }

        [XmlElement(ElementName = "category")]
        public Category Category { get; set; }

        [XmlElement(ElementName = "current_season")]
        public CurrentSeason CurrentSeason { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "season")]
    public class Season
    {
        [XmlAttribute(AttributeName = "start_date")]
        public DateTime StartDate { get; set; }

        [XmlAttribute(AttributeName = "end_date")]
        public DateTime EndDate { get; set; }

        [XmlAttribute(AttributeName = "year")]
        public int Year { get; set; }

        [XmlAttribute(AttributeName = "tournament_id")]
        public string TournamentId { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "round")]
    public class Round
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

    [XmlRoot(ElementName = "coverage_info")]
    public class CoverageInfo
    {
        [XmlAttribute(AttributeName = "live_coverage")]
        public bool LiveCoverage { get; set; }
    }

    [XmlRoot(ElementName = "reference_id")]
    public class ReferenceId
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "value")]
        public int Value { get; set; }
    }

    [XmlRoot(ElementName = "reference_ids")]
    public class ReferenceIds
    {
        [XmlElement(ElementName = "reference_id")]
        public ReferenceId ReferenceId { get; set; }
    }

    [XmlRoot(ElementName = "competitor")]
    public class Competitor
    {
        [XmlElement(ElementName = "reference_ids")]
        public ReferenceIds ReferenceIds { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "abbreviation")]
        public string Abbreviation { get; set; }

        [XmlAttribute(AttributeName = "country")]
        public string Country { get; set; }

        [XmlAttribute(AttributeName = "gender")]
        public string Gender { get; set; }

        [XmlAttribute(AttributeName = "country_code")]
        public string CountryCode { get; set; }
    }

    [XmlRoot(ElementName = "group")]
    public class Group
    {
        [XmlElement(ElementName = "competitor")]
        public List<Competitor> Competitor { get; set; }
    }

    [XmlRoot(ElementName = "groups")]
    public class Groups
    {
        [XmlElement(ElementName = "group")]
        public Group Group { get; set; }
    }

    [XmlRoot(ElementName = "tournament_info")]
    public class TournamentInfo
    {
        [XmlElement(ElementName = "tournament")]
        public Tournament Tournament { get; set; }

        [XmlElement(ElementName = "season")]
        public Season Season { get; set; }

        [XmlElement(ElementName = "round")]
        public Round Round { get; set; }

        [XmlElement(ElementName = "coverage_info")]
        public CoverageInfo CoverageInfo { get; set; }

        [XmlElement(ElementName = "groups")]
        public Groups Groups { get; set; }

        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }

        [XmlAttribute(AttributeName = "xsi")]
        public string Xsi { get; set; }

        [XmlAttribute(AttributeName = "generated_at")]
        public DateTime GeneratedAt { get; set; }

        [XmlAttribute(AttributeName = "schemaLocation")]
        public string SchemaLocation { get; set; }
    }
}