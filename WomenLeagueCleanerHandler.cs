using Buzz.TxLeague.Women.Config.Dgs;
using Buzz.TxLeague.Women.Config.Lineshouse;
using Buzz.TxLeague.Women.Config.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TxLeagueTool.Api;

namespace Buzz.TxLeague.Women.Config
{
    public class WomenLeagueCleanerHandler
    {
        private readonly LineshouseContext _lineshouseContext;
        private readonly DgsContext _dgsContext;

        private readonly Dictionary<int, (string sportName, int lsSportId)> _sports = new()
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
            this._lineshouseContext = context;
            this._dgsContext = dgsContext;
        }

        public void Handle()
        {
            this._lineshouseContext.TxLeagueMaps.Load();
            this._lineshouseContext.Leagues.Where(m => m.SportId != 12).ToList();

            var a = this._lineshouseContext.TxLeagueMaps.Local;
            var b = this._lineshouseContext.Leagues.Local;

            foreach (var sport in this._sports)
            {
                Console.WriteLine($"{sport.Value.sportName} starts....");

                var competitions = CompetitionProvider.GetCompetitions(sport.Key).Result.TxLeague;
                var womenCompetitions = this.GetWomenCompetitions(competitions.ToList()).ToList();
                foreach (var apiWomenCompetition in womenCompetitions)
                {
                    this.ProcessTxWomenCompetition(apiWomenCompetition);
                }
            }

            this._lineshouseContext.SaveChanges();
        }

        private IEnumerable<TxCompetition> GetWomenCompetitions(IEnumerable<TxCompetition> txCompetitions)
        {
            return txCompetitions.Where(x => x.mgname.StartsWith("W")
                        || this.StringContainsWomen(x.name, "(w)") || this.StringContainsWomen(x.name, "women")
                        || this.StringContainsWomen(x.name, "femenie") || this.StringContainsWomen(x.name, "mulheres")
                        || this.StringContainsWomen(x.name, "femenina") || this.StringContainsWomen(x.name, "féminin")
                        || this.StringContainsWomen(x.name, "feminine") || this.StringContainsWomen(x.name, "WTA")
                        || this.StringContainsWomen(x.name, "femrave") || this.StringContainsWomen(x.name, "feminino")
                        || this.StringContainsWomen(x.name, "femenino") || this.StringContainsWomen(x.name, "feminina")
                        || this.StringContainsWomen(x.name, "feminin") || this.StringContainsWomen(x.name, "femminile"));
        }

        private bool StringContainsWomen(string name, string match)
        {
            return name.ToLower().Contains(match);
        }

        private void ProcessTxWomenCompetition(TxCompetition apiCompetition)
        {
            Console.WriteLine($"Women Competition: {apiCompetition.pgname}");

            var txWomenCompetition = this._lineshouseContext.TxLeagueMaps.Find(apiCompetition.pgid);

            if (txWomenCompetition != null)
            {
                var womenLeague = this._lineshouseContext.Leagues.Find(txWomenCompetition.LeagueId);
                if (womenLeague == null)
                {
                    return;
                }

                if (womenLeague.Name.ToLower().Contains("(w)"))
                {
                    return;
                }

                var newWomenLeague = this.CleanWomenLeagueName(womenLeague);
                womenLeague.Name = $"{newWomenLeague} (w)";
                Console.WriteLine($"Competition 'pgid':{apiCompetition.pgid} 'mgname':{apiCompetition.mgname},'name':{apiCompetition.name}," +
                    $"'pgname':{apiCompetition.pgname} --> New women League Id: {womenLeague.Id}, name: {newWomenLeague} (w)");
                this._lineshouseContext.SaveChanges();

                var dgsCR_League_WomenLeague = this._dgsContext.League.Find(Convert.ToUInt32(womenLeague.Id));
                if (dgsCR_League_WomenLeague == null)
                {
                    return;
                }

                dgsCR_League_WomenLeague.Name = $"{newWomenLeague} (w)";
                this._dgsContext.SaveChanges();
            }
        }

        private string CleanWomenLeagueName(Lineshouse.Models.League womenLeague)
        {
            var listOfWomenVariations = new List<string>
            {
                "(w)","women's","women","femenie","mulheres","femenina","féminin","feminine","WTA","femrave","feminino","femenino","feminina","feminin",
                "(W)","Women's","Women","Femenie","Mulheres","Femenina","Féminin","Feminine","WTA","Femrave","Feminino","Femenino","Feminina","Feminin"
            };
            return womenLeague.Name.Filter(listOfWomenVariations).Trim();
        }
    }
}