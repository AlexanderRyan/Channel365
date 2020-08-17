using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Channel365.Web.Areas.Identity.IdentityHostingStartup))]
namespace Channel365.Web.Areas.Identity {
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}