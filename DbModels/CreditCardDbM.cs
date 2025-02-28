using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

using Models;
using Seido.Utilities.csSeedGenerator;
using Models.DTO;


namespace DbModels;
[Table("CreditCards", Schema = "supusr")]
public class CreditCardDbM : CreditCard, ISeed<CreditCardDbM>
{
    [Key]
    public override Guid CreditCardId { get; set; }

    #region adding more readability to an enum type in the database
    public virtual string strIssuer
    {
        get => Issuer.ToString();
        set { }  //set is needed by EFC to include in the database, so I make it to do nothing
    }
    #endregion
    
    //In a one-to-one relationship, you needed to establish that the Foreign Key should in this table, CreditCards
    //Connecting below FK above with EmployeeDbM.EmployeeId
    [JsonIgnore]
    public virtual Guid EmployeeId { get; set; }

    [NotMapped]
    public override IEmployee Employee { get => EmployeeDbM; set => throw new NotImplementedException(); }

    [JsonIgnore]
    [Required]
    [ForeignKey ("EmployeeId")] //Connecting FK above with EmployeeDbM.EmployeeId
    public  EmployeeDbM  EmployeeDbM { get; set; } = null;


    public override CreditCardDbM Seed (csSeedGenerator _seeder)
    {
        base.Seed (_seeder);
        return this;
    }

    public CreditCardDbM UpdateFromDTO(CreditCardCuDto org)
    {
        if (org == null) return null;

        Issuer = org.Issuer;
        FirstName = org.FirstName;
        LastName = org.LastName;
        Number = org.Number;

        ExpirationYear = org.ExpirationYear;
        ExpirationMonth = org.ExpirationMonth;

        return this;
    }

    public CreditCardDbM() { }
    public CreditCardDbM(CreditCardCuDto org)
    {
        CreditCardId = Guid.NewGuid();
        UpdateFromDTO(org);
    }
}
