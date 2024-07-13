using Lampros.Services.AuthAPI.Data;
using Lampros.Services.AuthAPI.Models;
using Lampros.Services.AuthAPI.Models.Dto;
using Lampros.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;

namespace Lampros.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {

        private readonly AuthDbContext _authDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AuthDbContext authDbContext, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _authDbContext = authDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _authDbContext.ApplicationUsers.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
            if(user is not null)
            {
                var roleExists = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;

        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _authDbContext.ApplicationUsers.FirstOrDefault(x => x.UserName.ToLower() == loginRequestDto.UserName.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

            if(user is null || !isValid)
            {
                return new LoginResponseDto() { User = null, Token = "" };
            }

            //if user was found and logged in successfully, generate token
            var token = _jwtTokenGenerator.GenerateToken(user);
            UserDto userDto = new()
            {
                Email = user.Email,
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            };

            
            LoginResponseDto loginResponseDto = new()
            {
                User = userDto,
                Token = token
            };

            return loginResponseDto;
        }

        //return type in tutorial changes to type string. Not a fan of this approach and would like to make 
        //standard return type
        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            //Don't need UserName table but Identity uses it as an index. Will explore what ignoring this column
            //through DbContext class does to functionality/performance.
            ApplicationUser user = new ApplicationUser
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                FirstName = registrationRequestDto.FirstName,
                LastName = registrationRequestDto.LastName,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                PhoneNumber = registrationRequestDto.PhoneNumber,
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if(result.Succeeded)
                {
                    var userToReturn =  _authDbContext.ApplicationUsers.First(x => x.Email == registrationRequestDto.Email);
                    UserDto userDto = new UserDto
                    {
                        Id = userToReturn?.Id,
                        Email = userToReturn.Email,
                        FirstName = userToReturn.FirstName,
                        LastName = userToReturn.LastName,
                        PhoneNumber = userToReturn.PhoneNumber
                    };
                    return "";

                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception)
            {

            }
            return "Error Encountered";
        }
    }
}
