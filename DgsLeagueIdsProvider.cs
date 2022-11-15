using Buzz.TxLeague.Women.Config.Dgs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buzz.TxLeague.Women.Config
{
    public class DgsLeagueIdsProvider
    {
        private DgsContext ctx;
        public Queue<short> Ids { get; private set; }

        public DgsLeagueIdsProvider(DgsContext ctx)
        {
            this.ctx = ctx;
            this.Ids = new Queue<short>();
        }

        public async Task Load()
        {
            List<short> list = await this.ctx.DLeague.Select(e => e.IdLeague).OrderBy(e => e).ToListAsync();
            int index = 0;
            for (short i = 1; i < short.MaxValue; i++)
            {
                if (index > list.Count() - 1)
                {
                    this.Ids.Enqueue(i);
                    continue;
                }
                if (list[index] != i)
                {
                    this.Ids.Enqueue(i);
                    continue;
                }
                index++;
            }
        }
    }
}