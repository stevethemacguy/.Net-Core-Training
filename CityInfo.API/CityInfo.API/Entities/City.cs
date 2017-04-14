using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Entities
{
    public class City
    {
        [Key] //[Key] is technically not needed when you use "Id" as a name. Entity Framework knows to make Id a primary key based on the naming convention.
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]  //This is the default and does NOT need to be specified. It generates a new key when a City is Added to the DB
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // You could also set this in the constructor
        public ICollection<PointOfInterest> PointsOfInterest { get; set; }
            = new List<PointOfInterest>();
    }
}
