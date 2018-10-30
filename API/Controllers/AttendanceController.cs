using System.Threading.Tasks;
using Application.Activities;
using Application.Attendances;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/activities")]
    public class AttendanceController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AttendanceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{id}/attend")]
        public async Task<ActivityDto> Add(int id)
        {
            return await _mediator.Send(new Add.Command(id));
        }

        [HttpDelete("{id}/attend")]
        public async Task<ActivityDto> Delete(int id)
        {
            return await _mediator.Send(new Delete.Command(id));
        }
    }
}