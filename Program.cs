using hotel_reservation_system.libs;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddTransient<MySqlDatabase>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();

    return new MySqlDatabase(configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

app.UseMiddleware<SecuredMiddleware>();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
