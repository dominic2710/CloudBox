using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using CloudBox.Api.Entities;
using CloudBox.Api.Controllers.User;

namespace CloudBox.Api.Functions.User
{
    public class UserFunction : IUserFunction
    {
        private readonly CloudBoxContext context;
        private readonly IConfiguration configuration;
        public UserFunction(CloudBoxContext _context, IConfiguration _configuration)
        {
            context = _context;
            configuration = _configuration;
        }

        public User? Authenticate(string email, string password)
        {
            try
            {
                var entity = context.TblUsers.SingleOrDefault(x => x.Email == email);
                if (entity == null) return null;

                var isPasswordMatched = VertifyPassword(password, entity.Password);

                if (!isPasswordMatched) return null;

                var token = GenerateJwtToken(entity);

                return new User
                {
                    Id = entity.Id,
                    UserName = entity.UserName,
                    AccessToken = token,
                    Avatar = entity.Avatar,
                    Email = entity.Email,
                    LastLogonTime = entity.LastLogonTime,
                    PhoneNumber = entity.PhoneNumber,
                    SercurityGroupId = entity.SercurityGroupId,
                    Status = entity.Status,
                };

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public User GetUserById(int id)
        {
            var entity = context.TblUsers
                .Where(x => x.Id == id)
                .FirstOrDefault();

            if (entity == null) return new User();

            return new User
            {
                Id = entity.Id,
                UserName = entity.UserName,
                Avatar = entity.Avatar,
                Email = entity.Email,
                LastLogonTime = entity.LastLogonTime,
                PhoneNumber = entity.PhoneNumber,
                SercurityGroupId = entity.SercurityGroupId,
                Status = entity.Status,
            };
        }

        public int Register(RegisterRequest request)
        {
            var exist = context.TblUsers.Where(x => x.Email == request.Email);
            if (exist.Any())
            {
                throw new Exception("Email already in use");
            }

            var entity = new TblUser
            {
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                SercurityGroupId = 1,
                Status = "Registered",
                Password = GeneratePasswordHash(request.Password)
            };

            context.TblUsers.Add(entity);
            context.SaveChanges();

            return 1;
        }

        private bool VertifyPassword(string enteredPassword, string storedPassword)
        {
            var encryptyedPassword = GeneratePasswordHash(enteredPassword);

            return encryptyedPassword.Equals(storedPassword);
        }

        private string GeneratePasswordHash(string enteredPassword)
        {
            string encryptyedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: enteredPassword,
                salt: Encoding.ASCII.GetBytes(configuration["StoredSalt"] ?? "Abc12345"),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return encryptyedPassword;
        }

        private string GenerateJwtToken(TblUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"] ?? "Tam123@1");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.Now.AddDays(int.Parse(configuration["Jwt:Expire"] ?? "1440")),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
