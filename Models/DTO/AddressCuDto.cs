using System;
using System.Diagnostics.Metrics;
using System.Reflection.Emit;
using System.Xml.Linq;
using Microsoft.Extensions.Azure;

namespace Models.DTO;

//DTO is a DataTransferObject, can be instanstiated by the controller logic
//and represents a, fully instantiable, subset of the Database models
//for a specific purpose.

//These DTO are simplistic and used to Update and Create objects
public class AddressCuDto
{
    public virtual Guid AddressId { get; set;}
    public string City { get; set;}
    public string Country { get; set; }

    public virtual List<Guid> AttractionsId{ get; set; } = null;

    public AddressCuDto() { }
    public AddressCuDto(IAddress org)
    {
        AddressId = org.AddressId;
        City = org.City;
        Country = org.Country;

        AttractionsId = org.Attractions?.Select(x => x.AttractionId).ToList();
    }
}