using DbRepos;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTO;

namespace Services;


public class ZooServiceDb : IZooService {

    private readonly ZooDbRepos _zooRepo;
    private readonly AnimalDbRepos _animalRepo;
    private readonly EmployeeDbRepos _employeeRepo;
    private readonly CreditCardDbRepos _creditcardRepo;
    private readonly ILogger<ZooServiceDb> _logger;    
    
    public ZooServiceDb(ZooDbRepos zooRepo, AnimalDbRepos animalRepo, EmployeeDbRepos employeeRepo, 
        CreditCardDbRepos creditcardRepo, ILogger<ZooServiceDb> logger)
    {
        _zooRepo = zooRepo;
        _animalRepo = animalRepo;
        _employeeRepo = employeeRepo;
        _creditcardRepo = creditcardRepo;
        _logger = logger;
    }

    public Task<ResponsePageDto<IZoo>> ReadZoosAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize) => _zooRepo.ReadItemsAsync(seeded, flat, filter, pageNumber, pageSize);
    public Task<ResponseItemDto<IZoo>> ReadZooAsync(Guid id, bool flat) => _zooRepo.ReadItemAsync(id, flat);
    public Task<ResponseItemDto<IZoo>> DeleteZooAsync(Guid id) => _zooRepo.DeleteItemAsync(id);
    public Task<ResponseItemDto<IZoo>> UpdateZooAsync(ZooCuDto item) => _zooRepo.UpdateItemAsync(item);
    public Task<ResponseItemDto<IZoo>> CreateZooAsync(ZooCuDto item) => _zooRepo.CreateItemAsync(item);

    public Task<ResponsePageDto<IAnimal>> ReadAnimalsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize) => _animalRepo.ReadItemsAsync(seeded, flat, filter, pageNumber, pageSize);
    public Task<ResponseItemDto<IAnimal>> ReadAnimalAsync(Guid id, bool flat) => _animalRepo.ReadItemAsync(id, flat);
    public Task<ResponseItemDto<IAnimal>> DeleteAnimalAsync(Guid id) => _animalRepo.DeleteItemAsync(id);
    public Task<ResponseItemDto<IAnimal>> UpdateAnimalAsync(AnimalCuDto item) => _animalRepo.UpdateItemAsync(item);
    public Task<ResponseItemDto<IAnimal>> CreateAnimalAsync(AnimalCuDto item) => _animalRepo.CreateItemAsync(item);

    public Task<ResponsePageDto<IEmployee>> ReadEmployeesAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize) => _employeeRepo.ReadItemsAsync(seeded, flat, filter, pageNumber, pageSize);
    public Task<ResponseItemDto<IEmployee>> ReadEmployeeAsync(Guid id, bool flat) => _employeeRepo.ReadItemAsync(id, flat);
    public Task<ResponseItemDto<IEmployee>> DeleteEmployeeAsync(Guid id) => _employeeRepo.DeleteItemAsync(id);
    public Task<ResponseItemDto<IEmployee>> UpdateEmployeeAsync(EmployeeCuDto item) => _employeeRepo.UpdateItemAsync(item);
    public Task<ResponseItemDto<IEmployee>> CreateEmployeeAsync(EmployeeCuDto item) => _employeeRepo.CreateItemAsync(item);


    public Task<ResponsePageDto<ICreditCard>> ReadCreditCardsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize) => _creditcardRepo.ReadItemsAsync(seeded, flat, filter, pageNumber, pageSize);
    public Task<ResponseItemDto<ICreditCard>> ReadCreditCardAsync(Guid id, bool flat) => _creditcardRepo.ReadItemAsync(id, flat);
    public Task<ResponseItemDto<ICreditCard>> DeleteCreditCardAsync(Guid id) => _creditcardRepo.DeleteItemAsync(id);
    public Task<ResponseItemDto<ICreditCard>> CreateCreditCardAsync(CreditCardCuDto item) => _creditcardRepo.CreateItemAsync(item);

    public Task<ResponsePageDto<IEmployee>> ReadEmployeesWithCCAsync(bool hasCreditCard, int pageNumber, int pageSize) => _creditcardRepo.ReadEmployeesWithCCAsync(hasCreditCard, pageNumber, pageSize);
    public Task<ResponseItemDto<ICreditCard>> ReadDecryptedCCAsync(Guid id) => _creditcardRepo.ReadDecryptedCCAsync(id);
}