using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Models;
using Seido.Utilities.csSeedGenerator;
using Models.DTO;


namespace DbModels;
[Table("Animals", Schema = "supusr")]
public class AnimalDbM : Animal, ISeed<AnimalDbM>
{
    [Key]
    public override Guid AnimalId { get; set; }

    #region adding more readability to an enum type in the database
    public virtual string strKind
    {
        get => Kind.ToString();
        set { }  //set is needed by EFC to include in the database, so I make it to do nothing
    }
    public virtual string strMood
    {
        get => Mood.ToString();
        set { } //set is needed by EFC to include in the database, so I make it to do nothing
    }
    #endregion
    
    [NotMapped]
    public override IZoo Zoo { get => ZooDbM; set => throw new NotImplementedException(); }

    [JsonIgnore]
    [Required]
    public  ZooDbM ZooDbM { get; set; }

    public override AnimalDbM Seed (csSeedGenerator _seeder)
    {
        base.Seed (_seeder);
        return this;
    }

    public AnimalDbM UpdateFromDTO(AnimalCuDto org)
    {
        if (org == null) return null;

        Kind = org.Kind;
        Mood = org.Mood;
        Age = org.Age;
        Name = org.Name;
        Description = org.Description;

        return this;
    }

    public AnimalDbM() { }
    public AnimalDbM(AnimalCuDto org)
    {
        AnimalId = Guid.NewGuid();
        UpdateFromDTO(org);
    }
}
