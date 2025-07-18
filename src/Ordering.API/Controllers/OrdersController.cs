using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Orders.Commands.CancelOrder;
using Ordering.Application.Orders.Commands.CreateOrder;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Ordering.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _dispatcher;

        public OrdersController(IMediator dispatcher)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        [HttpPost]
        [ProducesResponseType(typeof(Guid), Status201Created)]
        [ProducesResponseType(Status400BadRequest)]
        public async Task<ActionResult<Guid>> CreateOrder([FromBody] CreateOrderCommand command)
        {
            var response = await _dispatcher.Send(command);
            return CreatedAtAction(nameof(CreateOrder), response);
        }

        [HttpPut("{orderId:guid}/cancel")]
        [ProducesResponseType(typeof(CancelOrderResponse), Status200OK)]
        [ProducesResponseType(typeof(CancelOrderResponse), Status400BadRequest)]
        [ProducesResponseType(typeof(CancelOrderResponse), Status404NotFound)]
        public async Task<ActionResult<CancelOrderResponse>> CancelOrder(
            [FromRoute] Guid orderId,
            [FromBody] CancelOrderCommand command,
            CancellationToken cancellationToken)
        {
            var response = await _dispatcher.Send(command, cancellationToken);
            return Ok(response);
        }
    }
}
