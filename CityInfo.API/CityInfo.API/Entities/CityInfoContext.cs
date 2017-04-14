using Microsoft.EntityFrameworkCore;
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

        DbSet<City> Cities { get; set; }
        DbSet<PointOfInterest> PointsOfInterest{ get; set; }
    }
}
