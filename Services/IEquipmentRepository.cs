using Pr1.MinWebService.Domain;

namespace Pr1.MinWebService.Services;

public interface IEquipmentRepository
{
    IReadOnlyCollection<Equipment> GetAll();

    Equipment? GetById(Guid id);

    Equipment? GetByInventoryNumber(string inventoryNumber);

    Equipment Create(string name, string inventoryNumber, string category, decimal price, bool isOperational);
}