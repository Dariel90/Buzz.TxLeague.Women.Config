using Buzz.TxLeague.Women.Config.Dgs;
using Buzz.TxLeague.Women.Config.Dgs.Models.DGS.Dto;
using Buzz.TxLeague.Women.Config.Lineshouse;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Buzz.TxLeague.Women.Config
{
    public class TennisConfigHandler
    {
        private readonly LineshouseContext _lineshouseContext;
        private readonly DgsContext _dgsContext;

        public TennisConfigHandler(LineshouseContext context, DgsContext dgsContext)
        {
            this._lineshouseContext = context;
            this._dgsContext = dgsContext;
        }

        public void Handle()
        {
            //_dgsContext.Config.Load();
            //_dgsContext.League.Load();

            var leaguesConfig = this._dgsContext.Config.Include(x => x.League).Where(x => x.League.SportId == 33).
                Where(x => x.League.Name.Contains("WTA") || x.League.Name.Contains("ATP") || x.League.Name.Contains("ITF")).ToList();

            foreach (var config in leaguesConfig)
            {
                var data = JsonConvert.DeserializeObject<List<ConfigInfo>>(config.Data);
                Console.WriteLine($"{data}");

                var oldPropConfig = data.FirstOrDefault(x => x.Key == "1");
                if (oldPropConfig == null)
                {
                    continue;
                }
                var leagueId = config.League.Name.Contains("WTA", StringComparison.InvariantCultureIgnoreCase) ? 28539 : config.League.Name.Contains("ATP", StringComparison.InvariantCultureIgnoreCase) ? 28537 : config.League.Name.Contains("ITF", StringComparison.InvariantCultureIgnoreCase) && this.GetWomenCompetitions(config.League.Name) ? 28560 : config.League.Name.Contains("ITF", StringComparison.InvariantCultureIgnoreCase) && !this.GetWomenCompetitions(config.League.Name) ? 28558 : oldPropConfig.League;
                data.Remove(oldPropConfig);
                data.Add(new ConfigInfo
                {
                    Key = oldPropConfig.Key,
                    League = leagueId,
                    GameType = oldPropConfig.GameType,
                });
                var dgsConfig = this._dgsContext.Config.Find(config.Id);
                if (dgsConfig == null)
                {
                    continue;
                }

                dgsConfig.Data = JsonConvert.SerializeObject(data);
                this._dgsContext.SaveChanges();
            }

            //
        }

        private bool GetWomenCompetitions(string leagueName)
        {
            return this.StringContainsWomen(leagueName, "(w)") || this.StringContainsWomen(leagueName, "women") || this.StringContainsWomen(leagueName, "Women")
                        || this.StringContainsWomen(leagueName, "femenie") || this.StringContainsWomen(leagueName, "mulheres")
                        || this.StringContainsWomen(leagueName, "femenina") || this.StringContainsWomen(leagueName, "féminin")
                        || this.StringContainsWomen(leagueName, "feminine")
                        || this.StringContainsWomen(leagueName, "femrave") || this.StringContainsWomen(leagueName, "feminino")
                        || this.StringContainsWomen(leagueName, "femenino") || this.StringContainsWomen(leagueName, "feminina")
                        || this.StringContainsWomen(leagueName, "feminin") || this.StringContainsWomen(leagueName, "femminile");
        }

        private bool StringContainsWomen(string name, string match)
        {
            return name.ToLower().Contains(match, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}