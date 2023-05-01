using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shopping.Models;
using Shopping.Models.Const;
using Shopping.Models.ViewModels;
using Shopping.Services.Admin.BradeSer;
using Shopping.Services.Admin.CategorySer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryServices _categoryServices;
        private readonly IBradeServices _bradeServices;

        public CategoryController(ICategoryServices categoryServices,
            IBradeServices bradeServices)
        {
            _categoryServices = categoryServices;
            _bradeServices = bradeServices;
        }

        #region get
        [HttpGet]
        public async Task<JsonResult> GetAsync()
        {
            var category =await _categoryServices.GetAll();
            return new JsonResult(category);
        }

        [HttpGet("{id}")]
        public async Task<JsonResult> GetAsync(Guid id)
        {
            var category = await _categoryServices.Get(id);
            return new JsonResult(category);
        }

        [HttpGet]
        [Route("Search")]
        public async Task<JsonResult> SearchAsync(string name)
        {
            var category = await _categoryServices.FindWithName(name);
            return new JsonResult(category);
        }

        [HttpGet]
        [Route("Filter")]
        public JsonResult Filter(string name, int skip, int take,
            string IdBrade = "")
        {
            var categorys = _categoryServices.Filter(name, skip, take, IdBrade);
            return new JsonResult(categorys);
        }
        #endregion get

        #region operation

        [HttpPost]
        public async Task<ActionResult> PostAsync([FromForm] CareateUpdateCategoryDto sentcategory)
        {
            //valid there fathere exist
            var Brade=await _bradeServices.Get(sentcategory.BradeId);
            if(Brade==null)
            {
                return NotFound(
                    new Response()
                    {
                        Errors = new List<string>() { ValidConst.ErrorBradNotExist },
                        Result = false
                    });
            }

            //add category
            var category =await _categoryServices.Add(sentcategory);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutAsync([FromForm] CareateUpdateCategoryDto updatedcategory)
        {
            //valid id
            if (updatedcategory.id == Guid.Empty)
                return BadRequest(
                    new Response()
                    {
                        Errors = new List<string>() { ValidConst.ErrorIdNotVAlid },
                        Result = false
                    });

            //valid Brade
            var Brade = await _bradeServices.Get(updatedcategory.BradeId);
            if (Brade == null)
            {
                return NotFound(
                    new Response()
                    {
                        Errors = new List<string>() { ValidConst.ErrorBradNotExist },
                        Result = false
                    });
            }

            //valid CAtegory
            var category = await _categoryServices.Get(updatedcategory.id);
            if (category == null)
                return NotFound(
                    new Response()
                    {
                        Errors = new List<string>() { ValidConst.ErrorNotFoundCategory },
                        Result = false
                    });

            //update
            await _categoryServices.Update(updatedcategory);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            //valid id
            if (id == Guid.Empty)
                return BadRequest(
                   new Response()
                   {
                       Errors = new List<string>() { ValidConst.ErrorIdNotVAlid },
                       Result = false
                   });

            //valid category is exist
            var category = await _categoryServices.Get(id);
            if (category == null)
                return NotFound(
                   new Response()
                   {
                       Errors = new List<string>() { ValidConst.ErrorNotFoundCategory },
                       Result = false
                   });

            //valid no child
            if (category.Products.Count > 0)
                return BadRequest(new Response()
                {
                    Errors = new List<string>() { ValidConst.ErrorDeleteProducts },
                    Result = false
                });

            //delete
            category= await _categoryServices.Delete(id);
            return new JsonResult(category);
        }
        #endregion operation
    }
}
