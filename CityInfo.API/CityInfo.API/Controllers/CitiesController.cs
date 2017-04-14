using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class CitiesController : Controller
    {
        private ICityInfoRepository _cityInfoRepo;
        public CitiesController(ICityInfoRepository cityInfoRepo)
        {
            _cityInfoRepo = cityInfoRepo;
        }

        [HttpGet()]
        public IActionResult GetCities()
        {
            //Get the cities from the DB. NOTE: You don't return the entities directly, but use the DTO classes instead
            var cityEntities= _cityInfoRepo.GetCities();

            var allCities = new List<CityWithoutPointsOfInterestDto>();

            //Map the city entities to the DTO classes. TODO: Do this automatically isntead.
            foreach (var cityEntity in cityEntities)
            {
                allCities.Add(new CityWithoutPointsOfInterestDto
                {
                    Id = cityEntity.Id,
                    Description = cityEntity.Description,
                    Name = cityEntity.Name
                });
            }
            
            return Ok(allCities);
        }

        [HttpGet("{id}")]
        //id is automatically set with the id parameters coming from the request URL
        public IActionResult GetCity(int id)
        {
            var cityToReturn = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);

            if (cityToReturn == null)
            {
                return NotFound();
            }
            else
            {
                //Return a basic Json object
                return Ok(cityToReturn);
            }
        }
    }
}
