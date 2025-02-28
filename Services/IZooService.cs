using Models;
using Models.DTO;

namespace Services;

public interface IZooService {

    public Task<ResponsePageDto<IZoo>> ReadZoosAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize);
    public Task<ResponseItemDto<IZoo>> ReadZooAsync(Guid id, bool flat);
    public Task<ResponseItemDto<IZoo>> DeleteZooAsync(Guid id);
    public Task<ResponseItemDto<IZoo>> UpdateZooAsync(ZooCuDto item);
    public Task<ResponseItemDto<IZoo>> CreateZooAsync(ZooCuDto item);

    public Task<ResponsePageDto<IAnimal>> ReadAnimalsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize);
    public Task<ResponseItemDto<IAnimal>> ReadAnimalAsync(Guid id, bool flat);
    public Task<ResponseItemDto<IAnimal>> DeleteAnimalAsync(Guid id);
    public Task<ResponseItemDto<IAnimal>> UpdateAnimalAsync(AnimalCuDto item);
    public Task<ResponseItemDto<IAnimal>> CreateAnimalAsync(AnimalCuDto item);

    public Task<ResponsePageDto<IEmployee>> ReadEmployeesAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize);
    public Task<ResponseItemDto<IEmployee>> ReadEmployeeAsync(Guid id, bool flat);
    public Task<ResponseItemDto<IEmployee>> DeleteEmployeeAsync(Guid id);
    public Task<ResponseItemDto<IEmployee>> UpdateEmployeeAsync(EmployeeCuDto item);
    public Task<ResponseItemDto<IEmployee>> CreateEmployeeAsync(EmployeeCuDto item);

    public Task<ResponsePageDto<ICreditCard>> ReadCreditCardsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize);
    public Task<ResponseItemDto<ICreditCard>> ReadCreditCardAsync(Guid id, bool flat);
    public Task<ResponseItemDto<ICreditCard>> DeleteCreditCardAsync(Guid id);
    public Task<ResponseItemDto<ICreditCard>> CreateCreditCardAsync(CreditCardCuDto item);

    public Task<ResponsePageDto<IEmployee>> ReadEmployeesWithCCAsync(bool hasCreditCard, int pageNumber, int pageSize);
    public Task<ResponseItemDto<ICreditCard>> ReadDecryptedCCAsync(Guid id);
}