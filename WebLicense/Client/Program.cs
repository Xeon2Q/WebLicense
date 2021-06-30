using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebLicense.Client.Auxiliary;
using WebLicense.Client.Auxiliary.Configuration;

namespace WebLicense.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddHttpClient("WebLicense.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("WebLicense.ServerAPI"));
            builder.Services.AddTransient<JsLog>();

            builder.Services.AddLocalization();

            builder.Services.AddApiAuthorization(options =>
                   {
                       options.UserOptions.RoleClaim = "role";
                   })
                   .AddAccountClaimsPrincipalFactory<CustomAccountClaimsPrincipalFactory>();

            await builder.Build().RunAsync();
        }
    }
}
