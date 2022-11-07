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
    public class SetNewConfigForPlayerPropsOverUnder
    {
        private readonly LineshouseContext _lineshouseContext;
        private readonly DgsContext _dgsContext;

        public SetNewConfigForPlayerPropsOverUnder(LineshouseContext context, DgsContext dgsContext)
        {
            this._lineshouseContext = context;
            this._dgsContext = dgsContext;
        }

        public void Handle()
        {
            //this._dgsContext.Config.Load();
            //this._dgsContext.League.Load();
            var leaguesConfig = this._dgsContext.Config.Include(x => x.League);

            foreach (var config in leaguesConfig)
            {
                var data = JsonConvert.DeserializeObject<List<ConfigInfo>>(config.Data);
                Console.WriteLine($"{data}");

                var oldPropConfig = data.SingleOrDefault(x => x.Key == "0,2");
                if (oldPropConfig == null)
                {
                    continue;
                }
                data.Add(new ConfigInfo
                {
                    Key = "0,10",
                    League = oldPropConfig.League,
                    GameType = oldPropConfig.GameType,
                });
                var dgsConfig = this._dgsContext.Config.Find(config.Id);
                if (dgsConfig == null)
                {
                    continue;
                }

                dgsConfig.Data = JsonConvert.SerializeObject(data);
            }
            this._dgsContext.SaveChanges();
            //
        }
    }
}