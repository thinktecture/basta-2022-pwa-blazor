using Blazor.PaintJS;
using Blazor.PaintJS.Services;
using KristofferStrube.Blazor.FileSystemAccess;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Thinktecture.Blazor.AsyncClipboard;
using Thinktecture.Blazor.WebShare;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddFileSystemAccessService();

builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<PaintService>();

builder.Services.AddAsyncClipboardService();
builder.Services.AddWebShareService();

await builder.Build().RunAsync();
