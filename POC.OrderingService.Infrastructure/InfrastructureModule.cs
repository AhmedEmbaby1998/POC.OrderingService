using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using POC.OrderingService.Infrastructure.Abstractions;
using POC.OrderingService.Infrastructure.ActiveMq;
using POC.OrderingService.Infrastructure.OutOfBox;
using POC.OrderingService.Infrastructure.RedHatAMQ;
using Volo.Abp;
using Volo.Abp.BackgroundJobs.Hangfire;
using Volo.Abp.EntityFrameworkCore.DistributedEvents;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Hangfire;
using Volo.Abp.Modularity;
using static IdentityModel.ClaimComparer;

namespace POC.OrderingService.Infrastructure
{
    [DependsOn(typeof(POCDomainModule),typeof(AbpBackgroundJobsHangfireModule))]
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


            });

            context.Services.AddScoped<IMessageBrokerPublisher, RedhatAMQPublisher>();

            ConfigureHangfire(context, context.Services.GetConfiguration());

            if (context.Services.GetConfiguration().GetValue<bool>("FeatureFlags:UseHangFireOutOfBox"))
            {

                Configure<AbpDistributedEventBusOptions>(options =>
                {
                    options.Outboxes.Configure(config =>
                    {
                        config.IsSendingEnabled = context.Configuration.GetValue<bool>("OutOfBox:Enable");
                    });
                });
                context.Services.AddScoped<IDistributedEventBus, HangFireOutOfBoxEventBus>();
            }
            else
            {
                context.Services.AddScoped<IDistributedEventBus, DBOutOfBoxEventBus>();
                context.Services.AddScoped<IEventOutboxManager, EventOutOfBoxManager>();
            }

        }

        private void ConfigureHangfire(ServiceConfigurationContext context, IConfiguration configuration)
        {
            Configure<AbpHangfireOptions>(options =>
            {
                options.ServerOptions = new BackgroundJobServerOptions
                {
                    Queues = ["OutOfBox","Default"],
                    WorkerCount =Environment.ProcessorCount * 2,
                };
            });

            context.Services.AddHangfire(config =>
            {
                config.UseSqlServerStorage(configuration.GetConnectionString("Default"));
            });
        }
    }
}
