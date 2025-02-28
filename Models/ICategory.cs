

using Models;

public enum CategoryNames
{
    Restaurant,
    Museum,
    Park,
    AmusementPark,
    Zoo,
    Aquarium,
    Theater,
    Landmark,
    HistoricalSite,
    Beach,
    ShoppingMall,
    Stadium,
    ArtGallery,
    BotanicalGarden,
    Casino,
    ConcertHall
}

public interface ICategory {

    public Guid CategoryId { get; set; }

    public CategoryNames Name { get; set; }

    public List<IAttraction> Attractions { get; set; }

}