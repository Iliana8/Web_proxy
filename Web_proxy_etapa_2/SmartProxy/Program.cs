using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration["Backends:0"] ??= "http://localhost:8082";
builder.Configuration["Backends:1"] ??= "http://localhost:8083";

builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();

var app = builder.Build();

var backends = builder.Configuration
    .GetSection("Backends")
    .GetChildren()
    .Select(c => c?.Value ?? "http://localhost:8082")
    .Where(v => !string.IsNullOrWhiteSpace(v))
    .ToArray();

if (backends.Length == 0)
    backends = new[] { "http://localhost:8082" };

int rrIndex = 0;
string NextBackend() => backends[Interlocked.Increment(ref rrIndex) % backends.Length];

var cache = app.Services.GetRequiredService<IMemoryCache>();
var ttl = TimeSpan.FromSeconds(30);

async Task Forward(HttpContext ctx, string? overrideBackend = null, bool useCache = true)
{
    var client = ctx.RequestServices.GetRequiredService<IHttpClientFactory>().CreateClient();
    var targetBase = overrideBackend ?? NextBackend();
    var targetUri = new Uri(new Uri(targetBase), ctx.Request.Path + ctx.Request.QueryString);

    var cacheKey = $"{ctx.Request.Method}:{targetUri}";

    if (useCache && string.Equals(ctx.Request.Method, "GET", StringComparison.OrdinalIgnoreCase))
    {
        if (cache.TryGetValue(cacheKey, out string cached))
        {
            ctx.Response.ContentType = "application/json";
            await ctx.Response.WriteAsync(cached ?? string.Empty);
            return;
        }
    }

    using var req = new HttpRequestMessage(new HttpMethod(ctx.Request.Method), targetUri);

    if (ctx.Request.Body != null &&
        (ctx.Request.Method == "POST" || ctx.Request.Method == "PUT" || ctx.Request.Method == "PATCH"))
    {
        req.Content = new StreamContent(ctx.Request.Body);
        if (!string.IsNullOrWhiteSpace(ctx.Request.ContentType))
            req.Content.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(ctx.Request.ContentType!);
    }

    using var resp = await client.SendAsync(req, ctx.RequestAborted);
    var body = await resp.Content.ReadAsStringAsync(ctx.RequestAborted);

    if (useCache &&
        string.Equals(ctx.Request.Method, "GET", StringComparison.OrdinalIgnoreCase) &&
        (int)resp.StatusCode >= 200 && (int)resp.StatusCode < 300)
    {
        cache.Set(cacheKey, body, ttl);
    }

    ctx.Response.StatusCode = (int)resp.StatusCode;
    ctx.Response.ContentType = resp.Content.Headers.ContentType?.ToString() ?? "application/json";
    await ctx.Response.WriteAsync(body ?? string.Empty, ctx.RequestAborted);
}

app.MapGet("/employee", async ctx => await Forward(ctx, useCache: true));
app.MapGet("/employees", async ctx => await Forward(ctx, useCache: true));
app.MapPut("/employee", async ctx =>
{
    if (cache is MemoryCache mem) mem.Compact(1.0);
    await Forward(ctx, useCache: false);
});
app.Map("/{**_path}", async ctx =>
{
    _ = ctx;
    var useCache = string.Equals(ctx.Request.Method, "GET", StringComparison.OrdinalIgnoreCase);
    await Forward(ctx, useCache: useCache);
});

app.Run("http://localhost:8081");