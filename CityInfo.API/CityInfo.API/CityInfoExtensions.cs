using CityInfo.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API
{
    //CityInfoExtensions is used to seed the local SQL DB with data. 
    //This is a "code-first" approach (i.e. we're creating new DB entities using C# classes/objects)
    public static class CityInfoExtensions
    {
        //"this" here means that EnsureSeedDataForContext extends CityInfoContext
        public static void EnsureSeedDataForContext(this CityInfoContext context)
        {
            //If something already exists in the DB, then we don't need to seed it with data, so exit.
            if (context.Cities.Any())
            {
                return;
            }

            //Create the seed data
            var cities = new List<City>()
            {
                new City()
                {
                     Name = "New York City",
                     Description = "City of Skyscrapers.",
                     PointsOfInterest = new List<PointOfInterest>()
                     {
                         new PointOfInterest() {
                             Name = "Central Park",
                             Description = "The most visited urban park in the United States."
                         },
                          new PointOfInterest() {
                             Name = "Empire State Building",
                             Description = "A 102-story skyscraper located in Midtown Manhattan."
                          },
                     }
                },
                 new City()
                {
                    Name = "Sacramento",
                    Description = "The Capital City",
                    PointsOfInterest = new List<PointOfInterest>()
                    {
                        new PointOfInterest()
                        {
                            Name = "The State Capital",
                            Description = "Where public officials pretend to do work."
                        },
                        new PointOfInterest()
                        {
                            Name = "MidTown Stomp",
                            Description = "The best place to swing dance on a Saturday Night"
                        },
                        new PointOfInterest()
                        {
                            Name = "Burgers and Brew",
                            Description = "The best place to enjoy food and beer with your friends."
                        },
                    }
                },
                new City()
                {
                    Name = "Paris",
                    Description = "It's in France.",
                    PointsOfInterest = new List<PointOfInterest>()
                     {
                         new PointOfInterest() {
                             Name = "Eiffel Tower",
                             Description =  "A wrought iron lattice tower on the Champ de Mars, named after engineer Gustave Eiffel."
                         },
                          new PointOfInterest() {
                             Name = "The Louvre",
                             Description = "The world's largest museum."
                          },
                     }
                }
            };

            //Track the new entities
            context.Cities.AddRange(cities);

            //Since we just added entities to the DB, save changes so that they persist after the application closes.
            context.SaveChanges();
        }
    }
}
