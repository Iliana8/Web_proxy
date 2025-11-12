using System.Net.Http.Json;

Console.WriteLine("ClientTester -> Proxy @ http://localhost:8081");

var client = new HttpClient { BaseAddress = new Uri("http://localhost:8081/") };

var emp1 = await client.GetStringAsync("employee?id=1");
Console.WriteLine("GET employee?id=1 -> " + emp1);

var list = await client.GetStringAsync("employees?offset=0&limit=2");
Console.WriteLine("GET employees?offset=0&limit=2 -> " + list);

var payload = new { Id = 5, Name = "Eve", Department = "QA" };
var resp = await client.PutAsJsonAsync("employee", payload);
Console.WriteLine("PUT employee -> " + (int)resp.StatusCode + " " + await resp.Content.ReadAsStringAsync());

var list2 = await client.GetStringAsync("employees?offset=0&limit=10");
Console.WriteLine("GET employees?offset=0&limit=10 -> " + list2);

Console.WriteLine("Done.");