using System;

namespace GameScoreDataGen // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DataGenerator dataGenerator = new DataGenerator();
            //dataGenerator.GenerateData();
            dataGenerator.GetSortedSet();
        }
    }
}