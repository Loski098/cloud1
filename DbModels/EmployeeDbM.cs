using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Models;
using Seido.Utilities.csSeedGenerator;
using Models.DTO;


namespace DbModels;
[Table("Employees", Schema = "supusr")]
public class EmployeeDbM : Employee, ISeed<EmployeeDbM>
{
    [Key]
    public override Guid EmployeeId { get; set; }

    #region adding more readability to an enum type in the database
    public virtual string strRole
    {
        get => Role.ToString();
        set { }  //set is needed by EFC to include in the database, so I make it to do nothing
    }
    #endregion
    
    [NotMapped]
    public override List<IZoo> Zoos { get => ZoosDbM?.ToList<IZoo>(); set => throw new NotImplementedException(); }

    [JsonIgnore]
    [Required]
    public  List<ZooDbM>  ZoosDbM { get; set; } = null;


    [NotMapped]
    public override ICreditCard CreditCard { get => CreditCardDbM; set => throw new NotImplementedException(); }

    [JsonIgnore]
    public  CreditCardDbM  CreditCardDbM { get; set; } = null;

    public override EmployeeDbM Seed (csSeedGenerator _seeder)
    {
        base.Seed (_seeder);
        return this;
    }

    public EmployeeDbM UpdateFromDTO(EmployeeCuDto org)
    {
        if (org == null) return null;

        Role = org.Role;
        FirstName = org.FirstName;
        LastName = org.LastName;
        Email = org.Email;

        return this;
    }

    public EmployeeDbM() { }
    public EmployeeDbM(EmployeeCuDto org)
    {
        EmployeeId = Guid.NewGuid();
        UpdateFromDTO(org);
    }
}
