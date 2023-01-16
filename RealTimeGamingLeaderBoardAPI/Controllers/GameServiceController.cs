using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Linq;

namespace RealTimeGamingLeaderBoardAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameServiceController : ControllerBase
    {
        IDatabase db = null;
        public GameServiceController()
        {
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(
           new ConfigurationOptions
           {
               EndPoints = { "gamescoretest.redis.cache.windows.net:6380" },
               Password = "zTXCbR9GZ11Rz0SbsjGGPhuQ8lLwQcFY3AzCaMDMLEo="
           });

            this.db = redis.GetDatabase();

            //Random random = new Random();



            //Parallel.For(1, 1000000, i =>
            //{
            //    new ParallelOptions
            //    {
            //        MaxDegreeOfParallelism = 3
            //    };
            //    int score = random.Next(0, 1000);
            //    db.SortedSetAdd("leaderboard", "user" + i, score);
            //});
            //Console.WriteLine("Data added to Redis");

        }

        [HttpGet(Name = "GetLeaderBoard")]
        public IEnumerable<LeaderBoard> GetLeaderBoard(int topCount)
        {
            return db.SortedSetRangeByRankWithScores("leaderboard", 0, topCount, Order.Descending).ToList().Select((x, i) => new LeaderBoard { UserId = x.Element.ToString(), Score = (int) x.Score });
        }
        [HttpGet(Name = "GetLeaderBoardPosition")]
        public int GetLeaderBoardPosition(string userId)
        {
            return (int)db.SortedSetRank("leaderboard", userId).Value; //.ToList().Select((x, i) => new LeaderBoard { UserId = x.Element.ToString(), Score = (int)x.Score });
        }
        /* [HttpGet(Name = "GetLeaderBoardRelativePosition")]
         public IEnumerable<LeaderBoard> GetLeaderBoardRelativePosition(string userId, int FetchAboveNBelow)

         {

             return db.SortedSetRange ("leaderboard", 0, topCount, Order.Descending).ToList().Select((x, i) => new LeaderBoard { UserId = x.Element.ToString(), Score = (int)x.Score });
         }
        */

    }
}
