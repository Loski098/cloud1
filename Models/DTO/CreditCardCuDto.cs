using System;
using System.Diagnostics.Metrics;
using System.Reflection.Emit;
using System.Xml.Linq;

namespace Models.DTO;

//DTO is a DataTransferObject, can be instanstiated by the controller logic
//and represents a, fully instantiable, subset of the Database models
//for a specific purpose.

//These DTO are simplistic and used to Update and Create objects
public class CreditCardCuDto
{
    public Guid? CreditCardId { get; set; }
    public CardIssues Issuer { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Number { get; set; }
    public string ExpirationYear { get; set; }
    public string ExpirationMonth { get; set; }

    //Navigation properties
    public Guid EmployeeId { get; set; }

    public CreditCardCuDto() { }
    public CreditCardCuDto(ICreditCard org)
    {
        CreditCardId = org.CreditCardId;

        Issuer = org.Issuer;
        FirstName = org.FirstName;
        LastName = org.LastName;
        Number = org.Number;

        ExpirationYear = org.ExpirationYear;
        ExpirationMonth = org.ExpirationMonth;

        EmployeeId = org.Employee.EmployeeId;
    }
}