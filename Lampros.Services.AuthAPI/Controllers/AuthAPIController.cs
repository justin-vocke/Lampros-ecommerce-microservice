using Lampros.MessageBus;
using Lampros.Services.AuthAPI.Models.Dto;
using Lampros.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lampros.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly IMessageBus _messageBus;
        protected ResponseDto _responseDto;

        public AuthAPIController(IAuthService authService, IConfiguration configuration, IMessageBus messageBus)
        {
            _authService = authService;
            _responseDto = new();
            _configuration = configuration;
            _messageBus = messageBus;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto registrationRequestDto)
        {
            var errorMessage = await _authService.Register(registrationRequestDto);
            if(!string.IsNullOrEmpty(errorMessage))
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = errorMessage;
                return BadRequest(_responseDto);
            }
            var email = registrationRequestDto.Email;
            await _messageBus.PublishMessage(email, _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"));

            return Ok(_responseDto);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var loginResponse = await _authService.Login(loginRequestDto);

            if(loginResponse.User is null)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Please double check your credentials";
                return BadRequest(_responseDto);
            }
            _responseDto.Result = loginResponse;
            return Ok(_responseDto);    
        }

        [HttpPost]
        [Route("assignrole")]
        public async Task<IActionResult> AddRole([FromBody] RegistrationRequestDto registrationRequestDto)
        {
            var assignRoleSuccessful = await _authService.AssignRole(registrationRequestDto.Email, registrationRequestDto.RoleName.ToUpper());

            if (!assignRoleSuccessful)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Error Encountered";
                return BadRequest(_responseDto);
            }
            
            return Ok(_responseDto);
        }
    }
}
