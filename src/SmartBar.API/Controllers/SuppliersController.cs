using Microsoft.AspNetCore.Mvc;
using SmartBar.Application.Common.Mediator;
using SmartBar.Application.Features.Purchasing.Suppliers.Commands.CreateSupplier;
using SmartBar.Application.Features.Purchasing.Suppliers.Commands.DeactivateSupplier;
using SmartBar.Application.Features.Purchasing.Suppliers.Commands.UpdateSupplier;
using SmartBar.Application.Features.Purchasing.Suppliers.Queries.GetSupplierById;
using SmartBar.Application.Features.Purchasing.Suppliers.Queries.GetSuppliers;

namespace SmartBar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<SupplierDto>>> GetAll(
        [FromQuery] bool activeOnly = true,
        CancellationToken cancellationToken = default)
    {
        var result = await sender.Send(new GetSuppliersQuery(activeOnly), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SupplierDetailDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetSupplierByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateSupplierCommand command, CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateSupplierCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route id does not match command id.");

        await sender.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeactivateSupplierCommand(id), cancellationToken);
        return NoContent();
    }
}
