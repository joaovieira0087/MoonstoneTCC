using FastReport.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using MoonstoneTCC.Areas.Admin.Services;
using MoonstoneTCC.Areas.Admin.Servicos;
using MoonstoneTCC.Context;
using MoonstoneTCC.Hubs;
using MoonstoneTCC.Models;
using MoonstoneTCC.Repositories;
using MoonstoneTCC.Repositories.Interfaces;
using MoonstoneTCC.Services;
using ReflectionIT.Mvc.Paging;
using System.Globalization;


var builder = WebApplication.CreateBuilder(args);

// DB
var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(connection));

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// FastReport
builder.Services.AddFastReport();
FastReport.Utils.RegisteredObjects.AddConnection(typeof(MsSqlDataConnection));

// Options (a seção no appsettings deve se chamar "ConfigurationPastaImagens")
builder.Services.Configure<ConfigurationImagens>(
    builder.Configuration.GetSection("ConfigurationPastaImagens"));

// DI
builder.Services.AddTransient<IJogoRepository, JogoRepository>();
builder.Services.AddTransient<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddTransient<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<ISeedUserRoleInitial, SeedUserRoleInitial>();
builder.Services.AddScoped<RelatorioVendasService>();
builder.Services.AddScoped<GraficoVendasService>();
builder.Services.AddScoped<RelatorioJogosService>();
builder.Services.AddScoped<RelatorioUsuariosService>();
builder.Services.AddScoped<LoggerAdminService>();
builder.Services.AddScoped<IFavoritoRepository, FavoritoRepository>();
builder.Services.AddScoped<ICarteiraService, CarteiraService>();
builder.Services.AddSingleton<ICepFreteService, CepFreteServiceLocal>();
builder.Services.AddScoped(sp => CarrinhoCompra.GetCarrinho(sp));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IGamificacaoService, GamificacaoService>();

builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("Admin", p => p.RequireRole("Admin"));
});

builder.Services.AddControllersWithViews();
builder.Services.AddPaging(o =>
{
    o.ViewName = "Bootstrap4";
    o.PageParameterName = "pageindex";
});
builder.Services.AddMemoryCache();
builder.Services.AddSession();
builder.Services.AddSignalR();

// Cultura (pt-BR)
var culture = new CultureInfo("pt-BR");
var supportedCultures = new List<CultureInfo> { culture };
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(culture),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseFastReport();

app.UseRequestLocalization(localizationOptions);
app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Rotas
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "categoriaFiltro",
    pattern: "Jogo/{action}/{categoria?}",
    defaults: new { Controller = "Jogo", action = "List" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<NotificacaoHub>("/notificacaoHub");

// Seed (roles -> users)
CriarPerfisUsuarios(app);

app.Run();

void CriarPerfisUsuarios(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<ISeedUserRoleInitial>();
    seeder.SeedRoles();  // primeiro roles
    seeder.SeedUsers();  // depois usuários
}
