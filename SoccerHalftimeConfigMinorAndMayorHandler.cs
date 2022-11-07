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
    public class SoccerHalftimeConfigMinorAndMayorHandler
    {
        private readonly LineshouseContext _lineshouseContext;
        private readonly DgsContext _dgsContext;

        public SoccerHalftimeConfigMinorAndMayorHandler(LineshouseContext context, DgsContext dgsContext)
        {
            this._lineshouseContext = context;
            this._dgsContext = dgsContext;
        }

        public void Handle()
        {
            this._dgsContext.Config.Load();
            //this._dgsContext.League.Load();

            var leaguesConfig = this._dgsContext.Config.Include(x => x.League).Where(x => x.League.SportId == 29).ToList();

            foreach (var config in leaguesConfig)
            {
                var data = JsonConvert.DeserializeObject<List<ConfigInfo>>(config.Data);
                Console.WriteLine($"{data}");

                var gameType = 35;
                var leagueGameType = data.FirstOrDefault(x => x.Key == "0")?.GameType.Value;
                switch (leagueGameType)
                {
                    case 55: //Soccer Minor
                        gameType = 57; //Soccer Minor halves
                        break;

                    default:
                        gameType = 35;
                        break;
                }

                data = this.DeleteKeysInConflict(data);
                var oldPropConfig = data.SingleOrDefault(x => x.Key == "0,0,0,21");
                if (oldPropConfig == null)
                {
                    data.Add(new ConfigInfo
                    {
                        Key = "0,0,0,21",
                        League = null,
                        GameType = 35,
                    });
                }
                else
                {
                    data.Remove(oldPropConfig);
                    data.Add(new ConfigInfo
                    {
                        Key = oldPropConfig.Key,
                        League = oldPropConfig.League,
                        GameType = gameType,
                    });
                }
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

        private List<ConfigInfo> DeleteKeysInConflict(List<ConfigInfo> data)
        {
            var keyMinor = data.SingleOrDefault(x => x.Key == "0,0,0,21,0");
            var keyMajor = data.SingleOrDefault(y => y.Key == "0,0,0,21,1");
            if (keyMinor is not null)
            {
                data.Remove(keyMinor);
            }
            if (keyMajor != null)
            {
                data.Remove(keyMajor);
            }
            return data;
        }
    }
}