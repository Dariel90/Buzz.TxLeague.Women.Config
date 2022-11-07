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
    public class BasballAndBasketNonMajorPropsHandler
    {
        private readonly LineshouseContext _lineshouseContext;
        private readonly DgsContext _dgsContext;

        public BasballAndBasketNonMajorPropsHandler(LineshouseContext context, DgsContext dgsContext)
        {
            this._lineshouseContext = context;
            this._dgsContext = dgsContext;
        }

        public void Handle()
        {
            _dgsContext.Config.Load();
            _dgsContext.League.Load();

            var leaguesConfig = this._dgsContext.Config.Include(x => x.League).Where(x => x.League.SportId == 4 || x.League = 3).Where(x => !(new string[] { "NBA", "WNBA", "MLB" }.Contains(x.League.Description))).ToList();

            foreach (var config in leaguesConfig)
            {
                var data = JsonConvert.DeserializeObject<List<ConfigInfo>>(config.Data);
                Console.WriteLine($"{data}");
                //206	PROP 	1	BASEBALL - SEASONS
                //262	PROP 	1	BASKETBALL - UNASSIGNED -
                var oldPropConfig = data.FirstOrDefault(x => x.Key == "1");
                if (oldPropConfig == null)
                {
                    continue;
                }

                data.Remove(oldPropConfig);
                data.Add(new ConfigInfo
                {
                    Key = oldPropConfig.Key,
                    League = config.League.SportId == 3 ? 206 : 262,
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