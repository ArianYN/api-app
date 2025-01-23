using System.Runtime.InteropServices.ComTypes;
using apiApp.Data;
using apiApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APITest.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PeopleController : ControllerBase
{
    private readonly DatabaseContext _context;

    public PeopleController(DatabaseContext context)
    {
        _context = context; // Initialize the database context through dependency injection
    }

    [HttpPost("create")] // CREATE
    public async Task<IActionResult> AddCar([FromQuery] PersonDTO personDTO)
    {
        if (personDTO == null)
        {
            return BadRequest("Car already exists");
        }

        Person personToAdd = new Person()
        {
           FirstName = personDTO.FirstName,
           LastName = personDTO.LastName,
           Address = personDTO.Address,
           Email = personDTO.Email,
           CarsOwnedId = new List<Car>()
        };

        // *Check if user exists* Nah

        _context.People.Add(personToAdd);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPeople), new { id = personToAdd.Id }, personToAdd);
    }

    [HttpGet("read")] // READ
    public async Task<IActionResult> GetPeople(string? FirstName, string? LastName, int pageSize = 100, int pageNumber = 1)
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
        
        
        List<Person> people = new List<Person>();
        
        if (FirstName != null && LastName == null)
        {
            people = await _context.People.Where(c => c.FirstName == FirstName)
                .Skip((pageNumber-1)*pageSize)
                .Take(pageSize)
                .ToListAsync();
            
        } else if (LastName != null && FirstName == null)
        {
            people = await _context.People.Where(c => c.LastName == LastName)  
                .Skip((pageNumber-1)*pageSize)
                .Take(pageSize)
                .ToListAsync();
            
        } else if (LastName != null && FirstName != null)
        {
            people = await _context.People.Where(c => c.LastName == LastName && c.FirstName == FirstName)  
                .Skip((pageNumber-1)*pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        else
        {
            people = await _context.People
                .Skip((pageNumber-1)*pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    return Ok(people);
    }

    [HttpGet("findByid")]
    public async Task<IActionResult> FindByid([FromQuery] int id)
    {
        Person person = await _context.People.FindAsync(id);
        
        if (person == null)
        {
            return NotFound();
        }
        
        return Ok(person);
    }
    
    [HttpGet("count")]
    public async Task<IActionResult> CountPeople()
    {
        int count = await _context.People.CountAsync();
        return Ok(count);
    }

    [HttpPut("update")] // UPDATE
    public async Task<IActionResult> UpdatePerson(int idToUpdate, [FromQuery] PersonDTO personDTO)
    {
        Person personToUpdate = await _context.People.FindAsync(idToUpdate);

        if (personToUpdate == null)
        {
            return NotFound("Person not found");
        }

        if (personDTO == null)
        {
            return BadRequest("Invalid data");
        }

        if (!string.IsNullOrEmpty(personDTO.FirstName))
        {
            personToUpdate.FirstName = personDTO.FirstName;
        }

        if (!string.IsNullOrEmpty(personDTO.LastName))
        {
            personToUpdate.LastName = personDTO.LastName;
        }

        if (!string.IsNullOrEmpty(personDTO.Email))
        {
            personToUpdate.Email = personDTO.Email;
        }

        if (!string.IsNullOrEmpty(personDTO.Address))
        {
            personToUpdate.Address = personDTO.Address;
        }

        _context.People.Update(personToUpdate);
        await _context.SaveChangesAsync();

        return Ok(personToUpdate);
    }

    [HttpDelete("delete")] // DELETE
    public async Task<IActionResult> DeletePerson(int id)
    {
        Person personToDelete = await _context.People.FindAsync(id);
        if (personToDelete == null)
        {
            return BadRequest("Person not found");
        }

        _context.People.Remove(personToDelete);
        await _context.SaveChangesAsync();
        return Ok(personToDelete);
    }

    [HttpPatch("patch")]
    public async Task<IActionResult> PatchPerson([FromQuery] PersonPatchDTO personPatchDTO, int userId = 1)
    {
        if (userId == null)
        {
            return BadRequest("User not found");
        }
        
        Person currentPerson = _context.People.Find(userId);
        
        Person patchedPerson = new Person()
        {
            FirstName = currentPerson.FirstName,
            LastName = currentPerson.LastName,
            Address = currentPerson.Address,
            Email = personPatchDTO.Email,
            CarsOwnedId = currentPerson.CarsOwnedId,
        };
        
        // Save changes
        _context.People.Update(patchedPerson);
        await _context.SaveChangesAsync();

        return Ok(patchedPerson);
    }
    
}