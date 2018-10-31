using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Comments;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/activities")]
    public class CommentsController : BaseController
    {
        [HttpPost("{activityId}/comments")]
        public async Task<CommentDto> Create(int activityId, Create.Command command)
        {
            command.ActivityId = activityId;
            return await Mediator.Send(command);
        }

        [HttpGet("{activityId}/comments")]
        public async Task<List<CommentDto>> Get(int activityId)
        {
            return await Mediator.Send(new List.Query(activityId));
        }

        [HttpDelete("{activityId}/comments/{commentId}")]
        public async Task Delete(int activityId, int commentId)
        {
            await Mediator.Send(new Delete.Command(activityId, commentId));
        }
    }
}