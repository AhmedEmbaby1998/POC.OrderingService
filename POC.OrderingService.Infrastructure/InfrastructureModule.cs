using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Hangfire;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using POC.OrderingService.Infrastructure.ActiveMq;
using POC.OrderingService.Infrastructure.OutOfBox;
using POC.OrderingService.Infrastructure.RedHatAMQ;
using Volo.Abp;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Hangfire;
using Volo.Abp.Modularity;

namespace POC.OrderingService.Infrastructure
{
    [DependsOn(typeof(POCDomainModule))]
    public class InfrastructureModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            //Configure RedHatAMQ Settings
            Configure<RedHatAMQSettings>(
                context.Services.GetConfiguration().GetSection("RedHatAMQ")
            );
            context.Services.AddMassTransit(config =>
            {
                // Add consumers (if using IConsumer<T>)
                config.UsingActiveMq((context, cfg) =>
                {
                    var settings = context.GetRequiredService<IOptions<RedHatAMQSettings>>().Value;
                    cfg.Host(settings.HostName, settings.Port, h =>
                    {
                        h.Username(settings.UserName);
                        h.Password(settings.Password);
                    });
                });
                context.Services.AddScoped<IDistributedEventBus, RedHatAMQEventBus>();
                context.Services.AddScoped<IEventOutboxManager, EventOutOfBoxManager>();

            });
            ConfigureHangfire(context, context.Services.GetConfiguration());
        }

        private void ConfigureHangfire(ServiceConfigurationContext context, IConfiguration configuration)
        {
            Configure<AbpHangfireOptions>(options =>
            {
                options.ServerOptions = new BackgroundJobServerOptions
                {
                    Queues = ["OutOfBox","Default"],
                };
            });

            context.Services.AddHangfire(config =>
            {
                config.UseSqlServerStorage(configuration.GetConnectionString("Default"));
            });
        }
    }
}
