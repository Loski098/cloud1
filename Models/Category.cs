

using Models;
using Seido.Utilities.csSeedGenerator;

public class Category : ICategory, ISeed<Category>
{
    public virtual Guid CategoryId { get; set; }
    public CategoryNames Name { get; set; } // Gör detta till en enum sen?

    

    public virtual List<IAttraction> Attractions { get; set; }
    public bool Seeded {get; set;} = false;

    public virtual Category Seed(csSeedGenerator seedGenerator)
    {
        CategoryId = Guid.NewGuid();
        Name = seedGenerator.FromEnum<CategoryNames>();
        Seeded = true;

        return this;
    }

    // Bygg en seed när vi har definerat om vi skall ha enum eller inte (antar det doe)
}