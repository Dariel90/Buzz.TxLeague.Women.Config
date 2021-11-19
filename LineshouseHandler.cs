using System;
using System.Collections.Generic;
using System.Linq;
using Buzz.TxLeague.Women.Config.Lineshouse;
using Microsoft.EntityFrameworkCore;
using TxLeagueTool.Api;
using Buzz.TxLeague.Women.Config.Utils;
using Buzz.TxLeague.Women.Config.Dgs;

namespace Buzz.TxLeague.Women.Config
{
    public class WomenLeagueCleanerHandler
    {

        LineshouseContext _lineshouseContext;
        DgsContext _dgsContext;
        Dictionary<int, (string sportName, int lsSportId)> _sports = new()
        {
            { 7, ("baseball", 3) },
            { 3, ("basketball", 4) },
            { 6, ("football", 15) },
            { 2, ("hockey", 19) },
            { 1, ("soccer", 29) },
            { 5, ("tennis", 33) },
            { 8, ("handball", 18) },
            { 13, ("volleyball", 34) },
            { 4, ("rugby-union", 27) },
            { 29, ("rugby-league", 26) },
            { 26, ("aussie-rules", 39) },
            { 18, ("mma", 22) },
            { 19, ("boxing", 6) }
        };

        public WomenLeagueCleanerHandler(LineshouseContext context, DgsContext dgsContext)
        {
            _lineshouseContext = context;
            _dgsContext= dgsContext;
        }


        public void Handle()
        {
            _lineshouseContext.TxLeagueMaps.Load();
            _lineshouseContext.Leagues.Where(m => m.SportId != 12).ToList();

            var a  =_lineshouseContext.TxLeagueMaps.Local;
            var b  =_lineshouseContext.Leagues.Local;

            foreach (var sport in _sports)
            {
                Console.WriteLine($"{sport.Value.sportName} starts....");

                var competitions = CompetitionProvider.GetCompetitions(sport.Key).Result.TxLeague;
                var womenCompetitions = GetWomenCompetitions(competitions.ToList()).ToList();
                foreach (var apiWomenCompetition in womenCompetitions)
                    ProcessTxWomenCompetition(apiWomenCompetition);
            }

            _lineshouseContext.SaveChanges();
        }

        private IEnumerable<TxCompetition> GetWomenCompetitions(IEnumerable<TxCompetition> txCompetitions)
        {
            return txCompetitions.Where(x => x.mgname.StartsWith("W")
                        || StringContainsWomen(x.name, "(w)") || StringContainsWomen(x.name, "women")
                        || StringContainsWomen(x.name, "femenie") || StringContainsWomen(x.name, "mulheres")
                        || StringContainsWomen(x.name, "femenina") || StringContainsWomen(x.name, "féminin")
                        || StringContainsWomen(x.name, "feminine") || StringContainsWomen(x.name, "WTA")
                        || StringContainsWomen(x.name, "femrave") || StringContainsWomen(x.name, "feminino")
                        || StringContainsWomen(x.name, "femenino") || StringContainsWomen(x.name, "feminina")
                        || StringContainsWomen(x.name, "feminin"));
        }

        private bool StringContainsWomen(string name, string match)
        {
            return name.ToLower().Contains(match);
        }

        void ProcessTxWomenCompetition(TxCompetition apiCompetition)
        {
            Console.WriteLine($"Women Competition: {apiCompetition.pgname}");

            var txWomenCompetition = _lineshouseContext.TxLeagueMaps.Find(apiCompetition.pgid);

            if (txWomenCompetition != null)
            {
                var womenLeague = _lineshouseContext.Leagues.Find(txWomenCompetition.LeagueId);
                if (womenLeague == null) return;
                if (womenLeague.Name.Contains("(w)")) return;
                var newWomenLeague = CleanWomenLeagueName(womenLeague);
                womenLeague.Name = $"{newWomenLeague} (w)";
                Console.WriteLine($"Competition 'pgid':{apiCompetition.pgid} 'mgname':{apiCompetition.mgname},'name':{apiCompetition.name}," +
                    $"'pgname':{apiCompetition.pgname} --> New women League Id: {womenLeague.Id}, name: {newWomenLeague} (w)");                
                _lineshouseContext.SaveChanges();

                var dgsCR_League_WomenLeague = _dgsContext.League.Find(Convert.ToUInt32(womenLeague.Id));
                if(dgsCR_League_WomenLeague == null) return;
                dgsCR_League_WomenLeague.Name = $"{newWomenLeague} (w)";
                _dgsContext.SaveChanges();
            }
        }

        private string CleanWomenLeagueName(Lineshouse.Models.League womenLeague)
        {
            var listOfWomenVariations = new List<string>
            {
                "(w)","women","women's","femenie","mulheres","femenina","féminin","feminine","WTA","femrave","feminino","femenino","feminina","feminin",
                "(W)","Women","Women's","Femenie","Mulheres","Femenina","Féminin","Feminine","WTA","Femrave","Feminino","Femenino","Feminina","Feminin"
            };
            return womenLeague.Name.Filter(listOfWomenVariations).Trim();
        }
    }
}