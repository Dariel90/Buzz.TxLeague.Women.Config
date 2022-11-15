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
    public class FutsalConfigHandler
    {
        private readonly LineshouseContext _lineshouseContext;
        private readonly DgsContext _dgsContext;

        public FutsalConfigHandler(LineshouseContext context, DgsContext dgsContext)
        {
            this._lineshouseContext = context;
            this._dgsContext = dgsContext;
        }

        public void Handle()
        {
            //_dgsContext.Config.Load();
            //_dgsContext.League.Load();

            var leaguesConfig = this._dgsContext.Config.Include(x => x.League).Where(x => x.League.SportId == 16).ToList();
            foreach (var config in leaguesConfig)
            {
                var data = JsonConvert.DeserializeObject<List<ConfigInfo>>(config.Data);

                var oldPropConfig = data.FirstOrDefault(x => x.Key == "0");
                if (oldPropConfig == null)
                {
                    continue;
                }

                var dgsLeagueId = oldPropConfig.League.Value;
                var shortDgsLeagueId = Convert.ToInt16(dgsLeagueId);
                var dgsLeague = this._dgsContext.DLeague.FirstOrDefault(x => x.IdLeague == shortDgsLeagueId);
                Console.WriteLine($"{dgsLeague.Description}");
                if (dgsLeague == null)
                {
                    continue;
                }

                dgsLeague.IdSport = "SOC";
                this._dgsContext.DLeague.Update(dgsLeague);
                this._dgsContext.SaveChanges();
            }

            //
        }
    }
}