using ECommerce.Application.Features.Catalog;
using ECommerce.Application.Features.Catalog.CreateCategory;
using ECommerce.Application.Features.Catalog.DeleteCategory;
using ECommerce.Application.Features.Catalog.GetCategories;
using ECommerce.Application.Features.Catalog.GetCategoryById;
using ECommerce.Application.Features.Catalog.UpdateCategory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ISender _mediator;

    public CategoryController(ISender mediator) => _mediator = mediator;

    /// <summary>GET /api/category — list active categories.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _mediator.Send(new GetCategoriesQuery(), cancellationToken));

    /// <summary>GET /api/category/{id} — get one category.</summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var category = await _mediator.Send(new GetCategoryByIdQuery(id), cancellationToken);
        return category is null ? NotFound() : Ok(category);
    }

    /// <summary>POST /api/category — create a category.</summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    /// <summary>PUT /api/category/{id} — update a category.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("Route id and body id mismatch.");

        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>DELETE /api/category/{id} — delete a category.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCategoryCommand(id), cancellationToken);
        return NoContent();
    }
}
