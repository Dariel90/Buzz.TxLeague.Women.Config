using System;
using System.Collections.Generic;
using System.Linq;
using Buzz.TxLeague.Women.Config.Lineshouse;
using Microsoft.EntityFrameworkCore;
using TxLeagueTool.Api;
using Buzz.TxLeague.Women.Config.Utils;
using Buzz.TxLeague.Women.Config.Dgs;
using System.IO;

namespace Buzz.TxLeague.Women.Config
{
    public class WomenLeagueCleanerHandler
    {
        private LineshouseContext _lineshouseContext;
        private DgsContext _dgsContext;
        private string _fileWomenLeague = "women-league-without-w.log";
        private string _fileMenLeague = "men-league-with-w.log";

        private Dictionary<int, (string sportName, int lsSportId)> _sports = new()
        {
            //{ 7, ("baseball", 3) },
            //{ 3, ("basketball", 4) },
            //{ 6, ("football", 15) },
            //{ 2, ("hockey", 19) },
            { 1, ("soccer", 29) },
            //{ 5, ("tennis", 33) },
            //{ 8, ("handball", 18) },
            //{ 13, ("volleyball", 34) },
            //{ 4, ("rugby-union", 27) },
            //{ 29, ("rugby-league", 26) },
            //{ 26, ("aussie-rules", 39) },
            //{ 18, ("mma", 22) },
            //{ 19, ("boxing", 6) }
        };

        public WomenLeagueCleanerHandler(LineshouseContext context, DgsContext dgsContext)
        {
            _lineshouseContext = context;
            _dgsContext = dgsContext;
        }

        public void Handle()
        {
            _lineshouseContext.TxLeagueMaps.Load();
            _lineshouseContext.Leagues.Where(m => m.SportId != 12).ToList();

            var a = _lineshouseContext.TxLeagueMaps.Local;
            var b = _lineshouseContext.Leagues.Local;

            foreach (var sport in _sports)
            {
                Console.WriteLine($"{sport.Value.sportName} starts....");

                var competitions = CompetitionProvider.GetCompetitions(sport.Key).Result.TxLeague;
                var womenCompetitions = GetCompetitions(competitions.ToList(), true).ToList();
                foreach (var apiWomenCompetition in womenCompetitions)
                    ProcessTxWomenCompetition(apiWomenCompetition);

                var menCompetitions = GetCompetitions(competitions.ToList(), false).ToList();
                foreach (var apiMenCompetition in menCompetitions)
                    ProcessTxMenCompetition(apiMenCompetition);
            }

            _lineshouseContext.SaveChanges();
        }

        private IEnumerable<TxCompetition> GetCompetitions(IEnumerable<TxCompetition> txCompetitions, bool isWomen = true)
        {
            return txCompetitions.Where(x => x.mgname.StartsWith("W") == isWomen
                        || StringContainsWomen(x.name, "(w)") == isWomen || StringContainsWomen(x.name, "women") == isWomen
                        || StringContainsWomen(x.name, "femenie") == isWomen || StringContainsWomen(x.name, "mulheres") == isWomen
                        || StringContainsWomen(x.name, "femenina") == isWomen || StringContainsWomen(x.name, "féminin") == isWomen
                        || StringContainsWomen(x.name, "féminine") == isWomen
                        || StringContainsWomen(x.name, "feminine") == isWomen || StringContainsWomen(x.name, "WTA") == isWomen
                        || StringContainsWomen(x.name, "femrave") == isWomen || StringContainsWomen(x.name, "feminino") == isWomen
                        || StringContainsWomen(x.name, "femenino") == isWomen || StringContainsWomen(x.name, "feminina") == isWomen
                        || StringContainsWomen(x.name, "feminin") == isWomen || StringContainsWomen(x.name, "mujeres") == isWomen
                        || StringContainsWomen(x.name, "women's") == isWomen || StringContainsWomen(x.name, "femminile") == isWomen);
        }

        private bool StringContainsWomen(string name, string match)
        {
            return name.ToLower().Contains(match);
        }

        private void ProcessTxWomenCompetition(TxCompetition apiCompetition)
        {
            Console.WriteLine($"Women Competition: {apiCompetition.pgname}");

            var txWomenCompetition = _lineshouseContext.TxLeagueMaps.Find(apiCompetition.pgid);

            if (txWomenCompetition != null)
            {
                var womenLeague = _lineshouseContext.Leagues.Find(txWomenCompetition.LeagueId);
                if (womenLeague == null) return;
                if (womenLeague.Name.ToLower().Contains("(w)")) return;
                WriteToFile($"cnid={apiCompetition.cnid},cgid={apiCompetition.cgid},name=\"{apiCompetition.name}\",pgname=\"{apiCompetition.pgname}\",mgname=\"{apiCompetition.mgname}\",spid={1},lineshousespid={29}, pgId={apiCompetition.pgid}, ->  {txWomenCompetition.LeagueId}", _fileWomenLeague);
            }
        }

        private void ProcessTxMenCompetition(TxCompetition apiCompetition)
        {
            Console.WriteLine($"Men Competition: {apiCompetition.pgname}");

            var txmenCompetition = _lineshouseContext.TxLeagueMaps.Find(apiCompetition.pgid);

            if (txmenCompetition != null)
            {
                var menLeague = _lineshouseContext.Leagues.Find(txmenCompetition.LeagueId);
                if (menLeague == null) return;
                if (menLeague.Name.ToLower().Contains("(w)"))
                    WriteToFile($"cnid={apiCompetition.cnid},cgid={apiCompetition.cgid},name=\"{apiCompetition.name}\",pgname=\"{apiCompetition.pgname}\",mgname=\"{apiCompetition.mgname}\",spid={1},lineshousespid={29}, pgId={apiCompetition.pgid}, ->  {txmenCompetition.LeagueId}", _fileMenLeague);
            }
        }

        private void WriteToFile(string msg, string filename = "log.txt")
        {
            using (FileStream fs = new FileStream(filename, FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(msg);
            }
        }
    }
}