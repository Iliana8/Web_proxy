using System.Collections.Concurrent;
using Lab2_Etapa1_DW_Server_CSharp.Models;

namespace Lab2_Etapa1_DW_Server_CSharp.Data;

public class EmployeeRepository
{
    private readonly ConcurrentDictionary<int, Employee> _store = new();

    public Employee? Get(int id) => _store.TryGetValue(id, out var e) ? e : null;

    public void AddOrUpdate(int id, string name, string department) =>
        _store[id] = new Employee { Id = id, Name = name, Department = department };

    public IEnumerable<Employee> List(int offset, int limit) =>
        _store.Values.OrderBy(e => e.Id).Skip(offset).Take(limit);

    public int Size() => _store.Count;
}
