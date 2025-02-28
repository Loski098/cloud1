using Configuration;
using Newtonsoft.Json;
using Seido.Utilities.csSeedGenerator;

namespace Models;

public class Attraction : IAttraction, ISeed<Attraction>
{
        [JsonProperty(Order = 1)]
        public virtual Guid AttractionId { get; set; }

        [JsonProperty(Order = 2)]
        public string AttractionTitle { get; set; }

        [JsonProperty(Order = 3)]
        public string Description { get; set; }

        [JsonProperty(Order = 4)]
        public bool Seeded { get; set; } = false;

        [JsonProperty(Order = 5)]
        public virtual ICategory Category { get; set; }

        [JsonProperty(Order = 6)]
        public virtual IAddress Address { get; set; }

        [JsonProperty(Order = 7)]
        public virtual List<IComment> Comments { get; set; }

    public virtual Attraction Seed (csSeedGenerator seeder)
    {
        AttractionId = Guid.NewGuid();
        AttractionTitle = seeder.AttractionTitle;
        Description = seeder.LatinSentence;
        Seeded = true;

        return this;
    }
}