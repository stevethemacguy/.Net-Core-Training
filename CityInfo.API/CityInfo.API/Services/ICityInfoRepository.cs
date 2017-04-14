using System;
using System.Collections.Generic;
using CityInfo.API.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        //Get all the cities from the DB
        IEnumerable<City> GetCities();

        //Get a single city
        City GetCity(int cityId, bool includePointsOfInterest);

        //Get all POIs
        IEnumerable<PointOfInterest> GetPointsOfInterestForCity(int cityId);

        //Get a single POI for a specified CityId
        PointOfInterest GetPointOfInterestForCity(int cityId);
    }
}
