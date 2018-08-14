using Akka.Actor;
using Infrastructure.Sharding;
using InventoryManagement.Contact.Commands;
using InventoryManagement.Contact.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ApiGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : Controller
    {
        private readonly IActorRef _bookActor;

        public BooksController(CreateBookActor createBookActor)
        {
            _bookActor = createBookActor();
        }

        [HttpPost]
        public IActionResult CreateBook([FromBody]CreateBook command)
        {
            command = command.WithId(Guid.NewGuid());
            _bookActor.Tell(new ShardEnvelope(command.Id.ToString(), command));
            return Accepted();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(Guid id)
        {
            var bookDto = await _bookActor.Ask<BookDto>(new ShardEnvelope(id.ToString(), InventoryManagement.Contact.Query.GetBook.Instance));
            return Ok(bookDto);
        }
    }
}