using System;
using System.Collections.Generic;
using System.Text;

namespace NRTPAnalyzer
{
    public class NRTPDataRow
    {
        public int Duration { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int? StartStationId { get; set; }
        public string StartStation { get; set; }
        public double StartLat { get; set; }
        public double StartLong { get; set; }
        public int? EndStationId { get; set; }
        public string EndStation { get; set; }
        public double EndLat { get; set; }
        public double EndLong { get; set; }
        public int BikeID { get; set; }
        public char BikeType { get; set; }
        //00 = single ride or Day pass 01 = Annual or Monthly Member
        public UserTypeEnum UserType { get; set; }

        private string _userBirthYear;
        public string UserBirthYear 
        {
            get
            {
                return this._userBirthYear;
            }

            set
            {
                if(value.Length != 4)
                {
                    throw new Exception("Invalid Birth Year Format (yyyy)");
                } 
                else
                {
                    _userBirthYear = value;
                }
            }
        }
        //0 unknown, 1 male, 2 female
        public UserGenderEnum UserGender { get; set; }
    }
}
