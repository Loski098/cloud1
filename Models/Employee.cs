using Configuration;
using Seido.Utilities.csSeedGenerator;

namespace Models;

public class Employee:IEmployee, ISeed<Employee>
{
    public virtual Guid EmployeeId { get; set; }
    public WorkRole Role { get; set; }
    
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    //Navigation properties
    public virtual List<IZoo> Zoos { get; set; }
    public virtual ICreditCard CreditCard { get; set; }

    #region Seeder
    public bool Seeded { get; set; } = false;

    public virtual Employee Seed (csSeedGenerator seeder)
    {
        Seeded = true;
        EmployeeId = Guid.NewGuid();
        
        Role = seeder.FromEnum<WorkRole>();
        FirstName = seeder.FirstName;
        LastName = seeder.LastName;
        Email = seeder.Email(FirstName, LastName);

        return this;
    }
    #endregion
    
}