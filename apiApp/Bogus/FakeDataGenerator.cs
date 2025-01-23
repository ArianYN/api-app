using Bogus;
namespace apiApp.Bogus;

public class FakeDataGenerator
{
    public static class FakeBogusDataGenerator
    {
        public static List<Models.Car> GenerateCars(int count, List<int> validOwnerIds)
        {
            if (!validOwnerIds.Any() || validOwnerIds == null)
            {
                throw new ArgumentNullException("List invalid or empty");
            }
            
            Faker<Models.Car> faker = new Faker<Models.Car>()
                .RuleFor(u => u.OwnerId, f => f.PickRandom(validOwnerIds))
                .RuleFor(u => u.Vin, f => f.Vehicle.Vin())
                .RuleFor(u => u.Manufacturer, f => f.Vehicle.Manufacturer())
                .RuleFor(u => u.Model, f => f.Vehicle.Model())
                .RuleFor(u => u.Type, f => f.Vehicle.Type())
                .RuleFor(u => u.Fuel, f => f.Vehicle.Fuel());
            return faker.Generate(count);
        }

        public static List<Models.Person> GeneratePeople(int count)
        {
            Faker<Models.Person> faker = new Faker<Models.Person>()
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Address, f => f.Address.StreetAddress());
            return faker.Generate(count);
        }
    }
}