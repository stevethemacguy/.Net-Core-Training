using System.Collections.Generic;
using CityInfo.API.Models;

namespace CityInfo.API
{
    //Just an in-memory data store (not very realistic)
    public class CitiesDataStore
    {
        //A singleton
        public static CitiesDataStore Current { get; } = new CitiesDataStore();

        public List<City> Cities { get; set; }

        //Constructor
        public CitiesDataStore()
        {
            // A list of CityDto Objects
            Cities = new List<City>()
            {
                new City()
                {
                    Id = 1,
                    Name = "New York City",
                    Description = "It has a big park.",
                    PointsOfInterest = new List<PointOfInterest>()
                    {
                        new PointOfInterest()
                        {
                            Id = 1,
                            Name = "Central Park",
                            Description = "The most visited urban park in the United States."
                        },
                        new PointOfInterest()
                        {
                            Id = 2,
                            Name = "Empire State Building",
                            Description = "A 102-story skyscraper located in Midtown Manhattan."
                        },
                    }
                },
                new City()
                {
                    Id = 2,
                    Name = "Sacramento",
                    Description = "The Capital City",
                    PointsOfInterest = new List<PointOfInterest>()
                    {
                        new PointOfInterest()
                        {
                            Id = 3,
                            Name = "The State Capital",
                            Description = "Where public officials pretend to do work."
                        },
                        new PointOfInterest()
                        {
                            Id = 4,
                            Name = "MidTown Stomp",
                            Description = "The best place to swing dance on a Saturday Night"
                        },
                    }
                },
                new City()
                {
                    Id = 3,
                    Name = "Paris",
                    Description = "It has a big tower.",
                    PointsOfInterest = new List<PointOfInterest>()
                    {
                        new PointOfInterest()
                        {
                            Id = 5,
                            Name = "Eiffel Tower",
                            Description =
                                "A wrought iron lattice tower on the Champ de Mars, named after engineer Gustave Eiffel."
                        },
                        new PointOfInterest()
                        {
                            Id = 6,
                            Name = "The Louvre",
                            Description = "The world's largest museum."
                        },
                    }
                }
            };
        }
    }
}
