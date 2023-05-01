using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Shopping.Data.Reposotries;
using Shopping.Models;
using Shopping.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Shopping.Services.Admin.ProductSer
{
    public class ProductServices : IProductServices
    {
        private readonly IRProduct _RProduct;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

        public ProductServices(IRProduct rproduct,
            IWebHostEnvironment webHostEnvironment,
            IMapper mapper,
            IMemoryCache memoryCache)
        {
            _RProduct = rproduct;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        #region get
        public async Task<List<ProductDto>> GetAll()
        {
            var cacheKey = "productList";
            //checks if cache entries exists
            if (!_memoryCache.TryGetValue(cacheKey, out List<ProductDto> result))
            {
                //get all + mapper
                var products = await _RProduct.GetAll(orderBy: x => x.OrderBy(y => y.Name));
                result = _mapper.Map<List<ProductDto>>(products);

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

        public async Task<ProductDto> Get(Guid? id)
        {
            //get + mapper
            var product = await _RProduct.Get(id);
            var result = _mapper.Map<ProductDto>(product);
            return result;
        }

        public async Task<List<ProductDto>> FindWithName(string name)
        {
            //find + mapper
            var products = await _RProduct.FindCondition(x => x.Name.Contains(name));
            var result = _mapper.Map<List<ProductDto>>(products);
            return result;
        }

        public List<ProductDto> Filter(string name, int skip, int take,
            string IdBrade, string IdCat)
        {
            var products = _RProduct.Filter(x => x.Name.Contains(name)
                            && x.CategoryId.ToString().Equals(IdCat)
                            && x.Category.BradeId.ToString().Equals(IdBrade),
                            skip, take, orderBy: x => x.OrderBy(y => y.Name), 
                            include: x => x.Include(y => y.Items));
            
            var result = _mapper.Map<List<ProductDto>>(products);
            return result;
        }
        #endregion get

        #region operation
        public async Task<ProductDto> Add(CreateUpdateProductDto entity)
        {
            //add + mapper
            var resultDto = _mapper.Map<Product>(entity);
            var product = await _RProduct.Add(resultDto);
            var result = _mapper.Map<ProductDto>(product);
            return result;
        }

        public async Task<ProductDto> Update(CreateUpdateProductDto entity)
        {
            //Update + mapper
            var resultDto = _mapper.Map<Product>(entity);
            var product = await _RProduct.Update(resultDto);
            var result = _mapper.Map<ProductDto>(product);
            return result;
        }

        public async Task<ProductDto> Delete(Guid id)
        {
            //delete + mapper
            var product = await _RProduct.Delete(id);
            var result = _mapper.Map<ProductDto>(product);
            return result;
        }

        #endregion operation
        public void Save()
        {
            _RProduct.Save();
        }
    }
}
