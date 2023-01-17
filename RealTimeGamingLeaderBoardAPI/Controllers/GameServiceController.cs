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
        IDatabase? db = null;
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
        [HttpGet]
        [Route("~/GetLeaderBoard/{topCount}")]
        public IEnumerable<LeaderBoard> GetLeaderBoard(int topCount)
        {
            if (db == null) return Enumerable.Empty<LeaderBoard>();
            return db.SortedSetRangeByRankWithScores("leaderboard", 0, topCount, Order.Descending).ToList().Select((x, i) => new LeaderBoard { UserId = x.Element.ToString(), Score = (int) x.Score });
        }
        [HttpGet]
        [Route("~/GetLeaderBoardPosition")]
        public int GetLeaderBoardPosition(string userId)

        {
            if (db != null)
            {
                var position = db.SortedSetRank("leaderboard", userId, Order.Descending);
                if (position.HasValue)
                    return (int)position.Value;
            }

            return -1;
        }
        [HttpGet]
        [Route("~/GetLeaderBoardRelativePosition")]
        public IEnumerable<LeaderBoard>? GetLeaderBoardRelativePosition(string userId, int FetchAboveNBelow)
            
         {
            if (db == null) return Enumerable.Empty<LeaderBoard>();
            var rank = db.SortedSetRank("leaderboard", userId);
            if (rank != null)
            {
                var positionList = db.SortedSetRangeByRank("leaderboard", rank.Value - 4, rank.Value + 4, Order.Descending);
                return positionList.ToList().Select((x, i) => new LeaderBoard { UserId = x.ToString(), Score = (int)db.SortedSetScore("leaderboard", x) });
            }
            return null;
        }
        
    }
}
