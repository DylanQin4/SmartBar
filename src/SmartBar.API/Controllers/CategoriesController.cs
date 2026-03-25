using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartBar.Application.Features.Catalog.Categories.Commands.CreateCategory;
using SmartBar.Application.Features.Catalog.Categories.Commands.DeleteCategory;
using SmartBar.Application.Features.Catalog.Categories.Commands.UpdateCategory;
using SmartBar.Application.Features.Catalog.Categories.Queries.GetCategories;
using SmartBar.Application.Features.Catalog.Categories.Queries.GetCategoryById;

namespace SmartBar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CategoryDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCategoriesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryDetailDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCategoryByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var id = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route id does not match command id.");

        await sender.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteCategoryCommand(id), cancellationToken);
        return NoContent();
    }
}
