﻿using System;
using System.Diagnostics.Metrics;
using System.Reflection.Emit;
using System.Xml.Linq;

namespace Models.DTO;

public class CommentCuDto
{
    public virtual Guid? CommentId { get; set; }

    public CommentSentiment Sentiment { get; set; }
    public string Content { get; set; }

    public virtual Guid? AttractionId { get; set; } = null;
    public CommentCuDto() { }
    public CommentCuDto(IComment org)
    {
        CommentId = org.CommentId;

        Sentiment = org.Sentiment;
        Content = org.Content;

        AttractionId = org?.Attraction?.AttractionId;
    }
}