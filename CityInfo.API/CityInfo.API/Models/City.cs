using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Models;

namespace CityInfo.API
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int NumberOfPointsOfInterest
        {
            get { return PointsOfInterest.Count(); }
        }

        // You could also set this in the constructor
        public ICollection<PointOfInterest> PointsOfInterest { get; set; }
            = new List<PointOfInterest>();
    }
}
