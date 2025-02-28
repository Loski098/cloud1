using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Models;
using Seido.Utilities.csSeedGenerator;
using Models.DTO;

namespace DbModels;

[Table("Zoos", Schema = "supusr")]
public class ZooDbM : Zoo, ISeed<ZooDbM>
{
    [Key]
    public override Guid ZooId { get; set; }


    [NotMapped]
    public override List<IAnimal> Animals { get => AnimalsDbM?.ToList<IAnimal>(); set => throw new NotImplementedException(); }

    [JsonIgnore]
    public List<AnimalDbM> AnimalsDbM { get; set; }


    [NotMapped]
    public override List<IEmployee> Employees { get => EmployeesDbM?.ToList<IEmployee>(); set => throw new NotImplementedException(); }

    [JsonIgnore]
    public List<EmployeeDbM> EmployeesDbM { get; set; }


    public override ZooDbM Seed (csSeedGenerator _seeder)
     {
         base.Seed (_seeder);
         return this;
    }

    public ZooDbM UpdateFromDTO(ZooCuDto org)
    {
        if (org == null) return null;

        City = org.City;
        Country = org.Country;
        Name = org.Name;

        return this;
    }


    public ZooDbM() { }
    public ZooDbM(ZooCuDto org)
    {
        ZooId = Guid.NewGuid();
        UpdateFromDTO(org);
    }
}