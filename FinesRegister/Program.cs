using Microsoft.EntityFrameworkCore;
using FinesRegister.Models;
using Microsoft.AspNetCore.Identity;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<FinesRegisterContext>() // Убедитесь, что здесь указан ваш контекст базы данных
    .AddDefaultTokenProviders();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddDbContextPool<FinesRegisterContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
// Добавляем службы контроллеров с представлениями
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Конфигурируем HTTP-конвейер
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<FinesRegisterContext>();

    // Создание базы данных, если она ещё не создана
    context.Database.EnsureCreated();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();