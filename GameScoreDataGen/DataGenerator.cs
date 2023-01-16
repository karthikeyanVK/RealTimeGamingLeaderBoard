using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameScoreDataGen
{
    internal class DataGenerator
    {
        IDatabase? db = null;
        public DataGenerator()
        {
            
            using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(
            new ConfigurationOptions
            {
                EndPoints = { "gamescoretest.redis.cache.windows.net:6380" },
                Password = "zTXCbR9GZ11Rz0SbsjGGPhuQ8lLwQcFY3AzCaMDMLEo="
            }))
            {
                this.db = redis.GetDatabase();
            }
        }


        public void GenerateData()
        {
            Random random = new Random();
            
            
            Parallel.For(1, 1000000, i =>
            {
                new ParallelOptions
                {
                    MaxDegreeOfParallelism = 3
                };
                int score = random.Next(0, 1000);
                this.db.SortedSetAdd("leaderboard", "user" + i, score);
            });
            Console.WriteLine("Data added to Redis");
        }

        public void GetSortedSet()
        {
            using (ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(
           new ConfigurationOptions
           {
               EndPoints = { "gamescoretest.redis.cache.windows.net:6380" },
               Password = "zTXCbR9GZ11Rz0SbsjGGPhuQ8lLwQcFY3AzCaMDMLEo="
           }))
            {
                this.db = redis.GetDatabase();
                var sortedList = this.db.SortedSetRangeByRankWithScores("leaderboard", 0, 9, Order.Descending);
                foreach (var sortedSet in sortedList)
                {
                    
                    Console.WriteLine(sortedSet.ToString());
                }
            }

           
        }
        
    }
}
