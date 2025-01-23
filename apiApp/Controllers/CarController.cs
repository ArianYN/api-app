using apiApp.Data;
using apiApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace apiApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CarController : ControllerBase
{
    private readonly DatabaseContext _context;

    public CarController(DatabaseContext context)
    {
        _context = context; // Initialize the database context through dependency injection
    }

    [HttpPost("create")] // CREATE
    public async Task<IActionResult> AddCar([FromQuery] CarDTO carDTO)
    {
        if (carDTO == null)
        {
            return BadRequest("Car already exists");
        }

        Car carToAdd = new Car()
        {
            Vin = carDTO.Vin,
            Manufacturer = carDTO.Manufacturer,
            Model = carDTO.Model,
            Type = carDTO.Type,
            Fuel = carDTO.Fuel,
            OwnerId = carDTO.OwnerId,
        };

        // *Check if user exists*

        _context.Cars.Add(carToAdd);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetCars), new { id = carToAdd.Id }, carDTO);
    }

    [HttpGet("read")] // READ
    public async Task<IActionResult> GetCars(string? Manufacturer, string? Model, int pageSize = 100, int pageNumber = 1)
    {
        if (pageNumber < 1 || pageSize < 1)
        {
            return BadRequest("PageNumber and size must be greater than 0");
        }

        int allPeople = _context.People.Count();
        int totalPages = (int)Math.Ceiling(allPeople / (double)pageSize);

        if (pageNumber > totalPages)
        {
            return NotFound("Page number exceeds total pages");
        }
        
        List<Car> cars = new List<Car>();
        
        if (Manufacturer != null && Model == null)
        {
            cars = await _context.Cars.Where(c => c.Manufacturer == Manufacturer)  
                .Skip((pageNumber-1)*pageSize)
                .Take(pageSize)
                .ToListAsync();
            
        } else if (Model != null && Manufacturer == null)
        {
            cars = await _context.Cars.Where(c => c.Model == Model)  
                .Skip((pageNumber-1)*pageSize)
                .Take(pageSize)
                .ToListAsync();
            
        } else if (Model != null && Manufacturer != null)
        {
            cars = await _context.Cars.Where(c => c.Model == Model && c.Manufacturer == Manufacturer)  
                .Skip((pageNumber-1)*pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        else
        {
            cars = await _context.Cars
                .Skip((pageNumber-1)*pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    return Ok(cars);
    }
    [HttpGet("count")] // READ
    public async Task<IActionResult> Count()
    {
        int count = await _context.Cars.CountAsync();
        return Ok(count);
    }

    [HttpPut("update")] // UPDATE
    public async Task<IActionResult> UpdatePerson(int idToUpdate, [FromQuery] CarDTO carDTO)
    {
        Car carToUpdate = await _context.Cars.FindAsync(idToUpdate);

        if (carToUpdate == null)
        {
            return NotFound("Person not found");
        }

        if (carDTO == null)
        {
            return BadRequest("Invalid data");
        }

        if (!string.IsNullOrEmpty(carDTO.Vin))
        {
            carToUpdate.Vin = carDTO.Vin;
        }
        
        if (!string.IsNullOrEmpty(carDTO.Manufacturer))
        {
            carToUpdate.Manufacturer = carDTO.Manufacturer;
        }
        
        if (!string.IsNullOrEmpty(carDTO.Model))
        {
            carToUpdate.Model = carDTO.Model;
        }
        
        if (!string.IsNullOrEmpty(carDTO.Type))
        {
            carToUpdate.Type = carDTO.Type;
        }
        
        if (!string.IsNullOrEmpty(carDTO.Fuel))
        {
            carToUpdate.Fuel = carDTO.Fuel;
        }
        
        if (!string.IsNullOrEmpty((carDTO.OwnerId).ToString()))
        {
            carToUpdate.OwnerId = carDTO.OwnerId;
        }
        
        _context.Cars.Update(carToUpdate);
        await _context.SaveChangesAsync();

        return Ok(carToUpdate);
    }

    [HttpDelete("delete")] // DELETE
    public async Task<IActionResult> DeleteCar(int id)
    {
        Car carToDelete = await _context.Cars.FindAsync(id);
        
        if (carToDelete == null)
        {
            return BadRequest("Car not found");
        }
        
        _context.Cars.Remove(carToDelete);
        await _context.SaveChangesAsync();
        return Ok(carToDelete);
    }

    [HttpPatch("patch")]
    public async Task<IActionResult> PatchCar([FromQuery] CarPatchDTO carUpdateDTO, int changeToNewUserId)
    {
        Car carToEdit = await _context.Cars.FindAsync(carUpdateDTO.Id);

        if (carToEdit == null)
        {
            return NotFound("Car not found");
        }
        Person person = await _context.People.FindAsync(changeToNewUserId);
        if (person == null)
        {
            return NotFound("Person not found");
        }

        carToEdit.OwnerId = changeToNewUserId;

        // Save changes
        _context.Cars.Update(carToEdit);
        await _context.SaveChangesAsync();

        return Ok(carToEdit);
    }
    
}