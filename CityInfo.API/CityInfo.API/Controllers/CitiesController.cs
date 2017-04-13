using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class CitiesController : Controller
    {
        [HttpGet()]
        public IActionResult GetCities()
        {
            //Return a basic Json object
            return Ok(CitiesDataStore.Current.Cities);
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
