using AutoMapper;
using Shopping.Models.Customer;
using Shopping.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Models
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<ApplicationUser, UserDto>();

            CreateMap<Brade, BradeDto>();
            CreateMap<CreateUpdateBradeDto, Brade>();

            CreateMap<Category, CategoryDto>();
            CreateMap<CareateUpdateCategoryDto, Category>();

            CreateMap<Product, ProductDto>();
            CreateMap<CreateUpdateProductDto, Product>();

            CreateMap<Item, ItemDto>();
            CreateMap<CreateUpdateItemDto, Item>();

            CreateMap<Order, OrderDto>(); //.ForMember(x=>x.)
            CreateMap<OrderCreateUpdateDto, Order>();
        }
    }
}
