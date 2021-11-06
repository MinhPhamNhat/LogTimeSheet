using LogTimeSheet.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace LogTimeSheet.Utils
{
    public interface IJwt
    {
        string GenerateToken(User user);
        dynamic ValidateToken(string token);
    }

    public class Jwt : IJwt
    {
        string secretKey = "secretKeyPhaiChuaHon16KyTu";
        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) ,
                    new Claim("username", user.Username),
                    new Claim("role", user.Role.ToString()),
                    new Claim("id", user.UserId),
                    new Claim("name", user.Name)
                }),
                // generate token that is valid for 5 minute
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public dynamic ValidateToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var role = int.Parse(jwtToken.Claims.First(_ => _.Type == "role").Value);
                var username = jwtToken.Claims.First(_ => _.Type == "username").Value;
                var name = jwtToken.Claims.First(_ => _.Type == "name").Value;
                var id = jwtToken.Claims.First(_ => _.Type == "id").Value;

                // return user id from JWT token if validation successful
                return new { role, username, name, id };
            }
            catch
            {
                // return null if validation fails
                return null;
            }
        }
    }
}