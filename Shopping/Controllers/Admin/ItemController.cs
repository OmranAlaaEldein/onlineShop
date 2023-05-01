using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopping.Models;
using Shopping.Models.Const;
using Shopping.Models.ViewModels;
using Shopping.Services.Admin.ItemSer;
using Shopping.Services.Admin.ProductSer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly ItemServices _ItemServices;
        private readonly IProductServices _productServices;

        public ItemController(ItemServices itemServices,
            IProductServices productServices)
        {
            _ItemServices = itemServices;
            _productServices = productServices;
        }

        #region Get

        [HttpGet]
        public async Task<JsonResult> GetAsync()
        {
            var items = await _ItemServices.GetAll();
            return new JsonResult(items);
        }

        [HttpGet("{id}")]
        public async Task<JsonResult> GetAsync(Guid id)
        {
            var item = await _ItemServices.Get(id);
            return new JsonResult(item);
        }

        [HttpGet]
        [Route("Search")]
        public async Task<JsonResult> SearchAsync(string name)
        {
            var item = await _ItemServices.FindWithName(name);
            return new JsonResult(item);
        }

        [HttpGet]
        [Route("Filter")]
        public JsonResult Filter(string name, int skip, int take,
             string IdBrade = "", string IdCat = "", string IdProduct = "")
        {
            var items = _ItemServices.Filter(name, skip, take, IdBrade,IdCat , IdProduct);
            return new JsonResult(items);
        }
        #endregion Get

        #region Operation

        [HttpPost]
        public async Task<ActionResult> PostAsync(IFormFile file, [FromForm] CreateUpdateItemDto sentitem)
        {
            //valid there fathere exist
            var product = await _productServices.Get(sentitem.productId);
            if (product == null)
            {
                return NotFound(
                      new Response()
                      {
                          Errors = new List<string>() { ValidConst.ErrorNotFoundproduct },
                          Result = false
                      });
            }

            //add
            var item = await _ItemServices.Add(sentitem, file);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutAsync(IFormFile file, [FromForm] CreateUpdateItemDto updatedItem)
        {
            //valid id
            if (updatedItem.id == Guid.Empty)
                return BadRequest(new Response()
                {
                    Errors = new List<string>() { ValidConst.ErrorIdNotVAlid },
                    Result = false
                });

            //valid exist
            var Item = await _ItemServices.Get(updatedItem.id);
            if (Item == null)
                return NotFound(  new Response()
                            {
                                Errors = new List<string>() { ValidConst.ErrorNotFoundItem },
                                Result = false
                            });

            //update
            await _ItemServices.Update(updatedItem, file);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            //valid
            if (id == Guid.Empty)
                return BadRequest(new Response()
                {
                    Errors = new List<string>() { ValidConst.ErrorIdNotVAlid },
                    Result = false
                });

            //valid exist
            var item = await _ItemServices.Get(id);
            if (item == null)
                return NotFound(new Response()
                {
                    Errors = new List<string>() { ValidConst.ErrorNotFoundItem },
                    Result = false
                });

            //delete
            await _ItemServices.Delete(id);
            return Ok();
        }

        #endregion Operation
    }
}
