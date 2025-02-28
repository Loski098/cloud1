namespace Models;

public enum WorkRole {AnimalCare, Veterinarian, ProgramCoordinator, Maintenance, Management}

public interface IEmployee
{
    public Guid EmployeeId { get; set; }

    public WorkRole Role { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    //Navigation properties
    public List<IZoo> Zoos { get; set; }
}