using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;

namespace NRTPAnalyzer.CLI
{
    class Program
    {
        public class Options
        {
            [Option('p', "dataPath", Required = false, HelpText = "Path to .dat file (defaults to current directory/ridelog.dat)")]
            public string DataPath { get; set; }
            [Option('o', "outputPath", Required = false, HelpText = "Path to output file (defaults to current directory/answers.log)")]
            public string OutputPath { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(ParseNRTPData);
        }

        private static void ParseNRTPData(Options options)
        {
            string dataPath;
            string outputPath;
            if (!string.IsNullOrEmpty(options.DataPath))
            {
                dataPath = options.DataPath;
            }
            else
            {
                dataPath = "./ridelog.dat";
            }

            if (!string.IsNullOrEmpty(options.OutputPath))
            {
                outputPath = options.OutputPath;
            }
            else
            {
                outputPath = "./results.txt";
            }

            List<NRTPDataRow> list = NRTPUtility.ParseNRTPData(dataPath);

            //Question 1
            int totalSeconds = 0;
            list.ForEach(x => totalSeconds = totalSeconds + x.Duration);
            TimeSpan totalSpan = TimeSpan.FromSeconds(totalSeconds);

            //Question 2
            int currentYear = int.Parse((DateTime.Now).ToString("yyyy"));
            //Only get records where the user is at least 40
            int overFourtyCount = list.FindAll(x => (currentYear - (int.Parse(x.UserBirthYear) + 1)) > 40).Count;
            double overFourtyPercent = (double)overFourtyCount / list.Count;

            //Question 3
            //Get all subscriber trips
            int subscriberTripListCount = list.FindAll(x => x.UserType == UserTypeEnum.Member).Count;

            //Question 4
            //We dont really care about the object we get with the key, we just care how many of these records exist in the same hour
            IEnumerable<IGrouping<int, int>> hoursQuery = list.GroupBy(x => x.StartTime.Hour, x => x.StartTime.Hour);

            int resultHourCount = -1;
            int resultHour = 0;

            foreach (IGrouping<int, int> hourGroup in hoursQuery)
            {
                if (resultHourCount < hourGroup.Count())
                {
                    resultHour = hourGroup.Key;
                    resultHourCount = hourGroup.Count();
                }
            }

            //Question 5
            //We dont really care about the object we get with the key, we just care how many of these records exist for the key
            IEnumerable<IGrouping<string, string>> mostPopularBikeQuery = list.GroupBy(x => string.Format("{0},{1}", x.BikeID, x.BikeType), x => string.Format("{0},{1}", x.BikeID, x.BikeType));
            string resultBikeInfo = "";
            int resultBikeCount = 0;

            foreach (IGrouping<string, string> popularBikeItem in mostPopularBikeQuery)
            {
                if (resultBikeCount < popularBikeItem.Count())
                {
                    resultBikeInfo = popularBikeItem.Key;
                    resultBikeCount = popularBikeItem.Count();
                }
            }


            if (!File.Exists(outputPath))
            {
                using (StreamWriter sw = File.CreateText(outputPath))
                {
                    sw.WriteLine(String.Format("Total trip duration: {0:D2}h:{1:D2}m:{2:D2}s", totalSpan.Hours, totalSpan.Minutes, totalSpan.Seconds));
                    sw.WriteLine(String.Format("% of trips by people over 40: {0}", overFourtyPercent.ToString("0.0%")));
                    sw.WriteLine(String.Format("Trips completed by subscribers: {0}", subscriberTripListCount));
                    sw.WriteLine(String.Format("Busiest Start Hour: {0} Trips Started in that Hour: {1}", resultHour, resultHourCount));
                    sw.WriteLine(String.Format("Bike with most rides: {0} Trips Started on that bike: {1}", resultBikeInfo, resultBikeCount));
                }
            }
            else
            {
                StreamWriter sw = File.AppendText(outputPath);
                sw.WriteLine(String.Format("Total trip duration: {0:D2}h:{1:D2}m:{2:D2}s", totalSpan.Hours, totalSpan.Minutes, totalSpan.Seconds));
                sw.WriteLine(String.Format("% of trips by people over 40: {0}", overFourtyPercent.ToString("0.0%")));
                sw.WriteLine(String.Format("Trips completed by subscribers: {0}", subscriberTripListCount));
                sw.WriteLine(String.Format("Busiest Start Hour: {0} Trips Started in that Hour: {1}", resultHour, resultHourCount));
                sw.WriteLine(String.Format("Bike with most rides: {0} Trips Started on that bike: {1}", resultBikeInfo, resultBikeCount));
                sw.Close();
            }
        }
    }
}
