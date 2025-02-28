using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Models;
using Models.DTO;
using Seido.Utilities.csSeedGenerator;

namespace DbModels;

[Table("Attractions", Schema = "supusr")]
public class AttractionDbM : Attraction, ISeed<AttractionDbM>
{   
    [Key]
    public override Guid AttractionId { get; set; }

    [NotMapped] // Not mapped betyder strunta i relationen
    public override ICategory Category { get => CategoryDbM; set => throw new NotImplementedException(); }

    [NotMapped]
    public override List<IComment> Comments { get => CommentsDbM?.ToList<IComment>(); set => throw new NotImplementedException(); }

    [NotMapped]
    public override IAddress Address { get => AddressDbM; set => throw new NotImplementedException(); }

    [JsonIgnore]
    [Required]
    public CategoryDbM CategoryDbM { get; set; }

    [JsonIgnore]
    [Required]
    public AddressDbM AddressDbM { get; set;}

    [JsonIgnore]
    public List<CommentDbM> CommentsDbM { get; set; }

    public AttractionDbM(){ }

    public override AttractionDbM Seed(csSeedGenerator rnd)
    {
        base.Seed (rnd);
        return this;
    }

    public AttractionDbM UpdateFromDTO(AttractionCuDto org)
    {
        if (org == null) return null;

        AttractionTitle = org.AttractionTitle;
        Description = org.Description;
        
        return this;
    }

    public AttractionDbM(AttractionCuDto org)
    {
        AttractionId = Guid.NewGuid();
        UpdateFromDTO(org);
    }
}