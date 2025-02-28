namespace Models;

public enum CommentSentiment { Positive, Negative, Neutral };

public interface IComment
{
    public Guid CommentId { get; set; }
    public CommentSentiment Sentiment { get; set; }
    public string Content { get; set; }

    //Navigation properties
    public IAttraction Attraction { get; set; }
}