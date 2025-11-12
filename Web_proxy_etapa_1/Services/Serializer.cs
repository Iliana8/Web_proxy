using System.Text;
using System.Xml.Serialization;
using System.Text.Json;

namespace Lab2_Etapa1_DW_Server_CSharp.Services;

public static class Serializer
{
    public static async Task<T?> ReadBody<T>(HttpRequest req)
    {
        using var reader = new StreamReader(req.Body, Encoding.UTF8);
        var body = await reader.ReadToEndAsync();
        var contentType = req.ContentType?.ToLower() ?? "json";

        try
        {
            if (contentType.Contains("xml"))
            {
                var xml = new XmlSerializer(typeof(T));
                using var ms = new MemoryStream(Encoding.UTF8.GetBytes(body));
                return (T?)xml.Deserialize(ms);
            }
            else
            {
                return JsonSerializer.Deserialize<T>(body);
            }
        }
        catch { return default; }
    }

    public static IResult FormatResponse(object obj, string? format, int code = 200)
    {
        format ??= "json";
        if (format.ToLower().Contains("xml"))
        {
            var xml = new XmlSerializer(obj.GetType());
            using var sw = new StringWriter();
            xml.Serialize(sw, obj);
            return Results.Content(sw.ToString(), "application/xml", Encoding.UTF8, code);
        }
        var json = JsonSerializer.Serialize(obj);
        return Results.Content(json, "application/json", Encoding.UTF8, code);
    }
}
