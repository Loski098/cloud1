using DbRepos;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTO;

namespace Services;

public class AddressServiceDb : IAddressService
{
    private readonly AddressDbRepos _addressDbRepos;
    private readonly ILogger<AddressServiceDb> _logger;    
    
    public AddressServiceDb(AddressDbRepos addressDbRepos, ILogger<AddressServiceDb> logger)
    {
        _addressDbRepos = addressDbRepos;
        _logger = logger;
    }

    public Task<ResponseItemDto<IAddress>> ReadAddressAsync(Guid id, bool flat) => _addressDbRepos.ReadItemAsync(id, flat);
    

    public Task<ResponsePageDto<IAddress>> ReadAddressesAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize) => _addressDbRepos.ReadItemsAsync(seeded, flat, filter, pageNumber, pageSize);
    
}