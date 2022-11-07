using Buzz.TxLeague.Women.Config.Dgs;
using Buzz.TxLeague.Women.Config.Lineshouse;
using Buzz.TxLeague.Women.Config.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buzz.TxLeague.Women.Config
{
    public class FixLeaguesForDefaultTennisLeague
    {
        private readonly LineshouseContext _lineshouseContext;
        private readonly DgsContext dgsContext;

        public FixLeaguesForDefaultTennisLeague(LineshouseContext context, DgsContext dgsContext)
        {
            this._lineshouseContext = context;
            this.dgsContext = dgsContext;
        }

        public void Handle()
        {
            //this._dgsContext.Config.Load();
            //this._dgsContext.League.Load();
            using (var transaction = this._lineshouseContext.Database.BeginTransaction())
            {
                var query = $"SELECT `lineshouse-prod`.Events.Id as EvId,`lineshouse-prod`.BetradarEventMaps.TournamentId" +
                            " FROM `lineshouse-prod`.Fixtures " +
                            "JOIN `lineshouse-prod`.Events ON `lineshouse-prod`.Events.Id = `lineshouse-prod`.Fixtures.EventId " +
                            "JOIN `lineshouse-prod`.BetradarEventMaps ON `lineshouse-prod`.Events.Id = `lineshouse-prod`.BetradarEventMaps.ExtEventId where `lineshouse-prod`.Fixtures.IsMain and `lineshouse-prod`.Fixtures.Date > now() and `lineshouse-prod`.Events.LeagueId = 344584";
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

                var eventsToDelete = this.dgsContext.Events.Where(x => eventsToProcess.Contains((int)x.Id)).ToList();
                this.dgsContext.Events.RemoveRange(eventsToDelete);
                this.dgsContext.SaveChanges();

                if (eventsToProcess.Any())
                {
                    var events = string.Join(",", eventsToProcess.Select(e => e.ToString()).ToArray());
                    SyncEventsToDGS.RecreateGamesBulkTask(events);
                }
                WriteLog.WriteToFile($"================================> END <============================================== ");
            }
        }
    }
}