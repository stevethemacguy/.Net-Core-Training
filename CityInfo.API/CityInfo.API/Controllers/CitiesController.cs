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
            var cityEntities = _cityInfoRepo.GetCities();

            //Map the city entities to the DTO classes using automapper
            var allCities = AutoMapper.Mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities);

            return Ok(allCities);
        }

        [HttpGet("{id}")]
        //id is automatically set with the id parameters coming from the request URL
        public IActionResult GetCity(int id, bool includePois = false)
        {
            var cityToReturn = _cityInfoRepo.GetCity(id, includePois);

            if (cityToReturn == null)
            {
                return NotFound();
            }
            else
            {
                if (includePois)
                {
                    var cityResult = AutoMapper.Mapper.Map<CityDto>(cityToReturn);
                    return Ok(cityResult);
                }
                else
                {
                    var cityResult = AutoMapper.Mapper.Map<CityWithoutPointsOfInterestDto>(cityToReturn);
                    return Ok(cityResult);
                }
            }
        }

        [HttpGet("getcitybyname")]
        //id is automatically set with the id parameters coming from the request URL
        public IActionResult GetCityByName(string name, bool includePois = false)
        {
            var cityToReturn = _cityInfoRepo.GetCityByName(name, includePois);

            if (cityToReturn == null)
            {
                return NotFound();
            }
            else
            {
                if (includePois)
                {
                    var cityResult = AutoMapper.Mapper.Map<CityDto>(cityToReturn);
                    return Ok(cityResult);
                }
                else
                {
                    var cityResult = AutoMapper.Mapper.Map<CityWithoutPointsOfInterestDto>(cityToReturn);
                    return Ok(cityResult);
                }
            }
        }
    }
}
