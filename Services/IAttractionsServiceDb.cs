using Models;
using Models.DTO;

namespace Services;

public interface IAttractionService {

    public Task<ResponsePageDto<IAttraction>> ReadAttractionsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize);
    public Task<ResponseItemDto<IAttraction>> ReadAttractionAsync(Guid id, bool flat);
    public Task<ResponseItemDto<IAttraction>> UpdateAttractionAsync(AttractionCuDto item);
    public Task <ResponseItemDto<IAttraction>> DeleteAttractionAsync(Guid id);
    public Task<ResponseItemDto<IAttraction>> PostAttractionAsync(AttractionCuDto itemDto);
}