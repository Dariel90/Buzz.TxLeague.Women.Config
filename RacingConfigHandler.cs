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
    public class RacingConfigHandler
    {
        private readonly LineshouseContext _lineshouseContext;
        private readonly DgsContext _dgsContext;

        public RacingConfigHandler(LineshouseContext context, DgsContext dgsContext)
        {
            this._lineshouseContext = context;
            this._dgsContext = dgsContext;
        }

        public void Handle()
        {
            //_dgsContext.Config.Load();
            //_dgsContext.League.Load();

            var leaguesConfig = this._dgsContext.Config.Include(x => x.League).Where(x => x.League.SportId == 63).ToList();
            foreach (var config in leaguesConfig)
            {
                var data = JsonConvert.DeserializeObject<List<ConfigInfo>>(config.Data);
                Console.WriteLine($"{data}");

                var oldPropConfig = data.FirstOrDefault(x => x.Key == "1");
                if (oldPropConfig == null)
                {
                    continue;
                }

                data.Remove(oldPropConfig);
                data.Add(new ConfigInfo
                {
                    Key = oldPropConfig.Key,
                    League = oldPropConfig.League,
                    GameType = 76,
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
    }
}