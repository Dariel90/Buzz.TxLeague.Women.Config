using System;

namespace Buzz.TxLeague.Women.Config
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var womenLeagueCleanerHandler = new WomenLeagueCleanerHandler(new(),new());
            womenLeagueCleanerHandler.Handle();
        }
    }


}
