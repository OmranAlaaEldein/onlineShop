using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Shopping.Data.Reposotries;
using Shopping.Models.Customer;
using Shopping.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Services.Customer
{
    public class OrderServices : IOrderServices
    {
        private readonly IROrder _ROrder;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

        public OrderServices(IROrder order,
            IWebHostEnvironment webHostEnvironment,
            IMapper mapper,
            IMemoryCache memoryCache)
        {
            _ROrder = order;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }


        #region get

        public async Task<List<OrderDto>> GetAll()
        {
            var cacheKey = "orderList";
            //checks if cache entries exists
            if (!_memoryCache.TryGetValue(cacheKey, out List<OrderDto> result))
            {
                //get all + mapper
                var orders = await _ROrder.GetAll(orderBy: x => x.OrderBy(y => y.DateOrderAsk));
                result = _mapper.Map<List<OrderDto>>(orders);

                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(50),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromSeconds(20)
                };
                result = result.OrderBy(x=>x.state).ToList();
                //setting cache entries
                _memoryCache.Set(cacheKey, result, cacheExpiryOptions);
            }
            return result;
        }

        public async Task<OrderDto> Get(Guid? id)
        {
            //get + mapper
            var order = await _ROrder.Get(id);
            var result = _mapper.Map<OrderDto>(order);
            return result;
        }

        public async Task<List<OrderDto>> FindWithState(string state)
        {
            //find + mapper
            var orders = await _ROrder.FindCondition(x => x.state.ToString().Equals(state));
            var result = _mapper.Map<List<OrderDto>>(orders);
            return result;
        }

        public  List<OrderDto> Filter(string name, int skip, int take)
        {
            //    var products = _ROrder.Filter(x => x.Name.Contains(name), skip, take, orderBy: x => x.OrderBy(y => y.Name), include: x => x.Include(y => y.Items));
            //    var result = _mapper.Map<List<OrderDto>>(products);
            return new List<OrderDto>();
        }
        #endregion get




        #region operation

        public async Task<OrderDto> Add(OrderCreateUpdateDto entity)
        {
            //add + mapper
            var resultDto = _mapper.Map<Order>(entity);
            var order = await _ROrder.Add(resultDto);
            var result = _mapper.Map<OrderDto>(order);
            return result;
        }

        public async Task<OrderDto> Update(OrderCreateUpdateDto entity)
        {
            //Update + mapper
            var resultDto = _mapper.Map<Order>(entity);
            var product = await _ROrder.Update(resultDto);
            var result = _mapper.Map<OrderDto>(product);
            return result;
        }

        public void Save()
        {
            _ROrder.Save();
        }
        #endregion  operation
    }
}
