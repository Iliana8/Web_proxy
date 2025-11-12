using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var store = new ConcurrentDictionary<int, Employee>(new[]
{
    new KeyValuePair<int, Employee>(1, new Employee{ Id=1, Name="Alice", Department="HR"}),
    new KeyValuePair<int, Employee>(2, new Employee{ Id=2, Name="Bob", Department="IT"}),
    new KeyValuePair<int, Employee>(3, new Employee{ Id=3, Name="Carol", Department="QA"}),
    new KeyValuePair<int, Employee>(4, new Employee{ Id=4, Name="Dave", Department="Ops"}),
});

app.MapGet("/employee", (int id) =>
{
    return store.TryGetValue(id, out var emp) ? Results.Json(emp) : Results.NotFound(new { error = "Employee not found" });
});

app.MapGet("/employees", (int offset, int limit) =>
{
    var list = store.Values.OrderBy(e => e.Id).Skip(offset).Take(limit).ToList();
    return Results.Json(new { total = store.Count, offset, limit, data = list });
});

app.MapPut("/employee", async (HttpContext ctx) =>
{
    var emp = await ctx.Request.ReadFromJsonAsync<Employee>();
    if (emp is null || emp.Id <= 0 || string.IsNullOrWhiteSpace(emp.Name))
        return Results.BadRequest(new { error = "Invalid employee payload" });

    store.AddOrUpdate(emp.Id, emp, (k, _) => emp);
    return Results.Json(emp);
});

app.Run(Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? "http://localhost:0");

public record Employee
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string Department { get; init; } = "";
}