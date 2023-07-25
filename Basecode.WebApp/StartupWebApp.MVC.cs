using Microsoft.Extensions.DependencyInjection;

namespace Basecode.WebApp
{
    public partial class StartupWebApp
    {
        private void ConfigureMVC(IServiceCollection services)
        {
            services.AddMvc();
        }
    }
}
