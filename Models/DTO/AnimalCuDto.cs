using System;
using System.Diagnostics.Metrics;
using System.Reflection.Emit;
using System.Xml.Linq;

namespace Models.DTO;

//DTO is a DataTransferObject, can be instanstiated by the controller logic
//and represents a, fully instantiable, subset of the Database models
//for a specific purpose.

//These DTO are simplistic and used to Update and Create objects
public class AnimalCuDto
{
    public virtual Guid? AnimalId { get; set; }

    public AnimalKind Kind { get; set; }
    public AnimalMood Mood { get; set; }
    
    public int Age { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public virtual Guid? ZooId { get; set; } = null;
    public AnimalCuDto() { }
    public AnimalCuDto(IAnimal org)
    {
        AnimalId = org.AnimalId;

        Kind = org.Kind;
        Mood = org.Mood;
        Age = org.Age;
        Name = org.Name;
        Description = org.Description;

        ZooId = org?.Zoo?.ZooId;
    }
}