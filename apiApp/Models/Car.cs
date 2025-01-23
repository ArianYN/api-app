using System.ComponentModel.DataAnnotations.Schema;

namespace apiApp.Models;

public class Car : Common
{
    public string? Vin { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? Type { get; set; }
    public string? Fuel { get; set; }
    public int OwnerId { get; set; }
    public Person Owner { get; set; }
}

public class CarDTO
{
    public string? Vin { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? Type { get; set; }
    public string? Fuel { get; set; }
    public int OwnerId { get; set; }
}

public class CarPatchDTO
{
    public int Id { get; set; }
}