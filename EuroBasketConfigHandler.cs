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
    public class EuroBasketConfigHandler
    {
        private readonly LineshouseContext _lineshouseContext;
        private readonly DgsContext _dgsContext;

        public EuroBasketConfigHandler(LineshouseContext context, DgsContext dgsContext)
        {
            this._lineshouseContext = context;
            this._dgsContext = dgsContext;
        }

        public void Handle()
        {
            //_dgsContext.Config.Load();
            //_dgsContext.League.Load();

            var leaguesConfig = this._dgsContext.Config.Include(x => x.League).Where(x => x.League.SportId == 4).Where(x => !(new string[] { "NBA", "WNBA", "CBB" }.Contains(x.League.Description))).ToList();

            foreach (var config in leaguesConfig)
            {
                var data = JsonConvert.DeserializeObject<List<ConfigInfo>>(config.Data);
                Console.WriteLine($"{data}");

                var oldPropConfig = data.FirstOrDefault(x => x.Key.Contains("0,0,0,20,1"));
                if (oldPropConfig == null)
                {
                    continue;
                }

                data.Remove(oldPropConfig);
                data.Add(new ConfigInfo
                {
                    Key = oldPropConfig.Key,
                    League = oldPropConfig.League,
                    GameType = 45,//APEX888 ONLY
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