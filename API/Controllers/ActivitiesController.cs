using System.Threading.Tasks;
using Application.Activities;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ActivitiesController : BaseController
    {
        [HttpGet]
        public async Task<ActivitiesEnvelope> List(string username, string activityFilter, int? limit, int? offset)
        {
            return await Mediator.Send(new List.Query(username, activityFilter, limit, offset));
        }

        [HttpPost]
        public async Task<IActionResult> Create(Create.Command command)
        {
            var response = await Mediator.Send(command);
            return CreatedAtRoute("GetActivity", new {id = response.Id}, response);
        }

        [HttpGet("{id}", Name = "GetActivity")]
        public async Task<ActivityDto> Details(int id)
        {
            return await Mediator.Send(new Details.Query(id));
        }

        [HttpPut("{id}")]
        public async Task<Activity> Edit(int id, [FromBody]Edit.Command command)
        {
            command.Id = id;
            return await Mediator.Send(command);
        }
    }
}