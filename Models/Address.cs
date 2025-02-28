using Configuration;
using Seido.Utilities.csSeedGenerator;

namespace Models;

public class Address : IAddress, ISeed<Address>
{
    public virtual Guid AddressId { get; set;}
    public string City { get; set;}
    public string Country { get; set; }
    public bool Seeded { get; set; } = false;

    public virtual List<IAttraction> Attractions{ get; set; }

    public virtual Address Seed(csSeedGenerator rnd)
    {
        this.AddressId = Guid.NewGuid();
        this.Country = rnd.Country;
        this.City = rnd.City(Country);


        return this;
    }
}