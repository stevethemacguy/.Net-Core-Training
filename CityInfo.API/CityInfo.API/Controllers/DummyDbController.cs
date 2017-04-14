using CityInfo.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/testdatabase")]
    public class DummyDbController : Controller
    {
        private CityInfoContext _context;

        public DummyDbController(CityInfoContext context)
        {
            _context = context;
        }

        [HttpGet()]
        public IActionResult TestDatabase()
        {
            return Ok();
        }
    }
}
