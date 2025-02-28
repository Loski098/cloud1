using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Models;
using Models.DTO;
using Seido.Utilities.csSeedGenerator;

namespace DbModels;

[Table("Addresses", Schema = "supusr")]
public class AddressDbM : Address, ISeed<AddressDbM>
{   
    [Key]
    public override Guid AddressId { get; set; }

    public override AddressDbM Seed (csSeedGenerator rnd)
    {
        base.Seed (rnd);
        return this;
    }

    [NotMapped]
    public override List<IAttraction> Attractions{ get => AttractionDbM?.ToList<IAttraction>(); set => throw new NotImplementedException(); }

    [JsonIgnore]
    public List<AttractionDbM> AttractionDbM { get; set; }
    
}