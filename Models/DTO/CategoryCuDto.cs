using System;
using System.Diagnostics.Metrics;
using System.Reflection.Emit;
using System.Xml.Linq;
using Microsoft.Extensions.Azure;

namespace Models.DTO;

public class CategoryCuDto
{

     public virtual Guid? CategoryId { get; set; }
    public CategoryNames Name { get; set; } // Gör detta till en enum sen?

    public List<Guid> AttractionsId { get; set; } = null;

    public CategoryCuDto()
    {
        
    }

    public CategoryCuDto(ICategory org)
    {
        Name = org.Name;
        CategoryId = org.CategoryId;

        AttractionsId = org.Attractions?.Select(a => a.AttractionId).ToList();
    }

}