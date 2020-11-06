using DotNetNuke.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OS_Square 
{
    class StartUp : IDnnStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //Not used but prepares this module for DI

            //services.AddScoped<IPaymentService, SquarePaymentService>;
        }
    }
}
