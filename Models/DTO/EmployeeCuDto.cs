using System;
using System.Diagnostics.Metrics;
using System.Reflection.Emit;
using System.Xml.Linq;

namespace Models.DTO;

//DTO is a DataTransferObject, can be instanstiated by the controller logic
//and represents a, fully instantiable, subset of the Database models
//for a specific purpose.

//These DTO are simplistic and used to Update and Create objects
public class EmployeeCuDto
{
    public Guid? EmployeeId { get; set; }

    public WorkRole Role { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    //Navigation properties
    public List<Guid> ZooIds { get; set; } = null;

    public EmployeeCuDto() { }
    public EmployeeCuDto(IEmployee org)
    {
        EmployeeId = org.EmployeeId;

        Role = org.Role;
        FirstName = org.FirstName;
        LastName = org.LastName;
        Email = org.Email;

        ZooIds = org.Zoos?.Select(z => z.ZooId).ToList();
    }
}