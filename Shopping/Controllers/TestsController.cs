using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Shopping.Data;
using Shopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        private IRBrade _Rbrade;
        private readonly IMapper _mapper;

        //http://codingsonata.com/localization-in-asp-net-core-web-api/
        private readonly IStringLocalizer<TestsController> stringLocalizer;
        private readonly IStringLocalizer<SharedResource> sharedResourceLocalizer;

   
        public TestsController(IRBrade Rbrade, IMapper mapper,
            IStringLocalizer<TestsController> postsControllerLocalizer, IStringLocalizer<SharedResource> sharedResourceLocalizer)
        {
            _Rbrade = Rbrade;
            _mapper = mapper;
            this.stringLocalizer = postsControllerLocalizer;
            this.sharedResourceLocalizer = sharedResourceLocalizer;

        }
        [HttpPost]
        [Route("TestApi")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult TestApi()
        {
            //User user = new User() { name="omran",lastName="AlaaEldein"};

            //UserDto userDto = _mapper.Map<UserDto>(user);

            _Rbrade.Add(new Models.Brade() {Name="dd",pathImage="ddd" });
            _Rbrade.Add(new Models.Brade() {Name="dd",pathImage="ddd" });
            var t=  _Rbrade.GetAll().GetAwaiter().GetResult();

            var article = stringLocalizer["Article"];
            var postName = stringLocalizer.GetString("Welcome").Value ?? "";

            return Ok(new { t= t,PostType = article.Value, PostName = postName });
            //return new JsonResult(t);
        }

    }
}
