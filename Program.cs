using Microsoft.EntityFrameworkCore;
using VendinhaBackend.Data;
using VendinhaBackend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<VendinhaDbContext>(options =>
    options.UseSqlite("Data Source=Database/vendinha.db"));
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<DividaService>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<VendinhaDbContext>();
    context.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
