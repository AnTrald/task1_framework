using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;
using Pr1.MinWebService.Domain;
using Pr1.MinWebService.Errors;
using Pr1.MinWebService.Middlewares;
using Pr1.MinWebService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddSingleton<IEquipmentRepository, InMemoryEquipmentRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<RequestIdMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<TimingAndLogMiddleware>();

app.MapGet("/api/equipment", (IEquipmentRepository repo) =>
{
    return Results.Ok(repo.GetAll());
});

app.MapGet("/api/equipment/{id:guid}", (Guid id, IEquipmentRepository repo) =>
{
    var item = repo.GetById(id);
    if (item is null)
        throw new NotFoundException("Оборудование не найдено");

    return Results.Ok(item);
});

app.MapGet("/api/equipment/inventory/{inventoryNumber}", (string inventoryNumber, IEquipmentRepository repo) =>
{
    var item = repo.GetByInventoryNumber(inventoryNumber);
    if (item is null)
        throw new NotFoundException("Оборудование с таким инвентарным номером не найдено");

    return Results.Ok(item);
});

app.MapPost("/api/equipment", (HttpContext ctx, CreateEquipmentRequest request, IEquipmentRepository repo) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
        throw new ValidationException("Поле name не должно быть пустым");

    if (request.Name.Length > 200)
        throw new ValidationException("Поле name не должно превышать 200 символов");

    if (string.IsNullOrWhiteSpace(request.InventoryNumber))
        throw new ValidationException("Поле inventoryNumber не должно быть пустым");

    if (string.IsNullOrWhiteSpace(request.Category))
        throw new ValidationException("Поле category не должно быть пустым");

    if (request.Price <= 0)
        throw new ValidationException("Поле price не может быть меньше или равна нулю");

    var created = repo.Create(
        request.Name.Trim(),
        request.InventoryNumber.Trim(),
        request.Category.Trim(),
        request.Price,
        request.IsOperational
    );

    var location = $"/api/equipment/{created.Id}";
    ctx.Response.Headers.Location = location;

    return Results.Created(location, created);
});

app.Run();

public partial class Program { }