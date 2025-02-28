using Configuration;
using Seido.Utilities.csSeedGenerator;

namespace Models;

public class Comment : IComment, ISeed<Comment>
{
    public virtual Guid CommentId { get; set; } = Guid.NewGuid();
    public CommentSentiment Sentiment { get; set; }
    public string Content { get; set; }

    public virtual IAttraction Attraction { get; set; }

    #region Seeder
    public bool Seeded { get; set; } = false;

    public virtual Comment Seed (csSeedGenerator seeder)
    {
        Seeded = true;
        CommentId = Guid.NewGuid();
        
        Sentiment = seeder.FromEnum<CommentSentiment>();
        Content = seeder.LatinSentence;

        return this;
    }
    #endregion
}