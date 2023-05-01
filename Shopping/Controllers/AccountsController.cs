using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Shopping.Models.ViewModels;
using Shopping.Models.Const;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Shopping.Models;
using Shopping.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using AutoMapper;

namespace Shopping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly JwtConfig _jwtConfig;
        private readonly IConfiguration _configuration;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly ApplicationDbContext _apiDbContext;
        private readonly IMapper _mapper;


        public AccountsController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptionsMonitor<JwtConfig> optionsMonitor,
            IConfiguration configuration,
            TokenValidationParameters tokenValidationParameters,
             ApplicationDbContext apiDbContext,
             IMapper mapper)
        {
            this.userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
            this.roleManager = roleManager;
            _configuration = configuration;
            _tokenValidationParameters = tokenValidationParameters;
            _apiDbContext = apiDbContext;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] AccountModels model)
        {

            if (ModelState.IsValid) { 
                var userExists = await userManager.FindByNameAsync(model.Email);
                if (userExists != null)
                {
                    return BadRequest(new Response()
                    {
                        Result = false,
                        Errors = new List<string>(){"Email already exist"}
                    });
                }
                ApplicationUser user = new ApplicationUser()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Username,
                    LastName = model.LastName
                };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return Ok(await GenerateJwtTokenAsync(user));
                }

                return new JsonResult(new Response()
                {
                    Result = false,
                    Errors = result.Errors.Select(x => x.Description).ToList()
                }){ StatusCode = 500 };
            }
            return BadRequest(new Response()
            {
                Result = false,
                Errors = new List<string>(){ "Invalid payload" }});
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(model.Username);
                if (user == null)
                {
                    return BadRequest(new Response()
                    {
                        Result = false,
                        Errors = new List<string>(){ AuthConst.Invalid }
                    });
                }

                var isCorrect = await userManager.CheckPasswordAsync(user, model.Password);
                if (isCorrect)
                {
                    //var jwtToken = await GenerateJwtTokenAsync(user);

                    return Ok(await GenerateJwtTokenAsync(user));
                }
                else
                {
                    return BadRequest(new Response()
                    {
                        Result = false,
                        Errors = new List<string>(){ AuthConst.Invalid }
                    });
                }
            }
            return BadRequest(new Response()
            {
                Result = false,
                Errors = new List<string>(){ AuthConst.InvalidPayload }
            });
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
        {
            if (ModelState.IsValid)
            {
                var res = await VerifyToken(tokenRequest);

                if (res == null)
                {
                    return BadRequest(new Response()
                    {
                        Errors = new List<string>() {
                     AuthConst.ErrorTokens
                },
                        Result = false
                    });
                }

                return Ok(res);
            }

            return BadRequest(new Response()
            {
                Errors = new List<string>() {
                AuthConst.InvalidPayload
            },
                Result = false
            });
        }

        [HttpGet]
        [Route("GetUserProfile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UserProfile(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }


        [HttpPost]
        [Route("UpdateProfile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfile model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(model.id);


                if (!string.IsNullOrEmpty(model.oldPassword) && !string.IsNullOrEmpty(model.newPassword))
                {
                    if (!model.newPassword.Equals(model.confirmPassword))
                        return BadRequest(new Response()
                        {
                            Errors = new List<string>() {
                                   AuthConst.ErrorConfirmPassword
                                },
                            Result = false
                        });

                    var isCorrect = await userManager.CheckPasswordAsync(user, model.oldPassword);                    
                    if (!isCorrect)
                    {
                            return BadRequest(new Response()
                            {
                                Errors = new List<string>() {
                                   AuthConst.ErrorInvalidpassword
                                },
                                Result = false
                            });
                    }

                    user.PasswordHash = userManager.PasswordHasher.HashPassword(user, model.newPassword);
                }

                if (!string.IsNullOrEmpty(model.newUsername))
                {
                    user.UserName = model.newUsername;
                }
                if (!string.IsNullOrEmpty(model.newLastName))
                {
                    user.LastName = model.newLastName;
                }

                if (!string.IsNullOrEmpty(model.newEmail))
                {
                    user.Email = model.newEmail;
                }

                //save 
                await userManager.UpdateAsync(user);
                return Ok();
            }

            return BadRequest(new Response()
            {
                Errors = new List<string>() {
                AuthConst.InvalidPayload
            },
                Result = false
            });
        }

        [HttpPost]
        [Route("LogOut")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> LogOut(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            //save 
            
            await userManager.UpdateSecurityStampAsync(user);
            return Ok();
        }

        [HttpPost]
        [Route("getUsers")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<JsonResult> getUsers()
        {
            var users=userManager.Users.ToList();
            return new JsonResult(users);
        }

        //------------------------------
        private async Task<Response> GenerateJwtTokenAsync(IdentityUser user)
        {
            //https://dev.to/moe23/asp-net-core-5-rest-api-authentication-with-jwt-step-by-step-140d
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
                Expires = DateTime.UtcNow.AddHours(6), //DateTime.UtcNow.Add(_jwtConfig.ExpiryTimeFrame),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                IsUsed = false,
                UserId = user.Id,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddYears(1),
                IsRevoked = false,
                Token = RandomString(25) + Guid.NewGuid()
            };

            await _apiDbContext.RefreshTokens.AddAsync(refreshToken);
            await _apiDbContext.SaveChangesAsync();

            return new Response()
            {
                Token = jwtToken,
                Result = true,
                RefreshToken = refreshToken.Token
            };
        }

        private async Task<Response> VerifyToken(TokenRequest tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                //function sure token meets the validation parameters and actual jwt token(sure token and ref is realy)
                var principal = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParameters, out var validatedToken);

                // secur algorithm
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (result == false)
                    {
                        return null;
                    }
                }

                //secure time expire
                var utcExpiryDate = long.Parse(principal.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                var expDate = UnixTimeStampToDateTime(utcExpiryDate);

                if (expDate > DateTime.UtcNow)
                {
                    return new Response()
                    {
                        Errors = new List<string>() { AuthConst.ErrorTokenExpired },
                        Result = false
                    };
                }

                //toekn saved in the db
                var storedRefreshToken = await _apiDbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);

                if (storedRefreshToken == null)
                {
                    return new Response()
                    {
                        Errors = new List<string>() { AuthConst.ErrorRefreshToken },
                        Result = false
                    };
                }

                // Check date of saved token
                if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                {
                    return new Response()
                    {
                        Errors = new List<string>() { AuthConst.ErrorRelogin },
                        Result = false
                    };
                }

                // check if the refresh token has been used
                if (storedRefreshToken.IsUsed)
                {
                    return new Response()
                    {
                        Errors = new List<string>() { AuthConst.ErrorTokenUsed },
                        Result = false
                    };
                }

                // Check if the token is revoked
                if (storedRefreshToken.IsRevoked)
                {
                    return new Response()
                    {
                        Errors = new List<string>() { AuthConst.ErrorTokenRevoked },
                        Result = false
                    };
                }

                // we are getting here the jwt token id
                var jti = principal.Claims.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                // check the id that the recieved token has against the id saved in the db
                if (storedRefreshToken.JwtId != jti)
                {
                    return new Response()
                    {
                        Errors = new List<string>() { AuthConst.ErrorTokenMatch },
                        Result = false
                    };
                }

                storedRefreshToken.IsUsed = true;
                _apiDbContext.RefreshTokens.Update(storedRefreshToken);
                await _apiDbContext.SaveChangesAsync();

                var dbUser = await userManager.FindByIdAsync(storedRefreshToken.UserId);
                return await GenerateJwtTokenAsync(dbUser);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToUniversalTime();
            return dtDateTime;
        }

        private string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}

/*
 Note:
var userRoles = await userManager.GetRolesAsync(user);

                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                    var token = new JwtSecurityToken(
                        issuer: _configuration["JWT:ValidIssuer"],
                        audience: _configuration["JWT:ValidAudience"],
                        expires: DateTime.Now.AddHours(3),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                        );

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });

//
                //return Unauthorized();
//return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = ValidConst.ErrorUserExist });

 [HttpPost("logout")]
        [Authorize]
        public ActionResult Logout()
        {
            Response.Cookies.Delete("jwt", new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true
            });

            return Ok();

 var jwtToken = GenerateJwtTokenAsync(user);

                     return Ok(new Response()
                     {
                         Result = true,
                         Token = jwtToken
                     });
        } */
