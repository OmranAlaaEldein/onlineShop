using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopping.Models;
using Shopping.Models.Const;
using Shopping.Models.ViewModels;
using Shopping.Services.Admin.CategorySer;
using Shopping.Services.Admin.ProductSer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductServices _productServices;
        private readonly ICategoryServices _categoryServices;

        public ProductController(IProductServices productServices,
            ICategoryServices categoryServices)
        {
            _productServices = productServices;
            _categoryServices = categoryServices;
        }

        #region get
        [HttpGet]
        public async Task<JsonResult> GetAsync()
        {
            var products = await _productServices.GetAll();
            
            return new JsonResult(products);
        }

        [HttpGet("{id}")]
        public async Task<JsonResult> GetAsync(Guid id)
        {
            var product = await _productServices.Get(id);
            //mapper

            return new JsonResult(product);
        }

        [HttpGet]
        [Route("Search")]
        public async Task<JsonResult> SearchAsync(string name)
        {
            var product = await _productServices.FindWithName(name);
            //mapper

            return new JsonResult(product);
        }


        [HttpGet]
        [Route("Filter")]
        public JsonResult Filter(string name, int skip, int take,
            string IdBrade = "", string IdCat = "")
        {
            var products = _productServices.Filter(name, skip, take, IdBrade, IdCat);
            return new JsonResult(products);
        }
        #endregion get

        #region operation
        [HttpPost]
        public async Task<ActionResult> PostAsync([FromForm] CreateUpdateProductDto sentproduct)
        {
            //valid there fathere exist
            var Category = await _categoryServices.Get(sentproduct.CategoryId);
            if (Category == null)
            {
                return NotFound(new Response()
                {
                    Errors = new List<string>() { ValidConst.ErrorNotFoundCategory },
                    Result = false
                });
            }
            
            //add product
            var product = await _productServices.Add(sentproduct);
            return Ok(product);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutAsync([FromForm] CreateUpdateProductDto updatedproduct)
        {
            //valid id
            if (updatedproduct.id == Guid.Empty)
                return BadRequest(new Response()
                {
                    Errors = new List<string>() { ValidConst.ErrorIdNotVAlid },
                    Result = false
                });

            //valid product exist
            var product =await _productServices.Get(updatedproduct.id);
            if (product == null)
                return NotFound(new Response()
                {
                    Errors = new List<string>() { ValidConst.ErrorNotFoundproduct },
                    Result = false
                });

            //valid there fathere exist
            var Category = await _categoryServices.Get(updatedproduct.CategoryId);
            if (Category == null)
            {
                return NotFound(new Response()
                {
                    Errors = new List<string>() { ValidConst.ErrorNotFoundCategory },
                    Result = false
                });
            }
            
            //update
            await _productServices.Update(updatedproduct);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            //valid id
            if (id == Guid.Empty)
                return BadRequest(new Response()
                {
                    Errors = new List<string>() { ValidConst.ErrorIdNotVAlid },
                    Result = false
                });

            //valid product exist
            var product = await _productServices.Get(id);
            if (product == null)
                return NotFound(new Response()
                {
                    Errors = new List<string>() { ValidConst.ErrorNotFoundproduct },
                    Result = false
                });

            //valid no child
            if (product.Items.Count > 0)
                return BadRequest(new Response()
                {
                    Errors = new List<string>() { ValidConst.ErrorDeleteItems },
                    Result = false
                });

            //delete
            await _productServices.Delete(id);
            return new JsonResult(product);
        }
        #endregion operation
    }
}
