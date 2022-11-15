using Buzz.TxLeague.Women.Config.Dgs;
using Buzz.TxLeague.Women.Config.Lineshouse;
using Buzz.TxLeague.Women.Config.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlackAPI;
using System.Net.Http;
using System.Xml.Serialization;
using DGS_DTO = Buzz.TxLeague.Women.Config.Dgs.Models.DGS;

namespace Buzz.TxLeague.Women.Config
{
    public class FixLeaguesForDefaultTennisLeague
    {
        private readonly LineshouseContext _lineshouseContext;
        private readonly DgsContext dgsContext;
        private readonly SlackTaskClient slackClient;
        private readonly HttpClient httpClient;
        private const string SegmentUrl = "https://api.betradar.com/v1/sports/en/tournaments/sr:tournament:{0}/info.xml";
        private DgsLeagueIdsProvider idProvider;
        private int selectedId = 0;

        private readonly string iftWomen = "[{\"Key\":\"{@@@}\",\"League\":29517,\"GameType\":10},{\"Key\":\"0,0,0,0,1\",\"League\":null,\"GameType\":11},{\"Key\":\"0,0,0,50\",\"League\":null,\"GameType\":65},{\"Key\":\"0,0,0,51\",\"League\":null,\"GameType\":65},{\"Key\":\"0,0,0,52\",\"League\":null,\"GameType\":65},{\"Key\":\"0,0,0,53\",\"League\":null,\"GameType\":65},{\"Key\":\"0,0,0,54\",\"League\":null,\"GameType\":65},{\"Key\":\"0,0,1\",\"League\":2758,\"GameType\":24},{\"Key\":\"0,1\",\"League\":null,\"GameType\":78},{\"Key\":\"0,2\",\"League\":null,\"GameType\":10},{\"Key\":\"0,5\",\"League\":null,\"GameType\":77},{\"Key\":\"1\",\"League\":28560,\"GameType\":10},{\"Key\":\"0,10\",\"League\":null,\"GameType\":10}]";
        private readonly string iftMen = "[{\"Key\":\"{@@@}\",\"League\":29519,\"GameType\":10},{\"Key\":\"0,0,0,0,1\",\"League\":null,\"GameType\":11},{\"Key\":\"0,0,0,50\",\"League\":null,\"GameType\":65},{\"Key\":\"0,0,0,51\",\"League\":null,\"GameType\":65},{\"Key\":\"0,0,0,52\",\"League\":null,\"GameType\":65},{\"Key\":\"0,0,0,53\",\"League\":null,\"GameType\":65},{\"Key\":\"0,0,0,54\",\"League\":null,\"GameType\":65},{\"Key\":\"0,0,1\",\"League\":2758,\"GameType\":24},{\"Key\":\"0,1\",\"League\":null,\"GameType\":78},{\"Key\":\"0,2\",\"League\":null,\"GameType\":10},{\"Key\":\"0,5\",\"League\":null,\"GameType\":77},{\"Key\":\"1\",\"League\":28558,\"GameType\":10},{\"Key\":\"0,10\",\"League\":null,\"GameType\":10}]";
        private readonly string wtaWomen = "[{\"Key\":\"{@@@}\",\"League\":29521,\"GameType\":10},{\"Key\":\"0,0,0,0,1\",\"League\":null,\"GameType\":11},{\"Key\":\"0,0,0,50\",\"League\":null,\"GameType\":65},{\"Key\":\"0,0,0,51\",\"League\":null,\"GameType\":65},{\"Key\":\"0,0,0,52\",\"League\":null,\"GameType\":65},{\"Key\":\"0,0,0,53\",\"League\":null,\"GameType\":65},{\"Key\":\"0,0,0,54\",\"League\":null,\"GameType\":65},{\"Key\":\"0,1\",\"League\":null,\"GameType\":78},{\"Key\":\"0,2\",\"League\":null,\"GameType\":10},{\"Key\":\"0,5\",\"League\":null,\"GameType\":77},{\"Key\":\"1\",\"League\":28539,\"GameType\":10},{\"Key\":\"0,10\",\"League\":null,\"GameType\":10}]";
        private readonly string atpMen = "[{\"Key\":\"{@@@}\",\"League\":28710,\"GameType\":10},{\"Key\":\"0,0,0,0,1\",\"League\":null,\"GameType\":11},{\"Key\":\"0,0,0,50\",\"League\":null,\"GameType\":65},{\"Key\":\"0,0,0,51\",\"League\":null,\"GameType\":65},{\"Key\":\"0,0,0,52\",\"League\":null,\"GameType\":65},{\"Key\":\"0,0,0,53\",\"League\":null,\"GameType\":65},{\"Key\":\"0,0,0,54\",\"League\":null,\"GameType\":65},{\"Key\":\"0,0,1\",\"League\":2758,\"GameType\":24},{\"Key\":\"0,1\",\"League\":null,\"GameType\":78},{\"Key\":\"0,2\",\"League\":null,\"GameType\":10},{\"Key\":\"0,5\",\"League\":null,\"GameType\":77},{\"Key\":\"1\",\"League\":28537,\"GameType\":10},{\"Key\":\"0,10\",\"League\":null,\"GameType\":10}]";

