namespace Models;

public enum AnimalKind {Zebra, Elephant, Lion, Leopard, Gasell}
public enum AnimalMood { Happy, Hungry, Lazy, Sulky, Buzy, Sleepy };

public interface IAnimal
{
    public Guid AnimalId { get; set; }
    public AnimalKind Kind { get; set; }
    public AnimalMood Mood { get; set; }
    
    public int Age { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    //Navigation properties
    public IZoo Zoo { get; set; }
}