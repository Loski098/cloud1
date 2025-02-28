using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Configuration;
using Newtonsoft.Json;
using Seido.Utilities.csSeedGenerator;

namespace Models;

public class CreditCard : ICreditCard, ISeed<CreditCard>
{
    public virtual Guid CreditCardId { get; set; }

    public CardIssues Issuer { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Number { get; set; }
    public string ExpirationYear { get; set; }
    public string ExpirationMonth { get; set; }

    [JsonIgnore]
    public string EnryptedToken {get; set; }
    

    //Navigation properties
    public virtual IEmployee Employee { get; set; }

    #region Seeder
    public bool Seeded { get; set; } = false;

    public virtual CreditCard Seed (csSeedGenerator seeder)
    {
        Seeded = true;
        CreditCardId = Guid.NewGuid();
        
        Issuer = seeder.FromEnum<CardIssues>();

        Number = $"{seeder.Next(2222, 9999)}-{seeder.Next(2222, 9999)}-{seeder.Next(2222, 9999)}-{seeder.Next(2222, 9999)}";
        ExpirationYear = $"{seeder.Next(25, 32)}";
        ExpirationMonth = $"{seeder.Next(01, 13):D2}";
        return this;
    }
    #endregion

    public CreditCard EnryptAndObfuscate (Func<CreditCard, string> encryptor)
    {
        this.EnryptedToken = encryptor(this);

        this.FirstName = Regex.Replace(FirstName, "(?<=.{1}).", "*");
        this.LastName = Regex.Replace(LastName, "(?<=.{1}).", "*");

        string pattern = @"\b(\d{4}[-\s]?)(\d{4}[-\s]?)(\d{4}[-\s]?)(\d{4})\b";
        string replacement = "$1**** **** **** $4"; 
        this.Number =  Regex.Replace(Number, pattern, replacement);

        this.ExpirationYear = "**";
        this.ExpirationMonth = "**";
        
        return this;
    }

    public CreditCard Decrypt (Func<string, CreditCard> decryptor)
    {
        return decryptor(this.EnryptedToken);
    }
}