        public FixLeaguesForDefaultTennisLeague(LineshouseContext context, DgsContext dgsContext, SlackTaskClient slackClient)
        {
            this._lineshouseContext = context;
            this.dgsContext = dgsContext;
            this.slackClient = slackClient;
            this.httpClient = new HttpClient();
            this.idProvider = new DgsLeagueIdsProvider(dgsContext);
            this.idProvider.Load().Wait();
        }

        public async Task Handle()
        {
            //this._dgsContext.Config.Load();
            //this._dgsContext.League.Load();
            using (var transaction = this._lineshouseContext.Database.BeginTransaction())
            {
                var query = $"SELECT `lineshouse-prod`.Events.Id as EvId,`lineshouse-prod`.BetradarEventMaps.TournamentId" +
                            " FROM `lineshouse-prod`.Fixtures " +
                            "JOIN `lineshouse-prod`.Events ON `lineshouse-prod`.Events.Id = `lineshouse-prod`.Fixtures.EventId " +
                            "JOIN `lineshouse-prod`.BetradarEventMaps ON `lineshouse-prod`.Events.Id = `lineshouse-prod`.BetradarEventMaps.ExtEventId where `lineshouse-prod`.Fixtures.IsMain and `lineshouse-prod`.Fixtures.Date > now() and `lineshouse-prod`.Events.LeagueId = 344584 order by `lineshouse-prod`.BetradarEventMaps.TournamentId";
                var tournamentsInDefaultTennisLeague = this._lineshouseContext.TournamentEvents.FromSqlRaw(query).ToListAsync().Result;
                var eventsToProcess = new List<int>();
                var landingDefaultLeaguesEvents = new List<(int eventId, string tournamentId)>();
                if (!tournamentsInDefaultTennisLeague.Any())
                {
                    Console.WriteLine("No tennis leagues landing on the Default League");
                    return;
                }
                WriteLog.WriteToFile($"===========================> {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss tt")} <=========================== ");
                WriteLog.WriteToFile($"================================> BEGIN <============================================== ");

                foreach (var tournament in tournamentsInDefaultTennisLeague)
                {
                    var league = this._lineshouseContext.BetradarEventMaps.Include(x => x.ExtEvent).ThenInclude(x => x.League).FirstOrDefault(x => x.ExtEvent.League.Id != 344584 && x.TournamentId == tournament.TournamentId)?.ExtEvent.League;
                    if (league == null)
                    {
                        WriteLog.WriteToFile($"Tournament with Id: {tournament.TournamentId} and Event: {tournament.EvId} is landing in a Default League (Missing Reference League) ");
                        landingDefaultLeaguesEvents.Add(new(tournament.EvId, tournament.TournamentId));
                        //OpenConsoleForConfigure
                    }
                    else
                    {
                        query = $"UPDATE `lineshouse-prod`.Events SET LeagueId = {league.Id} WHERE Id = {tournament.EvId}";
                        this._lineshouseContext.Database.ExecuteSqlRaw(query);
                        eventsToProcess.Add(tournament.EvId);
                    }
                }
                this._lineshouseContext.SaveChanges();
                transaction.Commit();

                if (eventsToProcess.Any())
                {
                    var eventsToDelete = this.dgsContext.Events.Where(x => eventsToProcess.Contains((int)x.Id)).ToList();
                    this.dgsContext.Events.RemoveRange(eventsToDelete);
                    this.dgsContext.SaveChanges();
                    var events = string.Join(",", eventsToProcess.Select(e => e.ToString()).ToArray());
                    Console.WriteLine(events);
                    SyncEventsToDGS.RecreateGamesBulkTask(events);
                }
                WriteLog.WriteToFile($"================================> END <============================================== ");
            }
        }

