using VendinhaBackend.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<Database>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<Database>();
    database.CriarTabelas();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
