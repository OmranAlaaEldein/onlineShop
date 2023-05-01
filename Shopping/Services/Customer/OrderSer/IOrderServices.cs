using Microsoft.AspNetCore.Http;
using Shopping.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Services.Customer
{
    public interface IOrderServices
    {
        //Get
        Task<List<OrderDto>> GetAll();
        Task<OrderDto> Get(Guid? id);
        Task<List<OrderDto>> FindWithState(string state);
        List<OrderDto> Filter(string name, int skip, int take);

        //DML
        Task<OrderDto> Add(OrderCreateUpdateDto entity);
        Task<OrderDto> Update(OrderCreateUpdateDto entity);
      
        //Save
        void Save();
    }
}
