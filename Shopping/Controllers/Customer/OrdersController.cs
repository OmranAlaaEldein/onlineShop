using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shopping.Models;
using Shopping.Models.Const;
using Shopping.Models.ViewModels;
using Shopping.Services.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Controllers.Customer
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderServices _OrderServices;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(IOrderServices orderServices,
            UserManager<ApplicationUser> userManager)
        {
            _OrderServices= orderServices;
            _userManager = userManager;
        }

        #region get
        [HttpGet]
        public async Task<JsonResult> GetAsync() //by date
        {
            var ordesr = await _OrderServices.GetAll();
            return new JsonResult(ordesr);
        }

        [HttpGet("{id}")]
        public async Task<JsonResult> GetAsync(Guid id)
        {
            var ordes = await _OrderServices.Get(id);
            return new JsonResult(ordes);
        }

        [HttpGet]
        [Route("Search")]
        public async Task<JsonResult> SearchAsync(string state)
        {
            var ordesr = await _OrderServices.FindWithState(state);
            return new JsonResult(ordesr);
        }

        [HttpGet]
        [Route("Filter")]
        public JsonResult Filter(string name, int skip, int take)
        {
            var ordesr = _OrderServices.Filter(name, skip, take);
            return new JsonResult(ordesr);
        }
        #endregion get


        #region operation

        [HttpPost]
        public async Task<ActionResult> PostAsync([FromForm] OrderCreateUpdateDto sentorder)
        {
            //valid there user
            var user = _userManager.Users.FirstOrDefault(x=>x.Id==sentorder.idUser);
            if (user ==null)
                return BadRequest(new Response()
                {
                    Errors = new List<string>() { ValidConst.ErrorUserNotExist },
                    Result = false
                });
            var item = await _OrderServices.Add(sentorder);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutAsync([FromForm] OrderCreateUpdateDto updatedOrder)
        {
            //valid id
            if (updatedOrder.id == Guid.Empty)
                return BadRequest(new Response()
                {
                    Errors = new List<string>() { ValidConst.ErrorIdNotVAlid },
                    Result = false
                });

            //valid exist
            var Item = await _OrderServices.Get(updatedOrder.id);
            if (Item == null)
                return NotFound(new Response()
                {
                    Errors = new List<string>() { ValidConst.ErrorNotFoundOrder },
                    Result = false
                });

            //update
            await _OrderServices.Update(updatedOrder);
            return Ok();
        }
        #endregion operation

    }
}