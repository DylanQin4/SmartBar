using SmartBar.API.Middlewares;
using SmartBar.API.Services;
using SmartBar.Application.Common.Interfaces;
using SmartBar.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApplicationServices();
builder.AddInfrastructureServices();

builder.Services.AddScoped<IUser, CurrentUser>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}
else
{
    app.UseHsts();
}

app.UseMiddleware<CustomExceptionHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
