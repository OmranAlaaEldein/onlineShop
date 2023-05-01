using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Shopping.Data;
using Shopping.Models;
using Shopping.Models.Const;
using Shopping.Models.ViewModels;
using Shopping.Services.Admin.BradeSer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Shopping.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class BradeController : ControllerBase
    {
        private readonly IBradeServices _bradeServices;
        
        public BradeController(IBradeServices bradeServices)
        {
            _bradeServices = bradeServices;
        }
        #region get
        [HttpGet]
        public async Task<JsonResult> GetAsync()
        {
            var Brads = await _bradeServices.GetAll();
            return new JsonResult(Brads);
        }

        [HttpGet("{id}")]
        public async Task<JsonResult> GetAsync(Guid id)
        {
            var Brad = await _bradeServices.Get(id);
            if (Brad == null)
                return new JsonResult("NotFound");

            return new JsonResult(Brad);
        }

        [HttpGet]
        [Route("Search")]
        public async Task<JsonResult> SearchAsync(string name)
        {
            var Brad = await _bradeServices.FindWithName(name);
            return new JsonResult(Brad);
        }

        [HttpGet]
        [Route("Filter")]
        public JsonResult Filter(string name, int skip, int take)
        {
            var Brads = _bradeServices.Filter(name, skip, take);
            return new JsonResult(Brads);
        }
        #endregion get

        #region operation
        [HttpPost]
        public async Task<ActionResult> PostAsync(IFormFile file,[FromForm] CreateUpdateBradeDto createUpdatebrade)
        {
            var Bradresult = await _bradeServices.Add(createUpdatebrade, file);
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> PutAsync(IFormFile file,[FromForm] CreateUpdateBradeDto updatedBrade)
        {
            //valid id
            if (updatedBrade.id == Guid.Empty)
                return BadRequest(
                    new Response()
                    {
                        Errors = new List<string>() { ValidConst.ErrorIdNotVAlid },
                        Result = false
                    });

            //exist
            var brad = await _bradeServices.Get(updatedBrade.id);
            if (brad == null)
                return NotFound(new Response()
                {
                    Errors = new List<string>() { ValidConst.ErrorBradNotExist },
                    Result = false
                });

            //update
            var result = await  _bradeServices.Update(updatedBrade, file);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            //valid id
            if (id==Guid.Empty)
                return BadRequest(
                    new Response()
                    {
                        Errors = new List<string>() { ValidConst.ErrorIdNotVAlid },
                        Result = false
                    });

            //valid exist
            var brad =await _bradeServices.Get(id);
            if (brad == null)
                return NotFound(
                    new Response()
                    {
                        Errors = new List<string>() { ValidConst.ErrorBradNotExist },
                        Result = false
                    });

            //valid have category
            if (brad.Categorys.Count>0)
                return BadRequest(new Response(){
                    Errors = new List<string>() { ValidConst.ErrorDeleteCategory},
                    Result = false
                });

            //delete
            var result = await _bradeServices.Delete(id);
            return new JsonResult(result);
        }
        #endregion operation
    }
}
