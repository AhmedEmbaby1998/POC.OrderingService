using System;
using System.Collections.Generic;
using MediatR;
using POC.Shared.ValueObjects;

namespace POC.Orders.Commands
{
    public record PayOrderCommand : IRequest<Guid>
    {
        public PayOrderCommand(Guid id,Money totalPrice)
        {
            Id = id;
            TotalPrice = totalPrice;
        }

        public Money TotalPrice { get; }
        public Guid Id { get; set; }
    }
}
