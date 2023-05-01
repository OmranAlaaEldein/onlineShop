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
using Shopping.Models;
using Shopping.Models.Const;
using Shopping.Models.ViewModels;

namespace Shopping.Services.Admin.BradeSer
{
    public class BradeServices : IBradeServices
    {
        private readonly IRBrade _Rbrade;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

        public BradeServices(IRBrade Rbrade,
            IWebHostEnvironment webHostEnvironment,
            IMapper mapper,
            IMemoryCache memoryCache)
        {
            _Rbrade = Rbrade;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        #region get
        public async Task<List<BradeDto>> GetAll()
        {
            var cacheKey = "bradeList";
            //checks if cache entries exists
            if (!_memoryCache.TryGetValue(cacheKey, out List<BradeDto> result))
            {
                var brads = await _Rbrade.GetAll(orderBy: x=>x.OrderBy(y=>y.Name));
                result = _mapper.Map<List<BradeDto>>(brads);

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

        public async Task<BradeDto> Get(Guid? id)
        {
            var brads = await _Rbrade.Get(id);
           var result = _mapper.Map<BradeDto>(brads);
            return result;
        }

        public async Task<List<BradeDto>> FindWithName(string name)
        {
            var brads = await _Rbrade.FindCondition(x => x.Name.Contains(name));
            var result = _mapper.Map<List<BradeDto>>(brads);
            return result; 
        }

        public List<BradeDto> Filter(string name,int skip ,int take)
        {
            var brads = _Rbrade.Filter( x => x.Name.Contains(name),skip,take, orderBy:x=>x.OrderBy(y=>y.Name), include:x=>x.Include(y=>y.Categorys));
            var result = _mapper.Map<List<BradeDto>>(brads);
            return result;
        }


        #endregion get

        #region operation
        public async Task<BradeDto> Add(CreateUpdateBradeDto entity, IFormFile file)
        {
            //mapper + add
            var resultDto = _mapper.Map<Brade>(entity);
            var brade=await _Rbrade.Add(resultDto);
            
            //Create the Directory.
            string webRootPath = _webHostEnvironment.WebRootPath;
            string path = Path.Combine(webRootPath, PathConst.Brade);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (file.Length > 0)
            {
                var fileDes = file.FileName.Split('.');
                var fileName = fileDes[0] + "_" + resultDto.id+"."+ fileDes[1];
                var fullPath = Path.Combine(path, fileName);
                
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                resultDto.pathImage = fileName;
                _Rbrade.Save();
            }

            //mapper + return
            var resultAdd = _mapper.Map<BradeDto>(resultDto);
            return resultAdd;
        }

        public async Task<BradeDto> Update(CreateUpdateBradeDto entity, IFormFile file)
        {   
            //mapper+update
            var brade = _mapper.Map<Brade>(entity);
            var tempBrade = await _Rbrade.Get(brade.id); //var resultUpdate = await _Rbrade.Update(brade);
            
            tempBrade.Name = brade.Name;
            tempBrade.Categorys = new List<Category>();
            if(brade.Categorys!=null)
                tempBrade.Categorys.AddRange(brade.Categorys);

            //update image
            if (file.Length > 0)
            {
                //check folder exist else delete if image exist
                string webRootPath = _webHostEnvironment.WebRootPath;
                string path = Path.Combine(webRootPath, PathConst.Brade);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                else
                {
                    //check old image exist
                    if (!string.IsNullOrEmpty(tempBrade.pathImage))
                    {
                        var oldImage = path + "/" + tempBrade.pathImage;
                        FileInfo oldfile = new FileInfo(oldImage);
                        if (oldfile.Exists)
                        {
                            oldfile.Delete();
                        }
                    }
                }

                //add new image
                var fileDes = file.FileName.Split('.');
                var fileName = fileDes[0] + "_" + tempBrade.id + "." + fileDes[1];
                var fullPath = Path.Combine(path, fileName);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                tempBrade.pathImage = fileName;
            }

            //save and return;
            _Rbrade.Save();
            var result = _mapper.Map<BradeDto>(tempBrade);
            return result;
        }

        public async Task<BradeDto> Delete(Guid id)
        {
            //delete image
            var tempBrade=await _Rbrade.Get(id);
            
            if(!string.IsNullOrEmpty(tempBrade.pathImage))
            {
                string webRootPath = _webHostEnvironment.WebRootPath;
                string path = Path.Combine(webRootPath, PathConst.Brade);
                var oldImage = path + "/" + tempBrade.pathImage;
                FileInfo file = new FileInfo(oldImage);
                if (file.Exists)
                {
                    file.Delete();
                }
            }

            //delete + mapper 
            var resultDelete = await _Rbrade.Delete(id);
            var result = _mapper.Map<BradeDto>(resultDelete);
            return result;
        }

        #endregion operation

        public void Save()
        {
             _Rbrade.Save();
        }
    }
}
