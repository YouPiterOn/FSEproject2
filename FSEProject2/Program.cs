using Microsoft.AspNetCore.Builder;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
var app = builder.Build();
app.MapControllers();
app.Run();


public partial class Program
{
    public IConfiguration configRoot
    {
        get;
    }
    public Program(IConfiguration configuration)
    {
        configRoot = configuration;
    }
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddRazorPages();
    }
    public void Configure(WebApplication app, IWebHostEnvironment env)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.MapRazorPages();
        app.MapControllers();
        app.Run();
    }
}