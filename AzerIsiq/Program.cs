using AzerIsiq.DbInit;
using AzerIsiq.Extensions;
using AzerIsiq.Extensions.Exceptions;
using AzerIsiq.Extensions.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCorsPolicy()
    .AddDatabaseConfiguration(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddApplicationServices();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var kestrelPort = builder.Configuration.GetValue<int>("Kestrel:EndpointPort");

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(kestrelPort);
});

Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"DBConnection: {builder.Configuration.GetConnectionString("DBConnection")}");
Console.WriteLine($"Kestrel Port: {kestrelPort}");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    dbInitializer.Initialize();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<ValidationExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseMiddleware<BlockedUserMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();