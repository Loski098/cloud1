using Configuration;
using Seido.Utilities.csSeedGenerator;

namespace Models;

public class Zoo: IZoo, ISeed<Zoo>
{
    public virtual Guid ZooId { get; set;}
    public string Name { get; set; }
    public string City { get; set; }
    public string Country { get; set; }

    public virtual List<IAnimal> Animals { get; set; }
    public virtual List<IEmployee> Employees { get; set; }

    public bool Seeded { get; set; } = false;
    public virtual Zoo Seed (csSeedGenerator seeder)
    {
        ZooId = Guid.NewGuid();
        Country = seeder.Country;
        City = seeder.City(Country);
        Name = $"Zoo {seeder.LatinWordsAsSentence(seeder.Next(1,5), ":")} {seeder.PetName} {seeder.AlbumSuffix}";
        return this;
    }
}