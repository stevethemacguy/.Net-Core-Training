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
                //Get the POIs from the DB and map them to a List of POI DTOs
                var pois = _cityInfoRepo.GetPointsOfInterestForCity(cityId);
                var poisToReturn = AutoMapper.Mapper.Map<IEnumerable<PointOfInterestDto>>(pois);

                return Ok(poisToReturn);
            }
        }

        [HttpGet("{cityId}/pointOfInterest/{poiId}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int poiId)
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
            return Ok(AutoMapper.Mapper.Map<PointOfInterestDto>(poi));
        }

        //USE POST TO CREATE. we use the same url route, but just make it a post. Also, we use POIForCreated (so we can use validation attributes)
        //[FromBody] says to get the POI from the post body and try to de-serialize it to a PointsOfInterestForCreation object
        [HttpPost("{cityId}/pointOfInterest")]
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

            // See if the city that was passed in exists in the database
            var theCityExists = _cityInfoRepo.CityExists(cityId);

            if (!theCityExists)
            {
                return NotFound();
            }

            //The pointOfInterest comes from the post body and is of type PointOfInterestForCreation.
            //Since we created a mapping for this in the mapper (see startup.cs), we can create a POI entity by mapping it from the POIForCreation.
            var finalPointOfInterest = AutoMapper.Mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            _cityInfoRepo.AddPointOfInterestForCity(cityId,finalPointOfInterest);

            //Attempt to save the new POI to the repo. If it doesn't return an object, then it fails
            if (!_cityInfoRepo.Save())
            {
                _logger.LogInformation("There was a problem when trying to save the new POI to the database.");
                return StatusCode(500, "A problem occured while handling your request");
            }

            var newlyCreatedPoi = AutoMapper.Mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);

            //This returns a 201 (created) response with the URL where the newly created resource can be found.
            //Since we use the Get request to get a POI, we're saying that in order to get to this resource, they'll
            //need to use the GetPointOfInterest Route (passing in a cityId and POI Id that the Get Route needs).
            //IMPORTANT: The GET route MUST have a name that matches the name in CreatedAtRoute here (e.g. "GetPointOfInterest")
            return CreatedAtRoute("GetPointOfInterest", 
                                   new { cityId = cityId, poiId = newlyCreatedPoi.Id }, 
                                   newlyCreatedPoi);
        }

        //USE PUT TO UPDATE
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
            var doesCityExist = _cityInfoRepo.CityExists(cityId);

            if (doesCityExist == false)
            {
                return NotFound();
            }

            //Get the POI entity that's in the DB, so we can update it.
            var pointOfInterestEntity = _cityInfoRepo.GetPointOfInterestForCity(cityId, poiId);

            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            // The first argument is the new Object you're using to overwrite the existing entity. The second argument is the existing entity in the DB
            AutoMapper.Mapper.Map(pointOfInterest, pointOfInterestEntity); //This will overwrite the pointOfInterestEntity with the new pointOfInterest

            if (!_cityInfoRepo.Save())
            {
                _logger.LogInformation("There was a problem when trying to update an existing POI in the database.");
                return StatusCode(500, "A problem occured while handling your request");
            }

            //Since we're just updating
            return NoContent();
        }

        //USE PATCH TO MAKE PARTIAL UPDATES
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
            var doesCityExist = _cityInfoRepo.CityExists(cityId);

            if (doesCityExist == false)
            {
                return NotFound();
            }

            //Get the POI entity that's in the DB, so we can update it.
            var pointOfInterestEntity = _cityInfoRepo.GetPointOfInterestForCity(cityId, poiId);

            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            //Map the entitiy to a DTO, so we can then change the DTO properties
            var poiToPatch = AutoMapper.Mapper.Map<PointOfInterestForUpdate>(pointOfInterestEntity);

            //Apply the patch document to the poiToPatch. We pass in the ModelState to make sure it is valid before attempting to patch
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

            //Now that poiToPatch has been updated to the new values from the patchDoc, 
            //overwirte the poi entity in our DB (i.e. the source is the poiToPatch DTO, whichc overwrites the destination (existing) entity (pointOfInterestEntity))
            AutoMapper.Mapper.Map(poiToPatch, pointOfInterestEntity);

            //Since we just changed an entity, save changes on the repo to ensure the change persists.
            if (!_cityInfoRepo.Save())
            {
                _logger.LogInformation("There was a problem when trying to update an existing POI in the database.");
                return StatusCode(500, "A problem occured while handling your request");
            }

            return NoContent();
        }

        // DELETE
        [HttpDelete("{cityId}/pointsOfInterest/{poiId}")]
        public IActionResult DeletePoi(int cityId, int poiId)
        {
            //Check that the city exists
            //See if the city that was passed in exists in the collection
            var cityExists = _cityInfoRepo.CityExists(cityId);

            if (cityExists == false)
            {
                return NotFound();
            }

            //Get the POI entity that's in the DB, so we can delete it.
            var toDeletetEntity = _cityInfoRepo.GetPointOfInterestForCity(cityId, poiId);

            if (toDeletetEntity == null)
            {
                return NotFound();
            }

            //Remove the Poi
            _cityInfoRepo.DeletePointOfInterest(toDeletetEntity);

            //Since we just removed an entity form the DB, save changes on the repo to ensure the change persists.
            if (!_cityInfoRepo.Save())
            {
                _logger.LogInformation("The attempt to delete a POI from the database FAILED.");
                return StatusCode(500, "A problem occured while handling your request");
            }

            _emailService.Send("Point of interest deleted.",
                   $"Point of interest {toDeletetEntity.Name} with id {toDeletetEntity.Id} was deleted.");

            return NoContent();
        }
    }
}
