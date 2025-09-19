using Microsoft.EntityFrameworkCore;
using PlanlaBakalim.Data;
using PlanlaBakalim.Service.Abstract;
using PlanlaBakalim.Service.Concrete;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".App.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromDays(1);
});

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"));
});
builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));
builder.Services.AddScoped(typeof(IBusinessService), typeof(BusinessService));
builder.Services.AddScoped(typeof(IAppointmentService), typeof(AppointmentService));


builder.Services.AddAuthentication("CustomerScheme")
    .AddCookie("CustomerScheme", options =>
    {
        options.LoginPath = "/Hesap/Girisyap";
        options.AccessDeniedPath = "/AccessDenied";
        options.Cookie.Name = "CustomerAuth";
        options.Cookie.MaxAge = TimeSpan.FromDays(7);
    })
    .AddCookie("BusinessScheme", options =>
    {
        options.LoginPath = "/BusinessPanel/Account/LogIn";
        options.AccessDeniedPath = "/BusinessPanel/Account/AccessDenied";
        options.Cookie.Name = "BusinessAuth";
        options.Cookie.MaxAge = TimeSpan.FromDays(7);
    })
    .AddCookie("AdminScheme", options =>
    {
        options.LoginPath = "/Admin/Auth/Login";
        options.AccessDeniedPath = "/Admin/Account/AccessDenied";
        options.Cookie.Name = "AdminAuth";
        options.Cookie.MaxAge = TimeSpan.FromDays(7);
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin").AddAuthenticationSchemes("AdminScheme"));

    options.AddPolicy("BusinessPolicy", policy =>
        policy.RequireRole("IsletmeSahibi").AddAuthenticationSchemes("BusinessScheme"));

    options.AddPolicy("CustomerPolicy", policy =>
        policy.RequireRole("Musteri").AddAuthenticationSchemes("CustomerScheme"));
});


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession(); 

app.UseAuthentication();
app.UseAuthorization(); 


app.MapControllerRoute(
  name: "businessPanel",
  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);
app.MapControllerRoute(
  name: "admin",
  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
