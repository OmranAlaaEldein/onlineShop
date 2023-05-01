using Microsoft.AspNetCore.Http;
using Shopping.Models;
using Shopping.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Services.Admin.CategorySer
{
    public interface ICategoryServices
    {
        //Get
        Task<List<CategoryDto>> GetAll();
        Task<CategoryDto> Get(Guid? id);
        Task<List<CategoryDto>> FindWithName(string name);
        List<CategoryDto> Filter(string name, int skip, int take,string IdBrade);

        //DML
        Task<CategoryDto> Add(CareateUpdateCategoryDto entity);
        Task<CategoryDto> Update(CareateUpdateCategoryDto entity);
        Task<CategoryDto> Delete(Guid id);

        //Save
        void Save();
    }
}
