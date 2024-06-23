using Daktbot.Common.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Common
{
    public static class ServiceBuilder
    {
        public static void AddCommonServices(this IServiceCollection services)
        {
            services.AddSingleton<IPersonService, PersonService>();
            services.AddSingleton<IRaidService, RaidService>();
        }

    }
}
