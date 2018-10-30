using System.Threading.Tasks;
using Application.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<User> Create(Create.Command command)
        {
            return await _mediator.Send(command);
        }

        [HttpPost("login")]
        public async Task<User> Login(Login.Command command)
        {
            return await _mediator.Send(command);
        }
    }
}