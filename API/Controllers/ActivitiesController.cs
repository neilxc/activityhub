using System.Threading.Tasks;
using Application.Activities;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Application.Activities.Details;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivitiesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ActivitiesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActivitiesEnvelope> List()
        {
            return await _mediator.Send(new List.Query());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Create.Command command)
        {
            var response = await _mediator.Send(command);
            return CreatedAtRoute("GetActivity", new {id = response.Id}, response);
        }

        [HttpGet("{id}", Name = "GetActivity")]
        public async Task<Activity> Details(int id)
        {
            return await _mediator.Send(new Details.Query(id));
        }

        [HttpPut("{id}")]
        public async Task<Activity> Edit(int id, [FromBody]Edit.Command command)
        {
            command.Id = id;
            return await _mediator.Send(command);
        }
    }
}