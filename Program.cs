using Buzz.TxLeague.Women.Config.Utils;
using Renci.SshNet;
using System;
using System.IO;
using System.Text;

namespace Buzz.TxLeague.Women.Config
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Recreate Games?? (Y/N)");
            var response = Console.ReadLine();
            if (response.ToUpper() == "Y")
            {
                Console.WriteLine("Put the events ids separated by a comma");
                response = Console.ReadLine();
                SyncEventsToDGS.RecreateGamesBulkTask(response);
            }
            else
            {
                Console.WriteLine("Select the option");
                Console.WriteLine("1 - Fix Tennis Events landing on Default Tennis League and sync with DGS database");
                Console.WriteLine("2 - Set new Key for the configuration of leagues For Player Propositions OverUnder");
                Console.WriteLine("3 - Set new League for PROPS in the config for non-major basketball and baseball leagues");
                Console.WriteLine("4 - Change the SportId from MU to SOC, for Futsal leagues in DGS");
                response = Console.ReadLine();
                switch (response.ToUpper())
                {
                    case "1":
                        var tennisTournamentDefaultLeagueHandler = new FixLeaguesForDefaultTennisLeague(new(), new(), new("xoxb-1244510899716-3316988675411-wJM0nHMGzd4ZczI8ZnuSrrCt"));
                        tennisTournamentDefaultLeagueHandler.Handle();
                        break;

                    case "2":
                        var handler = new SetNewConfigForPlayerPropsOverUnder(new Lineshouse.LineshouseContext(), new());
                        handler.Handle();
                        break;

                    case "3":
                        var basballAndBasketNonMajorPropsHandler = new BasballAndBasketNonMajorPropsHandler(new());
                        basballAndBasketNonMajorPropsHandler.Handle();
                        break;

                    case "4":
                        var fusalSportIdFromMUToSOCHandler = new FutsalConfigHandler(new(), new());
                        fusalSportIdFromMUToSOCHandler.Handle();
                        break;
                }
                //var womenLeagueCleanerHandler = new WomenLeagueCleanerHandler(new(),new());
                //womenLeagueCleanerHandler.Handle();

                //var euroBasketConfig = new EuroBasketConfigHandler(new(), new());
                //euroBasketConfig.Handle();

                //var womenLeagueCleanerHandler = new BasketConfigHandler(new(), new());
                //womenLeagueCleanerHandler.Handle();

                //var nHLConfigHanlder = new NHLConfigHanlder(new(), new());
                //nHLConfigHanlder.Handle();

                //var racingConfigHandler = new RacingConfigHandler(new(), new());
                //racingConfigHandler.Handle();

                //var handler = new LineshouseSportConfigHandler(new());
                //handler.Handle();

                //var handler = new CheckingLeaguesPropsConfigHandler(new Lineshouse.LineshouseContext(), new());
                //handler.Handle();

                //var handler = new TennisConfigHandler(new Lineshouse.LineshouseContext(), new());
                //handler.Handle();

                //var handler = new SoccerHalftimeConfigHandler(new Lineshouse.LineshouseContext(), new());
                //handler.Handle();
            }
        }
    }
}