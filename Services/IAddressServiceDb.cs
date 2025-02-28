using Models;
using Models.DTO;

namespace Services;

public interface IAddressService {

    public Task<ResponsePageDto<IAddress>> ReadAddressesAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize);
    public Task<ResponseItemDto<IAddress>> ReadAddressAsync(Guid id, bool flat);
}