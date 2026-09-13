using System.Text;
using CromosList.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var jwtKey = builder.Configuration["JwtSettings:SecretKey"]!;
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"]!;
var jwtAudience = builder.Configuration["JwtSettings:Audience"]!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionStringRaw = builder.Configuration.GetConnectionString("DefaultConnection");
var connectionString = NormalizeConnectionString(connectionStringRaw);

Console.WriteLine($"ConnectionString: host={new Npgsql.NpgsqlConnectionStringBuilder(connectionString).Host} " +
                  $"(largo config={connectionStringRaw?.Length ?? 0})");

builder.Services.AddDbContext<CromosListDbContext>(options =>
    options.UseNpgsql(connectionString, npgsql => npgsql.EnableRetryOnFailure()));

var app = builder.Build();
app.UseCors("Angular");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CromosListDbContext>();

    for (var attempt = 1; attempt <= 5; attempt++)
    {
        try
        {
            db.Database.Migrate();
            Console.WriteLine("Migraciones aplicadas correctamente.");
            break;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Intento {attempt}/5 de conexión a BD falló: {ex.Message}");
            if (attempt == 5) throw;
            Thread.Sleep(TimeSpan.FromSeconds(5));
        }
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DISABLE_HTTPS_REDIRECT")))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("index.html");

app.Run();

static string NormalizeConnectionString(string? connectionString)
{
    if (string.IsNullOrWhiteSpace(connectionString))
        return string.Empty;

    var trimmed = connectionString.Trim()
        .Trim('"', '\'')
        .TrimEnd(';')
        .Trim();

    if (!trimmed.StartsWith("postgres", StringComparison.OrdinalIgnoreCase))
        return trimmed;

    var uri = new UriBuilder(trimmed);

    return $"Host={uri.Host};Port={uri.Port};Database={uri.Path.TrimStart('/')};" +
           $"Username={EscapeValue(uri.UserName)};Password={EscapeValue(uri.Password)};SSL Mode=Require";
}

static string EscapeValue(string value)
{
    var needsQuoting = value.IndexOfAny(new[] { ';', '=', '"', '\'' }) >= 0;
    return needsQuoting ? $"\"{value.Replace("\"", "\"\"")}\"" : value;
}
