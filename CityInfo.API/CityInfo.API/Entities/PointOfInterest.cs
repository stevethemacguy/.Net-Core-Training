    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Entities
{
    public class PointOfInterest
    {
        //You can specify [key] and [DatabaseGenerated(DatabaseGeneratedOption.Identity)] here like in City.cs, but
        //this is an example to show that SQL Server will still generate the entity without them specified.
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        //Create a foreign key that points to the parent City. In this case, "City" is considered a "navigation property"
        public City City { get; set; }
        public int CityId { get; set; }

        //This is how you would do it explicitly, but you don't need to
        //[ForeignKey("CityId")]  //This is
        //public City City { get; set; }  

    }
}
