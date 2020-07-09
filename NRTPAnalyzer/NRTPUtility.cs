using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace NRTPAnalyzer
{
    public class NRTPUtility
    {
        public static List<NRTPDataRow> ParseNRTPData(string filePath)
        {
            List<NRTPDataRow> result = new List<NRTPDataRow>();

            //Get All lines (start with !! and end with %%)
            string linePattern = @"(?<=!!).*?(?=%%)";
            Regex lineRegex = new Regex(linePattern);

            foreach(Match match in lineRegex.Matches(File.ReadAllText(filePath)))
            {
                string content = match.Value;
                NRTPDataRow row = new NRTPDataRow();
                //Now it is time to parse the content
                List<string> splitContent = new List<string>(content.Split('|'));

                /*
                 * Order:
                 *  0 - duration,startTime,endTime
                 *  1 - startStationId,startStation
                 *  2 - startLat ,startLong
                 *  3 - endStationId,endStation
                 *  4 - endLat,endLong
                 *  5 - bikeId,bikeType
                 *  6 - userType,userBirthYear,userGender
                 */

                LoadDurationContent(splitContent[0], row);
                LoadStartStationContent(splitContent[1], row);
                LoadStartLatLongContent(splitContent[2], row);
                LoadEndStationContent(splitContent[3], row);
                LoadEndLatLongContent(splitContent[4], row);
                LoadBikeContent(splitContent[5], row);
                LoadUserContent(splitContent[6], row);

                result.Add(row);
            }


            return result;
        }

        private static void LoadUserContent(string userContent, NRTPDataRow row)
        {
            /*
             * Order:
             * 0 - userType
             * 1 - userBirthYear
             * 2 - userGender
             */
            List<string> splitContent = new List<string>(userContent.Split(','));

            switch (splitContent[0])
            {
                case "00":
                    row.UserType = UserTypeEnum.DayPass;
                    break;
                case "01":
                    row.UserType = UserTypeEnum.Member;
                    break;
                default:
                    throw new Exception("Invalid user type found: " + splitContent[0]);
            }

            row.UserBirthYear = splitContent[1];

            switch (splitContent[2])
            {
                case "0":
                    row.UserGender = UserGenderEnum.Unknown;
                    break;
                case "1":
                    row.UserGender = UserGenderEnum.Male;
                    break;
                case "2":
                    row.UserGender = UserGenderEnum.Female;
                    break;
                default:
                    throw new Exception("Invalid gender identifier found: " + splitContent[2]);
            }
        }

        private static void LoadBikeContent(string bikeContent, NRTPDataRow row)
        {
            /*
             * Order:
             * 0 - bikeId
             * 1 - bikeType
             */
            List<string> splitContent = new List<string>(bikeContent.Split(','));

            row.BikeID = int.Parse(splitContent[0]);
            row.BikeType = char.Parse(splitContent[1]);
        }

        private static void LoadEndLatLongContent(string endLatLongContent, NRTPDataRow row)
        {
            /*
             * Order:
             * 0 - endLat
             * 1 - endLong
             */
            List<string> splitContent = new List<string>(endLatLongContent.Split(','));

            row.EndLat = double.Parse(splitContent[0]);
            row.EndLong = double.Parse(splitContent[1]);
        }

        private static void LoadEndStationContent(string endStationContent, NRTPDataRow row)
        {
            /*
             * Order:
             * 0 - endStationId
             * 1 - endStation
             */
            List<string> splitContent = new List<string>(endStationContent.Split(','));
            int tmp;
            row.EndStationId = int.TryParse(splitContent[0], out tmp) ? int.Parse(splitContent[0]) : (int?)null;
            row.EndStation = splitContent[1];
        }

        private static void LoadStartLatLongContent(string startLatLongContent, NRTPDataRow row)
        {
            /*
             * Order:
             * 0 - startLat
             * 1 - startLong
             */
            List<string> splitContent = new List<string>(startLatLongContent.Split(','));

            row.StartLat = double.Parse(splitContent[0]);
            row.StartLong = double.Parse(splitContent[1]);
        }

        private static void LoadStartStationContent(string startStationRow, NRTPDataRow row)
        {
            /*
             * Order:
             * 0 - startStationId
             * 1 - startStation
             */
            List<string> splitContent = new List<string>(startStationRow.Split(','));
            int tmp;
            row.StartStationId = int.TryParse(splitContent[0], out tmp) ? int.Parse(splitContent[0]) : (int?)null;
            row.StartStation = splitContent[1];
        }

        private static void LoadDurationContent(string durationRow, NRTPDataRow row)
        {
            /*
             * Order:
             * 0 - duration
             * 1 - startTime
             * 2 - endTime
             */

            List<string> splitContent = new List<string>(durationRow.Split(','));

            row.Duration = int.Parse(splitContent[0]);
            row.StartTime = DateTime.FromOADate(double.Parse(splitContent[1]));
            row.EndTime = DateTime.FromOADate(double.Parse(splitContent[2]));
        }
    }
}
