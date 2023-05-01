using Microsoft.AspNetCore.Http;
using Shopping.Models;
using Shopping.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Services.Admin.ProductSer
{
    public interface IProductServices
    {
        //Get
        Task<List<ProductDto>> GetAll();
        Task<ProductDto> Get(Guid? id);
        Task<List<ProductDto>> FindWithName(string name);
        List<ProductDto> Filter(string name, int skip, int take, string IdBrade, string IdCat);

        //DML
        Task<ProductDto> Add(CreateUpdateProductDto entity);
        Task<ProductDto> Update(CreateUpdateProductDto entity);
        Task<ProductDto> Delete(Guid id);

        //Save
        void Save();
    }
}
