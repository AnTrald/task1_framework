using System.Collections.Concurrent;
using Pr1.MinWebService.Domain;
using Pr1.MinWebService.Errors;

namespace Pr1.MinWebService.Services;

public sealed class InMemoryEquipmentRepository : IEquipmentRepository
{
    private readonly ConcurrentDictionary<Guid, Equipment> _items = new();
    private readonly ConcurrentDictionary<string, Guid> _inventoryNumbers = new();

    public IReadOnlyCollection<Equipment> GetAll()
        => _items.Values
            .OrderBy(x => x.Name)
            .ToArray();

    public Equipment? GetById(Guid id)
        => _items.TryGetValue(id, out var item) ? item : null;

    public Equipment? GetByInventoryNumber(string inventoryNumber)
    {
        if (_inventoryNumbers.TryGetValue(inventoryNumber, out var id))
            return GetById(id);
        return null;
    }

    public Equipment Create(string name, string inventoryNumber, string category, decimal price, bool isOperational)
    {
        if (_inventoryNumbers.ContainsKey(inventoryNumber))
            throw new ValidationException("Инвентарный номер должен быть уникальным");

        var id = Guid.NewGuid();
        var item = new Equipment(id, name, inventoryNumber, category, price, isOperational);

        _items[id] = item;
        _inventoryNumbers[inventoryNumber] = id;
        return item;
    }
}