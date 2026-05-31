using VendinhaBackend.Data;
using VendinhaBackend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<Database>();
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<DividaService>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
