using Microsoft.AspNetCore.Mvc;
using SmartBar.Application.Common.Mediator;
using SmartBar.Application.Features.Purchasing.PurchaseOrders.Commands.AddPurchaseLine;
using SmartBar.Application.Features.Purchasing.PurchaseOrders.Commands.AddSupplierPayment;
using SmartBar.Application.Features.Purchasing.PurchaseOrders.Commands.CancelPurchaseOrder;
using SmartBar.Application.Features.Purchasing.PurchaseOrders.Commands.CreatePurchaseOrder;
using SmartBar.Application.Features.Purchasing.PurchaseOrders.Queries.GetPurchaseOrderById;
using SmartBar.Application.Features.Purchasing.PurchaseOrders.Queries.GetPurchaseOrders;

namespace SmartBar.API.Controllers;

[ApiController]
[Route("api/purchase-orders")]
public class PurchaseOrdersController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<PurchaseOrderDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPurchaseOrdersQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PurchaseOrderDetailDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPurchaseOrderByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreatePurchaseOrderCommand command, CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPost("{id:guid}/lines")]
    public async Task<ActionResult<Guid>> AddLine(Guid id, AddPurchaseLineCommand command, CancellationToken cancellationToken)
    {
        if (id != command.PurchaseOrderId)
            return BadRequest("Route id does not match command purchase order id.");

        var lineId = await sender.Send(command, cancellationToken);
        return Ok(lineId);
    }

    [HttpPost("{id:guid}/payments")]
    public async Task<ActionResult<Guid>> AddPayment(Guid id, AddSupplierPaymentCommand command, CancellationToken cancellationToken)
    {
        if (id != command.PurchaseOrderId)
            return BadRequest("Route id does not match command purchase order id.");

        var paymentId = await sender.Send(command, cancellationToken);
        return Ok(paymentId);
    }

    [HttpPut("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new CancelPurchaseOrderCommand(id), cancellationToken);
        return NoContent();
    }
}