        //private async Task OpenConsoleForConfigureAsync(TournamentEvent tournament)
        //{
        //    var tournamentName = await this.GetTournamentName(Int32.Parse(tournament.TournamentId.Split(":")[2]));
        //    Console.WriteLine("Missing League: " + tournamentName);
        //    Console.WriteLine("Do you want to create a new League? Y/N");
        //    var response = Console.ReadLine().ToUpper();
        //    if (response == "Y")
        //    {
        //        Console.WriteLine("Enter the League name do you want to create (Lineshouse)");
        //        var leagueNameLH = Console.ReadLine();
        //        Console.WriteLine("Enter the League name do you want to create (DGS)");
        //        var leagueNameDGS = Console.ReadLine();
        //        var leagueId = this.CreateMappingForTennis(leagueNameLH, leagueNameDGS);
        //        var query = $"UPDATE `lineshouse-prod`.Events SET LeagueId = {leagueId} WHERE Id = {tournament.EvId}";
        //        this._lineshouseContext.Database.ExecuteSqlRaw(query);
        //        eventsToProcess.Add(tournament.EvId);
        //    }
        //    else
        //    {
        //        if (this.selectedId != 0)
        //        {
        //            Console.WriteLine("Do you want to use a previous selected league ID");
        //            response = Console.ReadLine().ToUpper();
        //            if (response == "Y")
        //            {
        //                var query = $"UPDATE `lineshouse-prod`.Events SET LeagueId = {this.selectedId} WHERE Id = {tournament.EvId}";
        //                this._lineshouseContext.Database.ExecuteSqlRaw(query);
        //                eventsToProcess.Add(tournament.EvId);
        //            }
        //            else
        //            {
        //                this.selectedId = 0;
        //            }
        //        }
        //        Console.WriteLine("Do you want to search in the existing ones? Y/N");
        //        response = Console.ReadLine().ToUpper();
        //        if (response == "Y")
        //        {
        //            Console.WriteLine("Type the name of the league do you want to search");
        //            var leagueNameLH = Console.ReadLine();
        //            var leagues = this._lineshouseContext.Leagues.Where(x => x.Name.Contains(leagueNameLH));
        //            foreach (var item in leagues)
        //            {
        //                Console.WriteLine($"Id:{ item.Id }, Name: {item.Name}");
        //            }
        //            Console.WriteLine("Select the Id for the league");
        //            this.selectedId = Int32.Parse(Console.ReadLine());
        //            Console.WriteLine("Do you want update the event with the league Id selected? Y/N");
        //            response = Console.ReadLine();
        //            if (response == "Y")
        //            {
        //                var query = $"UPDATE `lineshouse-prod`.Events SET LeagueId = {this.selectedId} WHERE Id = {tournament.EvId}";
        //                this._lineshouseContext.Database.ExecuteSqlRaw(query);
        //                eventsToProcess.Add(tournament.EvId);
        //            }
        //            else
        //            {
        //                Console.WriteLine("Do you want to create a new League? Y/N");
        //                response = Console.ReadLine().ToUpper();
        //                if (response == "Y")
        //                {
        //                    Console.WriteLine("Enter the League name do you want to create (Lineshouse)");
        //                    leagueNameLH = Console.ReadLine();
        //                    Console.WriteLine("Enter the League name do you want to create (DGS)");
        //                    var leagueNameDGS = Console.ReadLine();
        //                    var leagueId = this.CreateMappingForTennis(leagueNameLH, leagueNameDGS);
        //                    var query = $"UPDATE `lineshouse-prod`.Events SET LeagueId = {leagueId} WHERE Id = {tournament.EvId}";
        //                    this._lineshouseContext.Database.ExecuteSqlRaw(query);
        //                    eventsToProcess.Add(tournament.EvId);
        //                }
        //                else
        //                {
        //                    continue;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            continue;
        //        }
        //    }
        //}

