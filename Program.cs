using System.Text;
using Microsoft.IdentityModel.Tokens;
using peluqueria.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Cookies";
    options.DefaultChallengeScheme = "Cookies";
})
.AddCookie("Cookies", options =>
{
    options.LoginPath = "/Usuarios/Login";
    options.LogoutPath = "/Usuarios/Logout";
    options.AccessDeniedPath = "/Home/Restringido";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
})

.AddJwtBearer("JwtBearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["TokenAuthentication:Issuer"],
        ValidAudience = builder.Configuration["TokenAuthentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["TokenAuthentication:SecretKey"]))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Empleado", policy => policy.RequireRole("Empleado", "Administrador"));
    options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador"));
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IRepositorioServicio, RepositorioServicio>();
builder.Services.AddScoped<IRepositorioTurno, RepositorioTurno>();
builder.Services.AddScoped<IRepositorioPago, RepositorioPago>();
builder.Services.AddScoped<IRepositorioServicio, RepositorioServicio>();
builder.Services.AddScoped<IRepositorioUsuario, RepositorioUsuario>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
