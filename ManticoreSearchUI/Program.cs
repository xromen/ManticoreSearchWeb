using ManticoreSearch.Business.Services;
using ManticoreSearch.Data;
using ManticoreSearch.UI.Components;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using MudBlazor.Services;
using MySql.Data.MySqlClient;

namespace ManticoreSearch.UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add MudBlazor services
            builder.Services.AddMudServices();

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddScoped<ManticoreContext>();

            builder.Services.AddScoped<FilesService>();
            builder.Services.AddScoped<ManticoreService>();

            builder.Services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
                options.MaxRequestBodySize = Int64.MaxValue;
            });

            builder.Services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
                options.Limits.MaxRequestBodySize = Int64.MaxValue;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
