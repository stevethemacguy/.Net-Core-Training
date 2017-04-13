using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Models
{
    public class PointOfInterestForUpdate
    {
        [Required(ErrorMessage = "You must provide a name value")]
        [MaxLength(40, ErrorMessage = "Name can not be longer than 40 charactors")]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }
    }
}
