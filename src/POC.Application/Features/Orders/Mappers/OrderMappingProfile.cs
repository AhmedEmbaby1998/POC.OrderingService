using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using POC.OrderingService.Query.Abstraction.Dtos.Orders;
using POC.Orders.Query;
using POC.Shared.ValueObjects;

namespace POC.Features.Orders.Mappers
{
    internal class OrderMappingProfile: Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<OrderItemReadModelDto,OrderItemDto>()
                .ForMember(dest => dest.Money, opt => opt.MapFrom(src => new Money(src.Amount, src.Currency)))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => new Quantity(src.Count, src.Unit)));

            CreateMap<OrderReadModelDto, OrderDto>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address(src.Street, src.City, src.State, src.ZipCode)))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => new Money(src.Amount, src.Currency)));
        }
    }
}
