using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using QuartzUI;
using QuartzUI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("https://localhost:7000"),
    Timeout = TimeSpan.FromSeconds(30)
});

builder.Services.AddScoped<ApiClient>();
builder.Services.AddScoped<SweetAlertService>();

await builder.Build().RunAsync();
