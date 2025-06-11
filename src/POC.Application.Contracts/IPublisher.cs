using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC
{
    public interface IPublisher
    {
        Task PublishAsync<T>(T @event) where T : class;
    }
}
