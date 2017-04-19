using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Routing;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class PointOfInterestController : Controller
    {
        private ILogger<PointOfInterestController> _logger;
        private IMailService _emailService;
        private ICityInfoRepository _cityInfoRepo;
        // Constructor to use the logger
        public PointOfInterestController(ILogger<PointOfInterestController> logger, IMailService emailService, ICityInfoRepository cityInfoRepo)
        {
            _logger = logger;
            _cityInfoRepo = cityInfoRepo;
            _emailService = emailService;
        }

        //My new attempt
        [HttpGet("{cityId}/pointsOfInterest")]
        public IActionResult GetPointsOfInterestForCity(int cityId)
        {
            //See if the city that was passed in exists in the collection
            var theCity = _cityInfoRepo.CityExists(cityId);

            if (!theCity)
            {
                // Similar syntax to ES6. Logs an error to the debug console.
                _logger.LogInformation($"City with ID: {cityId} was not found");
                return NotFound();
            }
            else
            {
                //Build the iEnumberale result
                var pois = _cityInfoRepo.GetPointsOfInterestForCity(cityId);

                var poisToReturn = new List<PointOfInterestDto>();

                foreach (var p in pois)
                {
                    poisToReturn.Add(new PointOfInterestDto()
                    {
                        Id = p.Id,
                        Description = p.Description,
                        Name = p.Name
                    });
                }

                return Ok(poisToReturn);
            }
        }

        [HttpGet]
        [Route("{cityId}/pointOfInterest/{poiId}")]
        public IActionResult getPointOfInterest(int cityId, int poiId)
        {
            //See if the city that was passed in exists in the collection
            var theCity = _cityInfoRepo.CityExists(cityId);

            if (!theCity)
            {
                // Similar syntax to ES6. Logs an error to the debug console.
                _logger.LogInformation($"City with ID: {cityId} was not found");
                return NotFound();
            }

            //Get a single POI for a specified CityId
            var poi = _cityInfoRepo.GetPointOfInterestForCity(cityId, poiId);

            if (poi == null)
            {
                return NotFound();
            }

            //Map from the DB entity to an object we can use in the application
            var poiResult = new PointOfInterestDto()
            {
                Id = poi.Id,
                Description = poi.Description,
                Name = poi.Name
            };

            return Ok(poiResult);
        }

        //Old way using the in-memory data store
        //Get a specific pointOfInterest by it's id
        //[HttpGet("{cityId}/pointsOfInterest/{poiId}", Name = "GetPointOfInterest")]
        //public IActionResult GetPointOfInterest(int cityId, int poiId)
        //{
        //    try
        //    {
        //        // See if the city that was passed in exists in the collection
        //        var theCity = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

        //        if (theCity == null)
        //        {
        //            // Similar syntax to ES6. Logs an error to the debug console.
        //            _logger.LogInformation($"City with ID: {cityId} was not found");
        //            return NotFound();
        //        }

        //        // Check for a POI with the specified id
        //        var poi = theCity.PointsOfInterest.FirstOrDefault(p => p.Id == poiId);

        //        if (poi == null)
        //        {
        //            return NotFound();
        //        }

        //        return Ok(poi);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogWarning($"An exception occurred: {ex}");

        //        // This is publically visible, so should be generic (we already have something in the logger for developers).
        //        return StatusCode(500, "A problem occured with your request");
        //    }
        //}

        // CREATE. we use the same url route, but just make it a post. Also, we use POIForCreated (so we can use validation attributes)
        //[FromBody] says to get the POI from the post body and try to de-serialize it to a PointsOfInterestForCreation object
        [HttpPost("{cityId}/pointsOfInterest")]
        public IActionResult CreatePoi(int cityId, [FromBody] PointOfInterestForCreation pointOfInterest)
        {
            //If the data sent cannot be de-serialized into a PointsOfInterestForCreation, the POI will be null
            if (pointOfInterest == null)
            {
                return BadRequest(); //The consuming app messed up when sending data, so let them know it was a bad request.
            }

            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description", "Description and Name can not be the same");
            }

            //Check for things like the Required or MaxLength Attributes, which can make the model state invalid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // See if the city that was passed in exists in the collection
            var theCity = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (theCity == null)
            {
                return NotFound();
            }

            
            //Create a unique id
            var maxPointOfInterestId = CitiesDataStore.Current.Cities.SelectMany(
                c => c.PointsOfInterest).Max(p => p.Id);

            //Map the POIForCreation values to a POI since we use the POI in the rest of the app
            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            theCity.PointsOfInterest.Add(finalPointOfInterest);

            //This returns a response with the URL where the newly created resource can be found.
            //Since we use the Get request to get a POI, we're saying that in order to get to this resource, they'll
            //need to use the GetPointOfInterest Route (passing in a cityId and POI Id that the Get Route needs).
            return CreatedAtRoute("GetPointOfInterest", new
                {
                    cityId = cityId,
                    poiId = finalPointOfInterest.Id
                }, 
                finalPointOfInterest        //Also pass in the newly created point, that will end up in the response body.
            );
        }

        [HttpPut("{cityId}/pointsOfInterest/{poiId}", Name = "UpdatePointOfInterest")]
        public IActionResult UpdatePointOfInterest(int cityId, int poiId, 
            [FromBody] PointOfInterestForUpdate pointOfInterest)
        {
            //If the data sent cannot be de-serialized into a PointsOfInterestForUpdate, the POI will be null
            if (pointOfInterest == null)
            {
                return BadRequest(); //The consuming app messed up when sending data, so let them know it was a bad request.
            }

            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description", "Description and Name can not be the same");
            }

            //Check for things like the Required or MaxLength Attributes, which can make the model state invalid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // See if the city that was passed in exists in the collection
            var theCity = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (theCity == null)
            {
                return NotFound();
            }

            //Update the POI that's in the DB (faked at this point). With PUT, you must update ALL properties
            var poiToUpdate= theCity.PointsOfInterest.FirstOrDefault(p => p.Id == poiId);
            poiToUpdate.Name = pointOfInterest.Name;
            poiToUpdate.Description = pointOfInterest.Description;


            //Since we're just updating
            return NoContent();
        }

        [HttpPatch("{cityId}/pointsOfInterest/{poiId}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int poiId,
            [FromBody] JsonPatchDocument<PointOfInterestForUpdate> patchDoc )
        {

            //If the data sent cannot be de-serialized into a PointsOfInterestForUpdate, the POI will be null
            if (patchDoc == null)
            {
                return BadRequest(); //The consuming app messed up when sending data, so let them know it was a bad request.
            }

            // See if the city that was passed in exists in the collection
            var theCity = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (theCity == null)
            {
                return NotFound();
            }
            
            //Get the POI
            var poiFromDb = theCity.PointsOfInterest.FirstOrDefault(p => p.Id == poiId);

            if (poiFromDb == null)
            {
                return NotFound();
            }

            //Convert the POI into a PointOfInterestForUpdate
            var poiToPatch = new PointOfInterestForUpdate()
            {
                Name = poiFromDb.Name,
                Description = poiFromDb.Description
            };

            //Apply the patch document to the poiToUpdate. We pass in the ModelState to make sure it is valid before attempting to patch
            patchDoc.ApplyTo(poiToPatch, ModelState);

            //This ModelState actually refers to the patchDoc, NOT to the POI, so we need to validate that seperately below
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Rechecks validation rules on the POI now that the POI has been patched. 
            TryValidateModel(poiToPatch);

            //If there were any errors when re-validation the poi (after the patch is applied), they will be added to the model state.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Now that poiToPatch has been updated to the new values from the patchDoc, apply them to the poi in our DB
            poiFromDb.Name = poiToPatch.Name;
            poiFromDb.Description = poiToPatch.Description;

            return NoContent();
        }

        [HttpDelete("{cityId}/pointsOfInterest/{poiId}")]
        public IActionResult DeletePoi(int cityId, int poiId)
        {
            //Check that the city and poi exist
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return BadRequest();
            }

            var toDelete = city.PointsOfInterest.FirstOrDefault(p => p.Id == poiId);

            if (toDelete == null)
            {
                return BadRequest();
            }

            //Remove the Poi
            city.PointsOfInterest.Remove(toDelete);

            _emailService.Send("Point of interest deleted.",
                   $"Point of interest {toDelete.Name} with id {toDelete.Id} was deleted.");
            return NoContent();
        }
    }
}
