using Configuration;
using Seido.Utilities.csSeedGenerator;

namespace Models;

public class Animal:IAnimal, ISeed<Animal>
{
    public virtual Guid AnimalId { get; set; } = Guid.NewGuid();
    public AnimalKind Kind { get; set; }
    public AnimalMood Mood { get; set; }

    public int Age { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public virtual IZoo Zoo { get; set; }

    #region Seeder
    public bool Seeded { get; set; } = false;

    public virtual Animal Seed (csSeedGenerator seeder)
    {
        Seeded = true;
        AnimalId = Guid.NewGuid();
        
        Kind = seeder.FromEnum<AnimalKind>();
        Mood = seeder.FromEnum<AnimalMood>();
        Age = seeder.Next(0, 11);

        Name = seeder.PetName;
        Description = seeder.LatinSentence;

        return this;
    }
    #endregion
    
}