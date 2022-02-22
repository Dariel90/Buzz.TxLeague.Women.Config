using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TxLeagueTool.Api
{
    public static class CompetitionProvider
    {
        private const string URL = "https://xml2.txodds.com/feed/competitions.php?ident=trial_papasport&passwd=PZp8wBV4QM&sid=21,22,23,24&spid={0}";

        public static async Task<TxCompetitions> GetCompetitions(int sportId)
        {
            TxCompetitions result = null;

            using (HttpClient client = new HttpClient())
            {
                var url = string.Format(URL, sportId);
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    XmlSerializer xs = new XmlSerializer(typeof(TxCompetitions));
                    result = (TxCompetitions)xs.Deserialize(await response.Content.ReadAsStreamAsync());
                }
            }

            return result;
        }
    }
}