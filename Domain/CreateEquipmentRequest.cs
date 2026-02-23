namespace Pr1.MinWebService.Domain;

public sealed record CreateEquipmentRequest(string Name, string InventoryNumber, string Category, decimal Price, bool IsOperational);