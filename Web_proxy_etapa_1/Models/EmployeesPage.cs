namespace Lab2_Etapa1_DW_Server_CSharp.Models;

public class EmployeesPage
{
    public int Offset { get; set; }
    public int Limit { get; set; }
    public int Total { get; set; }
    public IEnumerable<Employee> Data { get; set; }

    public EmployeesPage(int offset, int limit, int total, IEnumerable<Employee> data)
    {
        Offset = offset; Limit = limit; Total = total; Data = data;
    }
}
