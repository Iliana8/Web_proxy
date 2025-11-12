using Lab2_Etapa1_DW_Server_CSharp.Data;
using Lab2_Etapa1_DW_Server_CSharp.Models;
using Lab2_Etapa1_DW_Server_CSharp.Services;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var repo = new EmployeeRepository();
repo.AddOrUpdate(1, "Alice", "Engineering");
repo.AddOrUpdate(2, "Bob", "HR");
repo.AddOrUpdate(3, "Carol", "Sales");
repo.AddOrUpdate(4, "Dave", "Engineering");

app.MapGet("/employee", (int id, string? format) =>
{
    var emp = repo.Get(id);
    if (emp is null) return Results.NotFound("Not found");
    return Serializer.FormatResponse(emp, format);
});

app.MapPut("/employee", async (HttpRequest req, string? format) =>
{
    var emp = await Serializer.ReadBody<Employee>(req);
    if (emp is null) return Results.BadRequest("Invalid payload");
    repo.AddOrUpdate(emp.Id, emp.Name, emp.Department);
    return Serializer.FormatResponse(emp, format, 201);
});

app.MapPost("/employee", async (HttpRequest req, string? format) =>
{
    var emp = await Serializer.ReadBody<Employee>(req);
    if (emp is null) return Results.BadRequest("Invalid payload");
    repo.AddOrUpdate(emp.Id, emp.Name, emp.Department);
    return Serializer.FormatResponse(emp, format, 201);
});

app.MapGet("/employees", (int? offset, int? limit, string? format) =>
{
    int off = offset ?? 0;
    int lim = limit ?? 10;
    var list = repo.List(off, lim);
    var page = new EmployeesPage(off, lim, repo.Size(), list);
    return Serializer.FormatResponse(page, format);
});

app.Run("http://localhost:8081");
