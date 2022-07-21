using Microsoft.Extensions.Localization;
using System.Reflection;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<IWeightDataService, WeightMongoDB>();
builder.Services.AddSingleton<ControlerMqttHandler>();
builder.Services.AddSingleton<AppPath>(() => Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
    .AddSingleton<IFuelService, FuelService>()
    .AddSingleton<MongoDatabaseHandler>()
    .AddSingleton<LocalizerService>()
    .AddSingleton<IStringLocalizerFactory, LocalizerFactory>()
    .AddTransient<IStringLocalizer>(f => f.GetService<IStringLocalizerFactory>().Create(typeof(Program)))
    ;



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

public delegate string AppPath();
