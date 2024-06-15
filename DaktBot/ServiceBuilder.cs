namespace Daktbot.Runner
{
    public static class ServiceBuilder
    {
        public static void BuildServices(IHostBuilder builder)
        {
            builder.ConfigureServices(serviceCollection => { });

        }
    }
}
