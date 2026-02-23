namespace Pr1.MinWebService.Domain;

public sealed record Equipment(Guid Id, string Name, string InventoryNumber, string Category, decimal Price, bool IsOperational);