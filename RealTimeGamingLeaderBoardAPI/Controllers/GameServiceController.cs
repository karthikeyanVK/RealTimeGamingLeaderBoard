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
            //Move this to configuration file   
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(
           new ConfigurationOptions
           {
               EndPoints = { "gamescoretest.redis.cache.windows.net:6380" },
               Password = "zTXCbR9GZ11Rz0SbsjGGPhuQ8lLwQcFY3AzCaMDMLEo="
           });

            this.db = redis.GetDatabase();
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
            var rank = db.SortedSetRank("leaderboard", userId, Order.Descending);
            if (rank.HasValue)
            {
                int rankValue = (int)rank.Value;
                int start = rankValue > FetchAboveNBelow ? rankValue - FetchAboveNBelow : 0;
                int stop = rankValue + FetchAboveNBelow;
                var positionList = db.SortedSetRangeByRank("leaderboard", start, stop, Order.Descending);
                return positionList.ToList().Select((x, i) => new LeaderBoard { UserId = x.ToString(), Score = (db.SortedSetScore("leaderboard", x).HasValue ? (int)db.SortedSetScore("leaderboard", x) : 0) });
            }
            return null;

        }


        [HttpPost]
        [Route("~/SetLeaderBoardScore")]
        public bool SetLeaderBoardScore(string userId, int score)
        {
            if (db == null) return false;
            var result = this.db.SortedSetAdd("leaderboard", userId, score);
            return  result;                        

        }



    }
}
