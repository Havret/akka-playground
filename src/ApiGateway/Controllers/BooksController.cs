using Akka.Actor;
using Infrastructure.Sharding;
using InventoryManagement.Contact.Commands;
using InventoryManagement.Contact.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagement.Contact.Query;

namespace ApiGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : Controller
    {
        private readonly IActorRef _bookActor;
        private readonly IActorRef _bookQueryHandler;

        public BooksController(CreateBookActor createBookActor, CreateBookQueryHandler createBookQueryHandler)
        {
            _bookActor = createBookActor();
            _bookQueryHandler = createBookQueryHandler();
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

        [HttpGet()]
        public async Task<IActionResult> GetBooks()
        {
            var books = await _bookQueryHandler.Ask<IEnumerable<BookDto>>(new GetBooks());
            return Ok(books);
        }
    }
}