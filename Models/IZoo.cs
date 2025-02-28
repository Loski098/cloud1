namespace Models;

public interface IZoo
{
    public Guid ZooId { get; set;}
    public string City { get; set; }
    public string Country { get; set; }
    public string Name { get; set; }

    //Navigation properties
    public List<IAnimal> Animals { get; set; }
    public List<IEmployee> Employees { get; set; }
}