using CityInfo.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API
{
    //Just an in-memory data store (not very realistic)
    public class CitiesDataStore
    {
        //A singleton
        public static CitiesDataStore Current { get; } = new CitiesDataStore();
        public List<CityDto> Cities { get; set; }

        //Constructor
        public CitiesDataStore()
        {
            // A list of CityDto Dummy Objects
            Cities = new List<CityDto>()
            {
                new CityDto()
                {
                     Id = 1,
                     Name = "New York City",
                     Description = "The one with that big park.",
                     PointsOfInterest = new List<PointOfInterestDto>()
                     {
                         new PointOfInterestDto() {
                             Id = 1,
                             Name = "Central Park",
                             Description = "The most visited urban park in the United States." },
                          new PointOfInterestDto() {
                             Id = 2,
                             Name = "Empire State Building",
                             Description = "A 102-story skyscraper located in Midtown Manhattan." },
                     }
                },
                new CityDto()
                {
                    Id = 2,
                    Name = "Sacramento",
                    Description = "The Capital City",
                    PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 3,
                            Name = "The State Capital",
                            Description = "Where public officials pretend to do work."
                        },
                        new PointOfInterestDto()
                        {
                            Id = 4,
                            Name = "MidTown Stomp",
                            Description = "The best place to swing dance on a Saturday Night"
                        },
                    }

                },
                new CityDto()
                {
                    Id= 3,
                    Name = "Paris",
                    Description = "The one with that big tower.",
                    PointsOfInterest = new List<PointOfInterestDto>()
                     {
                         new PointOfInterestDto()
                         {
                             Id = 5,
                             Name = "Eiffel Tower",
                             Description = "A wrought iron lattice tower on the Champ de Mars, named after engineer Gustave Eiffel."
                         },
                         new PointOfInterestDto()
                         {
                             Id = 6,
                             Name = "The Louvre",
                             Description = "The world's largest museum."
                         }
                     }
                }
            };

        }
    }
}
