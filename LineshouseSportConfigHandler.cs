using Buzz.TxLeague.Women.Config.Lineshouse;
using Buzz.TxLeague.Women.Config.Lineshouse.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Buzz.TxLeague.Women.Config
{
    public class LineshouseSportConfigHandler
    {
        private readonly LineshouseContext _lineshouseContext;

        private readonly List<AuxClass> _sportsConfig = new List<AuxClass>();
        //new Tuple("Cycling Cycle Ball", (short)150), new Tuple("Cycling Cycle Ball", (short)152),
        //new Tuple("Cycling Cycle Ball",(short)161), new Tuple("Cycling Cycle Ball",(short)184), (short)190) })
        //new((short)204, new[]{ (short)187, (short)188 }),
        //new((short)12, new[]{(short)80,(short)102,(short)103,(short)104,(short)107,(short)109,(short)111 ,(short)123,(short)124,(short)125,(short)127,(short)129,(short)130
        //                ,(short)131 ,(short)132,(short)133,(short)135,(short)137,(short)138,(short)139,(short)141,(short)142,(short)144,(short)145
        //                ,(short)146,(short)147,(short)149,(short)164,(short)165,(short)166,(short)167,(short)168,(short)169}),
        //new ((short)63, new[]{ (short)118, (short)159, (short)119, (short)116, (short)78, (short)163 }),
        //new ((short)206,new[]{ (short)20, (short)209 })

        public LineshouseSportConfigHandler(LineshouseContext context)
        {
            this._lineshouseContext = context;
            //this._sportsConfig.Add(new AuxClass
            //{
            //    mainSportId = 45,
            //    childSportList = new List<(string sportName, short childSportId)>
            //        {
            //            new ("Cycling Cycle Ball",112),
            //            new ("Cycling BMX Freestyle",150),
            //            new ("Cycling BMX Racing",152),
            //            new ("Motorcycle Racing",161),
            //            new ("Track cycling",184),
            //            new ("BMX racing",190),
            //            new ("Mountain Bike",193),
            //        }
            //});
            //this._sportsConfig.Add(new AuxClass
            //{
            //    mainSportId = 204,
            //    childSportList = new List<(string sportName, short childSportId)>
            //        {
            //            new("Trampoline Gymnastics",187),
            //            new("Rhythmic gymnastics",188)
            //        }
            //});
            this._sportsConfig.Add(new AuxClass
            {
                mainSportId = 63,
                childSportList = new List<(string sportName, short childSportId)>
                    {
                        //new("Drag Racing",118),
                        //new("Touring Car Racing",159),
                        //new("Stock Car Racing",119),
                        //new("Sprint Car Racing",116),
                        //new("Short Track",78),
                        new("Rally", 74),
                        new("Indy Racing", 99),
                        new("Formula E", 113),
                    }
            });
            //this._sportsConfig.Add(new AuxClass
            //{
            //    mainSportId = 206,
            //    childSportList = new List<(string sportName, short childSportId)>
            //        {
            //            new("Horse Racing",20),
            //            new("Horseball",209),
            //        }
            //});
            //this._sportsConfig.Add(new AuxClass
            //{
            //    mainSportId = 228,//Cards
            //    childSportList = new List<(string sportName, short childSportId)>
            //        {
            //            new("Poker",62),
            //        }
            //});
            this._sportsConfig.Add(new AuxClass
            {
                mainSportId = 12,
                childSportList = new List<(string sportName, short childSportId)>
                {
                    //new("Gears of War",20),
                    //new("Clash Royale",103),
                    //new("King of Glory",104),
                    //new("eSoccer",107),
                    //new("Quake",109),
                    //new("PlayerUnknowns Battlegrounds",111),
                    //new("World of Warcraft",123),
                    //new("eBasketball",124),
                    //new("Dragon Ball FighterZ",125),
                    //new("Tekken",127),
                    //new("Arena of Valor",129),
                    //new("TF2",130),
                    //new("SSBM",131),
                    //new("Paladins",132),
                    //new("Artifact",133),
                    //new("Apex Legends",135),
                    //new("Pro Evolution Soccer",137),
                    //new("Madden NFL",138),
                    //new("Brawl Stars",139),
                    //new("Fortnite",141),
                    //new("MTG",142),
                    //new("Dota Underlords",144),
                    //new("Teamfight Tactics",145),
                    //new("Auto Chess",146),
                    //new("Fighting Games",147),
                    //new("Motorsport",149),
                    //new("Valorant",164),
                    //new("eIce Hockey",165),
                    //new("eTennis",166),
                    //new("eCricket",167),
                    //new("eVolleyball",168),
                    //new("Wild Rift",169),
                    //new("StarCraft",95),
                    //new("Rainbow Six",83),
                    new("League of Legends", 82),
                }
            });
            //this._sportsConfig.Add(new AuxClass
            //{
            //    mainSportId = 4,
            //    childSportList = new List<(string sportName, short childSportId)>
            //        {
            //            new("Basketball 3x3",126),
            //            new("Finnish Baseball",181),
            //        }
            //});
        }

        public void Handle()
        {
            //var childSportNames = new List<(string sportName, short childSportId)>();
            //foreach (var item in _sportsConfig)
            //{
            //    var sport = item.childSportList.FirstOrDefault(x => _lineshouseContext.Sports.FirstOrDefault(y => x.childSportId == y.Id) != null);
            //    childSportNames.Add(sport);
            //}
            var leaguesWithIssues = new List<League>();
            foreach (var item in this._sportsConfig)
            {
                foreach (var item2 in item.childSportList)
                {
                    var childSportId = Convert.ToUInt16(item2.childSportId);

                    var legues = this._lineshouseContext.Leagues.Include(x => x.Sport).Where(x => x.Sport.Id == childSportId);
                    if (legues != null)
                    {
                        leaguesWithIssues.AddRange(legues);
                    }
                }
            }
            foreach (var league in leaguesWithIssues)
            {
                var mainSportId = this.GetMainSportIdByChildSportId(league.SportId);
                var childSportId = league.SportId;
                if (mainSportId is 0)
                {
                    continue;
                }
                var mainSportConfig = this._sportsConfig.FirstOrDefault(x => x.mainSportId == mainSportId);
                var sportName = mainSportConfig.childSportList.FirstOrDefault(x => x.childSportId == league.SportId).sportName;
                //var sportName = childSportNames.FirstOrDefault(n => n.childSportId == league.SportId);
                league.Name = $"{sportName} - {league.Name}";
                league.SportId = mainSportId;
                this._lineshouseContext.SaveChanges();
            }
        }

        private short GetMainSportIdByChildSportId(int id)
        {
            var result = this._sportsConfig.FirstOrDefault(x => x.childSportList.Any(y => y.childSportId == id))?.mainSportId ?? Convert.ToInt16(0);
            return result;
        }
    }

    public class AuxClass
    {
        public short mainSportId { get; set; }
        public List<(string sportName, short childSportId)> childSportList { get; set; }
    }
}