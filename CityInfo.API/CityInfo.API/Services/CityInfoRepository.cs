using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context)
        {
            _context = context;
        }

        public void AddPointOfInterestForCity(int cityId, PointOfInterest pointOfInterest)
        {
            var city = GetCity(cityId, false);
            city.PointsOfInterest.Add(pointOfInterest);
        }

        public bool CityExists(int cityId)
        {
            return _context.Cities.Any(c => c.Id == cityId);
        }

        public IEnumerable<City> GetCities()
        {
            //Order the cities by name
            return _context.Cities.OrderBy(c => c.Name).ToList();
        }

        //Use a boolean to allow the consumer to decide whether to retrieve the POIs instead of always showing them.
        public City GetCity(int cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                //Get he point of interest that has the same ID as the city ID. FirstOrDefault basically executes the query
                return _context.Cities
                    .Include(c => c.PointsOfInterest)
                    .FirstOrDefault(c => c.Id == cityId);
            }

            //If we're not including the POIs, then jus return the city
            return _context.Cities.FirstOrDefault(c => c.Id == cityId);
        }

        public PointOfInterest GetPointOfInterestForCity(int cityId, int pointOfInterestId)
        {
            return _context.PointsOfInterest
                .FirstOrDefault(p => p.CityId == cityId && p.Id == pointOfInterestId);
        }

        public IEnumerable<PointOfInterest> GetPointsOfInterestForCity(int cityId)
        {
            //Return ALL the POIs (since we're not using FirstOrDefault)
            return _context.PointsOfInterest
                .Where(p => p.CityId == cityId).ToList();
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _context.PointsOfInterest.Remove(pointOfInterest);
        }

        //Used to persist changes in the SQL DB (i.e. when you create or delete something from the DB, you must call save on the DB context).
        public bool Save()
        {
            //SaveChanges returns the number of entities that were changed, if any.
            //In this case, we'll just return true if at least one entity was updated/removed.
            return (_context.SaveChanges() >= 0);
        }

        public City GetCityByName(string name, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                //Get he point of interest that has the same ID as the city ID. FirstOrDefault basically executes the query
                return _context.Cities
                    .Include(c => c.PointsOfInterest)
                    .FirstOrDefault(c => c.Name == name);
            }

            //If we're not including the POIs, then just return the city
            return _context.Cities.FirstOrDefault(c => c.Name == name);
        }
    }
}
