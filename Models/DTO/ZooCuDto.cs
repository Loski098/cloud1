using System;
using System.Diagnostics.Metrics;
using System.Reflection.Emit;
using System.Xml.Linq;

namespace Models.DTO;

//DTO is a DataTransferObject, can be instanstiated by the controller logic
//and represents a, fully instantiable, subset of the Database models
//for a specific purpose.

//These DTO are simplistic and used to Update and Create objects
public class ZooCuDto
{
    public virtual Guid? ZooId { get; set; }

    public string City { get; set; }
    public string Country { get; set; }
    public string Name { get; set; }

    public virtual List<Guid> AnimalsId { get; set; } = null;
    public virtual List<Guid> EmployeesId { get; set; } = null;

    public ZooCuDto() { }
    public ZooCuDto(IZoo org)
    {
        ZooId = org.ZooId;
        Name = org.Name;
        Country = org.Country;
        City = org.City;
        
        AnimalsId = org.Animals?.Select(i => i.AnimalId).ToList();
        EmployeesId = org.Employees?.Select(e => e.EmployeeId).ToList();
    }
}