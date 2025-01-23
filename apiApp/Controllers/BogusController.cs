using Microsoft.AspNetCore.Mvc;
using apiApp.Data;
using apiApp.Bogus;
using apiApp.Models;

namespace apiApp.Bogus;

[Route("api/[controller]")]
[ApiController]

    public class BogusController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public BogusController(DatabaseContext context)
        {
            _context = context;
        }
        
        [HttpPost("people")]
        public IActionResult GenerateBogusPeople([FromQuery] int count = 1)
        {
            if (count <= 0)
            {
                return BadRequest("Count must be greater than 0");
            }

            List<Person> people = FakeDataGenerator.FakeBogusDataGenerator.GeneratePeople(count);
            _context.People.AddRange(people);
            _context.SaveChanges();
            return Ok(new { Message = $"{count} people generated!" });
        }

        [HttpPost("cars")]
        public IActionResult GenerateBogusCars([FromQuery] int count = 1)
        {
            if (count <= 0)
            {
                return BadRequest("Count must be greater than 0.");
            }
            
            List<int> validUserId = _context.People.Select(u => u.Id).ToList();

            List<Car> fakeCars = FakeDataGenerator.FakeBogusDataGenerator.GenerateCars(count, validUserId);

            _context.Cars.AddRange(fakeCars);
            _context.SaveChanges();

            return Ok(new { Message = $"{count} cars generated!" });
        }
    }