        private int CreateMappingForTennis(string leagueNameLH, string leagueNameDGS)
        {
            var leagueIdFromLh = 0;

            var leagueLh = new Buzz.TxLeague.Women.Config.Lineshouse.Models.League
            {
                IsDefault = false,
                Name = leagueNameLH,
                SportId = 33,
            };
            this._lineshouseContext.Leagues.Add(leagueLh);
            this._lineshouseContext.SaveChanges();
            leagueIdFromLh = leagueLh.Id;

            short id = this.idProvider.Ids.Dequeue();
            var dgsleague = new DGS_DTO::League
            {
                Description = leagueNameDGS,
                IdLeague = id,
                IdLeagueRegion = 1,
                IdSport = "NHL",
                LastModification = DateTime.Now,
                LastModificationUser = 93,
                LeagueOrder = 125,
                ShortDescription = leagueNameLH.ToLower().Contains("(w)") || leagueNameLH.ToLower().Contains("women") ? "TENWO" : "TENME",
                TeamFkrequired = false
            };

            this.dgsContext.DLeague.Add(dgsleague);

            var dgsCRLeague = new Dgs.Models.League
            {
                Name = leagueNameLH,
                Description = null,
                SportId = 33
            };
            this.dgsContext.League.Add(dgsCRLeague);
            this.dgsContext.SaveChanges();
            this.dgsContext.Config.Add(new Dgs.Models.Config
            {
                LeagueId = dgsCRLeague.Id,
                ProfileId = 21,
                Data = this.GetDataForTennis(leagueNameLH, id)
            });
            using (var transaction = this.dgsContext.Database.BeginTransaction())
            {
                this.dgsContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [DGSData].[dbo].[LEAGUE] ON");
                this.dgsContext.SaveChanges();
                this.dgsContext.Database.ExecuteSqlRaw("SET IDENTITY_INSERT [DGSData].[dbo].[LEAGUE] OFF");
                transaction.Commit();
            }

            return leagueIdFromLh;
        }

        private string GetDataForTennis(string leagueNameLH, short id)
        {
            var result = string.Empty;
            if (leagueNameLH.Contains("ITF") && leagueNameLH.Contains("(w)"))
            { result = this.iftWomen.Replace("{@@@}", id.ToString()); }
            else
            if (leagueNameLH.Contains("ITF") && !leagueNameLH.Contains("(w)"))
            { result = this.iftMen.Replace("{@@@}", id.ToString()); }
            else
            if (leagueNameLH.Contains("WTA") && leagueNameLH.Contains("(w)"))
            { result = this.wtaWomen.Replace("{@@@}", id.ToString()); }
            else
            if (leagueNameLH.Contains("ATP") && !leagueNameLH.Contains("(w)"))
            { result = this.atpMen.Replace("{@@@}", id.ToString()); }

            return result;
        }

        private async Task<string> GetTournamentName(int touranmentId)
        {
            TournamentInfo result = null;

            using (HttpClient client = new HttpClient())
            {
                var url = string.Format(SegmentUrl, touranmentId);
                client.DefaultRequestHeaders.Add("Accept", "application/*+xml;version=5.1");
                client.DefaultRequestHeaders.Add("x-access-token", "8Mae0Qx32xHTjdx3tT");
                var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    XmlSerializer xs = new XmlSerializer(typeof(TournamentInfo));
                    result = (TournamentInfo)xs.Deserialize(await response.Content.ReadAsStreamAsync());
                }
            }
            return result.Tournament.Name;
        }

        private async Task SendTournamentsForMapToSlack(string tournamentId, int evId)
        {
            string message =
                $@"Tournament:
                    Id={tournamentId}
                    Event={evId}";

            _ = await this.slackClient.PostMessageAsync("#betradar-receiver-notifications", message);
        }
    }
}