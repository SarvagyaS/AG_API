using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AG.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AG.Controllers
{
    [Route("api/user/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AGContext _AGContext;
        private readonly AppSettings _appSettings;

        public UserController(AGContext aGContext, IOptions<AppSettings> appSettings)
        {
            _AGContext = aGContext;
            _appSettings = appSettings.Value;
        }

        [HttpPost]
        public async Task<UserDetails> Register([FromBody]UserDetails ud)
        {
            try
            {
                ud.registration_date = DateTime.Now;
                ud.user_ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                _AGContext.UserDetails.Add(ud);
                await _AGContext.SaveChangesAsync();
                return ud;
            }
            catch (Exception ex)
            {
                return new UserDetails(); ;
            }
        }

        [HttpPost]
        public async Task<UserDetails> Authenticate([FromBody]Login u)
        {
            try
            {
                var user = await _AGContext.UserDetails.Include(a=>a.UserAddressDetails).Where(a => a.email == u.Username && a.password == u.Password).FirstOrDefaultAsync();
                if (user != null)
                {
                    user.token = GenerateToken(user);
                    if (user.UserAddressDetails != null)
                    {
                        foreach (var item in user.UserAddressDetails)
                        {
                            item.UserDetails = null;
                        }
                    }
                    return user;
                }
                return new UserDetails();
            }
            catch (Exception ex)
            {
                return new UserDetails();
            }
        }

        private string GenerateToken(UserDetails userDetails, DateTime? dtExpiry = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                       new Claim(ClaimTypes.Name,userDetails.Id.ToString()),
                }),
                Expires = dtExpiry == null ? DateTime.UtcNow.AddDays(7) : dtExpiry,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var Token = tokenHandler.CreateToken(tokenDescriptor);
            string strToKen = tokenHandler.WriteToken(Token);
            return strToKen;
        }

        [HttpPost]
        public async Task<UserAddressDetails> SaveAddressDetails([FromBody] UserAddressDetails u)
        {
            try
            {
                if (u.Id>0)
                {
                    var a = await _AGContext.UserAddressDetails.Where(a => a.Id == u.Id).FirstOrDefaultAsync();
                    if (a!= null)
                    {
                        a.postal_code = u.postal_code;
                        a.state = u.state;
                        a.address_line_1 = u.address_line_1;
                        a.address_line_2 = u.address_line_2;
                        a.city = u.city;
                        a.country = u.country;
                        await _AGContext.SaveChangesAsync();
                        return u;
                    }
                }
                else
                {
                    u.created_date = DateTime.Now;
                    _AGContext.Add(u);
                    await _AGContext.SaveChangesAsync();
                    return u;
                }
                return new UserAddressDetails();
            }
            catch (Exception ex)
            {
                return new UserAddressDetails();
            }
        }

    }
}
