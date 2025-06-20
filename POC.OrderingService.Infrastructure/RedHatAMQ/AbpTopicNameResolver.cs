using System;
using System.Reflection;
using Volo.Abp.EventBus;

namespace POC.OrderingService.Infrastructure.RedHatAMQ
{
    internal static class AbpTopicNameResolver
    {
        internal static string GetTopicName(this object @event)
        {
            // 1. Check for [EventName] attribute first
            var eventType = @event.GetType();
            var eventNameAttr = eventType.GetCustomAttribute<EventNameAttribute>();
            if (eventNameAttr != null)
            {
                return eventNameAttr.Name;
            }

            // 2. Fallback to convention-based naming
            string name = eventType.Name;

            // Remove "Event" suffix if present
            if (name.EndsWith("Event"))
            {
                name = name[..^5];
            }

            // Convert to dotted notation (OrderCreated -> Order.Created)
            name = System.Text.RegularExpressions.Regex.Replace(
                name,
                "(?<=[a-z])([A-Z])",
                ".$1",
                System.Text.RegularExpressions.RegexOptions.Compiled
            ).ToLower();

            return name;
        }
    }
}
