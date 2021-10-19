using AG.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AG.Helpers
{
    public class JWTHelpers
    {

        public IOptions<AppSettings> _appSettings { get; set; }
        private readonly IHttpContextAccessor _httpContextAccessor;
        public JWTHelpers(IHttpContextAccessor httpContextAccessor, IOptions<AppSettings> appSettings)
        {
            _httpContextAccessor = httpContextAccessor;
            _appSettings = appSettings;
        }

        private string GenerateToken(UserDetails userDetails, DateTime? dtExpiry = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Value.Secret);
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

        public int GetUserSessionByToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            string accessToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split("Bearer")[1].Trim();
            var jsonData = tokenHandler.ReadJwtToken(accessToken.Trim());
            int userId = Convert.ToInt32(jsonData.Payload["unique_name"].ToString());
            return userId;
        }
    }
}
