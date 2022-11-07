using Buzz.TxLeague.Women.Config.Dgs;
using Buzz.TxLeague.Women.Config.Dgs.Models.DGS.Dto;
using Buzz.TxLeague.Women.Config.Lineshouse;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CR_DTO = Buzz.TxLeague.Women.Config.Dgs.Models;

namespace Buzz.TxLeague.Women.Config
{
    public class CheckingLeaguesPropsConfigHandler
    {
        private readonly LineshouseContext _lineshouseContext;
        private readonly DgsContext _dgsContext;
        private readonly List<(int sport, int league)?> unassignedProp = new List<(int sport, int league)?>();

        public CheckingLeaguesPropsConfigHandler(LineshouseContext context, DgsContext dgsContext)
        {
            this._lineshouseContext = context;
            this._dgsContext = dgsContext;

            this.unassignedProp.Add((4, 2404));
            this.unassignedProp.Add((2, 2402));
            this.unassignedProp.Add((5, 2405));
            this.unassignedProp.Add((1, 2401));
            this.unassignedProp.Add((3, 2403));
            this.unassignedProp.Add((6, 2406));
            this.unassignedProp.Add((7, 2407));
            this.unassignedProp.Add((9, 2409));
            this.unassignedProp.Add((10, 2410));
            this.unassignedProp.Add((11, 2411));
            this.unassignedProp.Add((12, 2412));
            this.unassignedProp.Add((13, 2413));
            this.unassignedProp.Add((14, 2414));
            this.unassignedProp.Add((16, 2416));
            this.unassignedProp.Add((18, 2418));
            this.unassignedProp.Add((19, 28374));
            this.unassignedProp.Add((22, 2421));
            this.unassignedProp.Add((26, 2425));
            this.unassignedProp.Add((27, 2427));
            this.unassignedProp.Add((29, 2429));
            this.unassignedProp.Add((30, 2430));
            this.unassignedProp.Add((31, 2431));
            this.unassignedProp.Add((32, 2432));
            this.unassignedProp.Add((33, 2433));
            this.unassignedProp.Add((39, 2437));
            this.unassignedProp.Add((45, 2443));
            this.unassignedProp.Add((47, 2445));
            this.unassignedProp.Add((48, 2446));
            this.unassignedProp.Add((49, 2447));
            this.unassignedProp.Add((50, 2448));
            this.unassignedProp.Add((51, 2449));
            this.unassignedProp.Add((52, 2450));
            this.unassignedProp.Add((53, 2451));
            this.unassignedProp.Add((54, 2452));
            this.unassignedProp.Add((56, 2454));
            this.unassignedProp.Add((70, 20579));
        }

        public void Handle()
        {
            //this._dgsContext.Config.Load();
            //var leaguesConfig = this._dgsContext.Config.Include(x => x.League).Where(Predicate()).ToList();
            var i = 0;
            var basket = new string[] { "NBA" };
            var leaguesConfig = this._dgsContext.Config.Include(x => x.League).Where(x => x.League.SportId == 4 && basket.Contains(x.League.Description)).ToList();
            foreach (var config in leaguesConfig)
            {
                try
                {
                    i++;
                    var data = JsonConvert.DeserializeObject<List<ConfigInfo>>(config.Data);

                    var propConfigInfo = data.FirstOrDefault(x => x.Key == "1");
                    if (propConfigInfo == null)
                    {
                        continue;
                    }
                    var leagueId = Convert.ToInt16(propConfigInfo.League);
                    var dgsLeagueInfo = this._dgsContext.DLeague.FirstOrDefault(x => x.IdLeague == leagueId);

                    if (dgsLeagueInfo == null)
                    { continue; }

                    if (propConfigInfo.GameType == 36)
                    {
                        var msg = $"The cr_league with id: {config.LeagueId} and name: {config.League.Name}, for PROPS config has a GameType: {propConfigInfo.GameType} in the key config. for the league:{dgsLeagueInfo.IdLeague}, idSport:{dgsLeagueInfo.IdSport}, desc.:{dgsLeagueInfo.Description} and shortDesc.:{dgsLeagueInfo.ShortDescription}";
                        this.WriteToFile(msg);

                        var leagueSport = Convert.ToUInt16(config.League.SportId);
                        data.Remove(propConfigInfo);
                        data.Add(new ConfigInfo
                        {
                            Key = propConfigInfo.Key,
                            League = propConfigInfo.League,
                            GameType = 17,
                        });
                        var dgsConfig = this._dgsContext.Config.Find(config.Id);
                        if (dgsConfig == null)
                        {
                            continue;
                        }
                        dgsConfig.Data = JsonConvert.SerializeObject(data);
                        this._dgsContext.SaveChanges();
                    }
                }
                catch (Exception)
                {
                    var msg = $"Config with id:{config.Id},idLeague:{config.LeagueId},league name:{config.League.Name},sportId:{config.League.SportId},Data:{config.Data.ToString()}";
                    this.WriteToFile(msg);
                }
            }
        }

        private static Func<CR_DTO::Config, bool> Predicate()
        {
            return x => (
                JsonConvert.DeserializeObject<List<ConfigInfo>>(x.Data).FirstOrDefault(y => y.Key == "1") != null);
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