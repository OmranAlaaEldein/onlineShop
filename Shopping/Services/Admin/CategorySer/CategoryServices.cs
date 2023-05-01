using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Shopping.Data;
using Shopping.Data.Reposotries;
using Shopping.Models;
using Shopping.Models.ViewModels;

namespace Shopping.Services.Admin.CategorySer
{
    public class CategoryServices : ICategoryServices
    {

        private readonly IRCategory _RCategory;
        private readonly IWebHostEnvironment _webHostEnvironment; 
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

        public CategoryServices(IRCategory rcategory,
            IWebHostEnvironment webHostEnvironment,
             IMapper mapper,
             IMemoryCache memoryCache)
        {
            _RCategory = rcategory;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }


        #region get
        public async Task<List<CategoryDto>> GetAll()
        {
            var cacheKey = "categoryList";
            //checks if cache entries exists
            if (!_memoryCache.TryGetValue(cacheKey, out List<CategoryDto> result))
            {
                var categorys = await _RCategory.GetAll(orderBy: x => x.OrderBy(y => y.Name));
                result = _mapper.Map<List<CategoryDto>>(categorys);

                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(50),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromSeconds(20)
                };
                //setting cache entries
                _memoryCache.Set(cacheKey, result, cacheExpiryOptions);
            }
            
            return result;
        }

        public async Task<CategoryDto> Get(Guid? id)
        {
            var categorys = await _RCategory.Get(id);
            var result = _mapper.Map<CategoryDto>(categorys);
            return result;
        }

        public async Task<List<CategoryDto>> FindWithName(string name)
        {
            var categorys = await _RCategory.FindCondition(x => x.Name.Contains(name));
            var result = _mapper.Map<List<CategoryDto>>(categorys);
            return result;
        }

        public List<CategoryDto> Filter(string name, int skip, int take, string IdBrade)
        {
            var categorys = _RCategory.Filter(x => x.Name.Contains(name)&& x.Brade.id.ToString().Equals(IdBrade), skip, take, orderBy: x => x.OrderBy(y => y.Name), include: x => x.Include(y => y.Products));
            var result = _mapper.Map<List<CategoryDto>>(categorys);
            return result;
        }
        #endregion get

        #region operation

        public async Task<CategoryDto> Add(CareateUpdateCategoryDto entity)
        {
            //mapper + add
            var resultDto = _mapper.Map<Category>(entity);
            var category = await _RCategory.Add(resultDto);
            var result = _mapper.Map<CategoryDto>(category);

            return result;
        }

        public async Task<CategoryDto> Update(CareateUpdateCategoryDto entity)
        {
            //mapper + update
            var resultDto = _mapper.Map<Category>(entity);
            var category = await _RCategory.Update(resultDto);
            var result = _mapper.Map<CategoryDto>(category);

            return result;
        }

        public async Task<CategoryDto> Delete(Guid id)
        {
            //delete + mapper
            var category = await _RCategory.Delete(id);
            var result = _mapper.Map<CategoryDto>(category);

            return result;
        }

        #endregion operation
        public void Save()
        {
            _RCategory.Save();
        }

        
    }


}
