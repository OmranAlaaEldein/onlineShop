using Microsoft.AspNetCore.Http;
using Shopping.Models;
using Shopping.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Services.Admin.BradeSer
{
    public interface IBradeServices
    {
        //Get
        Task<List<BradeDto>> GetAll();

        Task<BradeDto> Get(Guid? id);

        Task<List<BradeDto>> FindWithName(string name);

        List<BradeDto> Filter(string name, int skip, int take);

        //DML
        Task<BradeDto> Add(CreateUpdateBradeDto entity,IFormFile file);

        Task<BradeDto> Update(CreateUpdateBradeDto entity, IFormFile file);

        Task<BradeDto> Delete(Guid id);

        //Save
        void Save();
    }
}
