using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Shopping.Data.Reposotries;
using Shopping.Models;
using Shopping.Models.Const;
using Shopping.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Shopping.Services.Admin.ItemSer
{
    public class ItemServices : IItemServices
    {
        private readonly IRItem _RItem;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

        public ItemServices(IRItem ritem,
            IWebHostEnvironment webHostEnvironment,
            IMapper mapper,
            IMemoryCache memoryCache)
        {
            _RItem = ritem;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
            _memoryCache=memoryCache;
    }

        #region get
        public async Task<ItemDto> Get(Guid? id)
        {
            //get + mapper
            var item = await _RItem.Get(id);
            var result = _mapper.Map<ItemDto>(item);
            return result;
        }

        public async Task<List<ItemDto>> GetAll()
        {
            var cacheKey = "itemList";
            //checks if cache entries exists
            if (!_memoryCache.TryGetValue(cacheKey, out List<ItemDto> result))
            {
                //get all + mapper
                var items = await _RItem.GetAll(orderBy: x => x.OrderBy(y => y.Name));
                result = _mapper.Map<List<ItemDto>>(items);

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

        public async Task<List<ItemDto>> FindWithName(string name)
        {
            //find + mapper
            var items = await _RItem.FindCondition(x => x.Name.Contains(name));
            var result = _mapper.Map<List<ItemDto>>(items);
            return result;
        }


        public List<ItemDto> Filter(string name, int skip, int take, string IdBrade, string IdCat, string IdProduct)
        {
            var items = _RItem.Filter(x => x.Name.Contains(name) 
                                && x.productId.ToString().Equals(IdProduct) 
                                && x.product.CategoryId.ToString().Equals(IdCat) 
                                && x.product.Category.BradeId.ToString().Equals(IdBrade),
                                skip, take, orderBy: x => x.OrderBy(y => y.Name));
            
            var result = _mapper.Map<List<ItemDto>>(items);
            return result;
        }
        #endregion get

        #region operation
        public async Task<ItemDto> Add(CreateUpdateItemDto entity, IFormFile file)
        {
            //add + mapper
            var resultDto = _mapper.Map<Item>(entity);
            var item = await _RItem.Add(resultDto);

            //Create the Directory.
            string webRootPath = _webHostEnvironment.WebRootPath;
            string path = Path.Combine(webRootPath, PathConst.Item);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //save file
            if (file.Length > 0)
            {
                var fileDes = file.FileName.Split('.');
                var fileName = fileDes[0] + "_" + entity.id + "." + fileDes[1];
                var fullPath = Path.Combine(path, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                entity.pathImage = fileName;
                _RItem.Save();
            }

            //mapper + return
            var result = _mapper.Map<ItemDto>(item);
            return result;
        }
        public async Task<ItemDto> Update(CreateUpdateItemDto entity, IFormFile file)
        {
            //update image
            var tempItem = await _RItem.Get(entity.id);
            var resultDto = _mapper.Map<Item>(entity);


            //update image
            if (file.Length > 0)
            {
                //check folder exist else delete if image exist
                string webRootPath = _webHostEnvironment.WebRootPath;
                string path = Path.Combine(webRootPath, PathConst.Item);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                else
                {
                    //check old image exist
                    if (!string.IsNullOrEmpty(tempItem.pathImage))
                    {
                        var oldImage = path + "/" + tempItem.pathImage;
                        FileInfo oldfile = new FileInfo(oldImage);
                        if (oldfile.Exists)
                        {
                            oldfile.Delete();
                        }
                    }
                }

                //add new image
                var fileDes = file.FileName.Split('.');
                var fileName = fileDes[0] + "_" + tempItem.id + "." + fileDes[1];
                var fullPath = Path.Combine(path, fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                resultDto.pathImage = fileName;
            }

            //Update + mapper
            var item = await _RItem.Update(resultDto);
            var result = _mapper.Map<ItemDto>(item);
            return result;
        }
        public async Task<ItemDto> Delete(Guid id)
        {
            //delete image
            var tempItem = _RItem.Get(id).GetAwaiter().GetResult();
            if (!string.IsNullOrEmpty(tempItem.pathImage))
            {
                string webRootPath = _webHostEnvironment.WebRootPath;
                string path = Path.Combine(webRootPath, PathConst.Item);
                var oldImage = path + "/" + tempItem.pathImage;
                FileInfo file = new FileInfo(oldImage);
                if (file.Exists)
                {
                    file.Delete();
                }
            }
            
            //delete + mapper
            var item = await _RItem.Delete(id);
            var result = _mapper.Map<ItemDto>(item);
            return result;

        }

        #endregion operation
        public void Save()
        {
            _RItem.Save();
        }

       
    }
}
