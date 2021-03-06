﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Entities
{
    public class CityInfoContext : DbContext
    {
        public CityInfoContext(DbContextOptions<CityInfoContext> options): base(options)
        {
            //If the DB is not yet created, then create it with our entity objects.
            //Database.EnsureCreated();
            Database.Migrate();
        }

        //Used to query and save Entities to the DB
        public DbSet<City> Cities { get; set; }
        public DbSet<PointOfInterest> PointsOfInterest{ get; set; }
    }
}
