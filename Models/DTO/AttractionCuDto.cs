﻿using System;
using System.Diagnostics.Metrics;
using System.Reflection.Emit;
using System.Xml.Linq;
using Microsoft.Extensions.Azure;

namespace Models.DTO;

//DTO is a DataTransferObject, can be instanstiated by the controller logic
//and represents a, fully instantiable, subset of the Database models
//for a specific purpose.

//These DTO are simplistic and used to Update and Create objects
public class AttractionCuDto
{
    public virtual Guid? AttractionId { get; set; }

    public string AttractionTitle { get; set; }

    public string Description { get; set; }

    public virtual Guid CategoryId { get; set; }

    public virtual Guid AddressId { get; set; }

    public virtual List<Guid> CommentsId { get; set; } = null;


    public AttractionCuDto() { }
    public AttractionCuDto(IAttraction org)
    {
        AttractionId = org.AttractionId;
        AttractionTitle = org.AttractionTitle;
        Description = org.Description;

        CategoryId = org.Category.CategoryId;
        AddressId = org.Address.AddressId;
        CommentsId = org.Comments?.Select(i => i.CommentId).ToList();
    }
}