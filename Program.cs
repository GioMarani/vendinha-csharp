using VendinhaBackend.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<Database>();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
