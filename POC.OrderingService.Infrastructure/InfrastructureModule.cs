using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using POC.OrderingService.Infrastructure.ActiveMq;
using Volo.Abp;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Modularity;

namespace POC.OrderingService.Infrastructure
{
    [DependsOn(typeof(POCDomainModule))]
    public class InfrastructureModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            Configure<ActiveMQSettings>(
                context.Services.GetConfiguration().GetSection("ActiveMQ")
            ); 
            context.Services.AddMassTransit(x =>
            {
                x.UsingActiveMq((ctx, cfg) =>
                {
                    var settings = ctx.GetRequiredService<ActiveMQSettings>();
                    cfg.Host(configuration["ActiveMQ:Host"], h =>
                    {
                        h.Username(configuration["ActiveMQ:Username"]);
                        h.Password(configuration["ActiveMQ:Password"]);

                    });

                    // Configure endpoints
                    cfg.ConfigureEndpoints(ctx);
                });

            });

        }
    }
}
