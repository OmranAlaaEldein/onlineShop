using Microsoft.AspNetCore.Http;
using Shopping.Models;
using Shopping.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Services.Admin.ItemSer
{
    public interface IItemServices
    {
        //Get
        Task<List<ItemDto>> GetAll();
        Task<ItemDto> Get(Guid? id);
        Task<List<ItemDto>> FindWithName(string name);
        List<ItemDto> Filter(string name, int skip, int take, string IdBrade, string IdCat, string IdProduct);

        //DML
        Task<ItemDto> Add(CreateUpdateItemDto entity, IFormFile file);
        Task<ItemDto> Update(CreateUpdateItemDto entity, IFormFile file);
        Task<ItemDto> Delete(Guid id);

        //Save
        void Save();
    }
}
